using CodeCloneAnalysis_DB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Cyclone
{
    public class CloneAnalyzer
    {
        cycloneEntities _cycloneEntities;
        String workingDirectory, WAMPVisualizationPath;
        public Boolean TimeToTimeAnalyse(source _source)
        {
            try
            {
                workingDirectory = ConfigurationSettings.AppSettings["DefaultSourceLocation"];
                using (_cycloneEntities = new cycloneEntities())
                {
                    String SourcePath = "";
                    List<source_analyzer> sourceAnalyzerList = _cycloneEntities.source_analyzer.Where(a => a.source_id == _source.source_id).ToList();
                    if (sourceAnalyzerList.Count > 0)
                    {
                        var LastSourcePath = sourceAnalyzerList.OrderByDescending(a => a.processed_time)
                            .FirstOrDefault().source_path;
                        if (LastSourcePath != null && LastSourcePath != "")
                        {
                            SourcePath = LastSourcePath;
                        }
                        else
                        {
                            SourcePath = _source.location;
                        }
                    }
                    else
                    {
                        SourcePath = _source.location;
                    }

                    if (SourcePath != "")
                    {
                        var uniqueId = Guid.NewGuid().ToString();
                        var DestinationPath = workingDirectory + "\\GitSources\\" + uniqueId;
                        if (!Directory.Exists(DestinationPath))
                        {
                            Directory.CreateDirectory(DestinationPath);
                            grantAccess(DestinationPath);
                        }

                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);

                        TimelyAnalyzeSource(DestinationPath, _source.source_id, _source);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
            

        }

        private void TimelyAnalyzeSource(string DestinationPath, string projectREF, source source)
        {
            workingDirectory = ConfigurationSettings.AppSettings["DefaultSourceLocation"];
            String DefaultOutputLocation = ConfigurationSettings.AppSettings["DefaultOutputLocation"];
            String batchFileLocation = ConfigurationSettings.AppSettings["CONQATEngineBATPath"];
            if (Directory.Exists(workingDirectory))
            {
                grantAccess(workingDirectory);

                if (Directory.Exists(workingDirectory + "\\" + projectREF))
                {
                    grantAccess(workingDirectory + "\\" + projectREF);
                    if (File.Exists(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".cqb"))
                    {
                        if (File.Exists(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".cqr"))
                        {
                            var batchFile = workingDirectory + "\\" + projectREF + "\\" + projectREF + ".bat";
                            if (File.Exists(batchFile))
                            {
                                try
                                {

                                    Process p;
                                    ProcessStartInfo psi = new ProcessStartInfo();
                                    psi.UseShellExecute = false;
                                    psi.CreateNoWindow = true;
                                    psi.FileName = batchFile;
                                    psi.Arguments = " /k start /wait";
                                    p = System.Diagnostics.Process.Start(psi);
                                    do
                                    {
                                        if (!p.HasExited) { Thread.Sleep(1000); }

                                    } while (!p.WaitForExit(1000));

                                    p.WaitForExit();
                                    p.Close();
                                    //read the xml and update the database
                                    if (!File.Exists(workingDirectory + "\\" + projectREF + "\\" + "clones.xml"))
                                    {
                                        //strStatus = "Clones not found!";
                                    }
                                    else
                                    {
                                        XmlReader xmlReader = new XmlReader();
                                        source_analyzer sourceAnalyzer = new source_analyzer();
                                        List<source_file> sourceFileList = new List<source_file>();
                                        List<clone_class> cloneClassList = new List<clone_class>();
                                        List<clone_fragment> cloneFragmentList = new List<clone_fragment>();
                                        sourceAnalyzer = xmlReader.ReadCloneXml(workingDirectory + "\\" + projectREF + "\\" + "clones.xml",
                                           projectREF, out sourceFileList, out cloneClassList, out cloneFragmentList);
                                        sourceAnalyzer.source_path = DestinationPath;
                                        //source source = new source();
                                        //source.create_date = DateTime.Now;
                                        //source.location = LocalFilePath;
                                        //source.source_id = projectREF;
                                        //source.source_name = ProjectName;
                                        //source.git_url = GitHubUrl;
                                        using (var _cycloneEntities = new cycloneEntities())
                                        {
                                            //_cycloneEntities.sources.Add(source);
                                            _cycloneEntities.SaveChanges();
                                            foreach (source_file sourceFile in sourceFileList)
                                            {
                                                sourceFile.source_id = source.source_id;
                                                _cycloneEntities.source_file.Add(sourceFile);
                                            }
                                            _cycloneEntities.SaveChanges();

                                            _cycloneEntities.source_analyzer.Add(sourceAnalyzer);
                                            _cycloneEntities.SaveChanges();
                                            foreach (clone_class cloneClass in cloneClassList)
                                            {
                                                _cycloneEntities.clone_class.Add(cloneClass);

                                            }
                                            _cycloneEntities.SaveChanges();
                                            foreach (clone_fragment cloneFragment in cloneFragmentList)
                                            {
                                                _cycloneEntities.clone_fragment.Add(cloneFragment);
                                            }
                                            _cycloneEntities.SaveChanges();
                                            TypeDetector _typeDetector = new TypeDetector();
                                            _typeDetector.getTypes(DestinationPath);
                                            SetUpFilesForVisualizaton(sourceAnalyzer.analyzer_id);
                                        }
                                    }
                                }
                                catch (DbEntityValidationException ex)
                                {

                                    // Retrieve the error messages as a list of strings.
                                    var errorMessages = ex.EntityValidationErrors
                                            .SelectMany(x => x.ValidationErrors)
                                            .Select(x => x.ErrorMessage);

                                    // Join the list to a single string.
                                    var fullErrorMessage = string.Join("; ", errorMessages);

                                    // Combine the original exception message with the new one.
                                    var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                                    // Throw a new DbEntityValidationException with the improved exception message.
                                    throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
                                }
                            }
                            else
                            {
                                string cqrFile = workingDirectory + projectREF + @"\" + projectREF + ".cqr";
                                var command = @batchFileLocation + "conqat.bat" + @" -f  " + @"""" + cqrFile + @"""";
                                File.WriteAllText(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".bat", command);
                                TimelyAnalyzeSource(DestinationPath, projectREF, source);
                            }
                        }
                        else
                        {
                            //creating cqr
                            using (StreamReader sr = new StreamReader("Templates\\CQR\\template.cqr"))
                            {
                                String cqrText = sr.ReadToEnd();
                                cqrText = cqrText.Replace("{blockSpecname}", projectREF);
                                File.WriteAllText(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".cqr", cqrText);
                                TimelyAnalyzeSource(DestinationPath, projectREF, source);

                            }
                        }
                    }
                    else
                    {
                        //creating cqb
                        using (StreamReader sr = new StreamReader("Templates\\CQB\\template.cqb"))
                        {
                            String cqbText = sr.ReadToEnd();
                            cqbText = cqbText.Replace("{blockSpecname}", projectREF)
                                .Replace("{rootDir}", DestinationPath)
                                .Replace("{xmlOutput}", @DefaultOutputLocation + projectREF + @"\")
                                .Replace("{htmlPresentationOutput}", @DefaultOutputLocation + projectREF + @"\");
                            File.WriteAllText(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".cqb", cqbText);
                            TimelyAnalyzeSource(DestinationPath, projectREF, source);

                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(workingDirectory + "\\" + projectREF);
                    TimelyAnalyzeSource(DestinationPath, projectREF, source);

                }
            }
        }
        public void grantAccess(String path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                FileSystemRights.FullControl, InheritanceFlags.ObjectInherit |
                InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow));
           // dInfo.SetAccessControl(dSecurity);
        }

        private void SetUpFilesForVisualizaton(string analyzeId)
        {
            WAMPVisualizationPath = ConfigurationSettings.AppSettings["WAMPVisualizationPath"];
            String LocalFilePathFromCopy = "";
            using (var _cycloneEntities = new cycloneEntities())
            {
                LocalFilePathFromCopy = _cycloneEntities.source_analyzer.Where(a => a.analyzer_id == analyzeId)
                    .FirstOrDefault().source_path;

            }
            if (LocalFilePathFromCopy != "")
            {
                if (!Directory.Exists(WAMPVisualizationPath + "\\sources"))
                {
                    Directory.CreateDirectory(WAMPVisualizationPath + "\\sources");
                    SetUpFilesForVisualizaton(analyzeId);
                }
                else
                {
                    if (!Directory.Exists(WAMPVisualizationPath + "\\sources\\" + analyzeId))
                    {
                        Directory.CreateDirectory(WAMPVisualizationPath + "\\sources\\" + analyzeId);
                        SetUpFilesForVisualizaton(analyzeId);
                    }
                    else
                    {
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(LocalFilePathFromCopy, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(LocalFilePathFromCopy, WAMPVisualizationPath + "\\sources\\" + analyzeId));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(LocalFilePathFromCopy, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(LocalFilePathFromCopy, WAMPVisualizationPath + "\\sources\\" + analyzeId), true);
                    }
                }
            }
        }
    }
}
