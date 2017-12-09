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

namespace CodeCloneAnalysis_BL
{
    public class CloneAnalyzer
    {
        String projectREF, workingDirectory, DefaultOutputLocation, batchFileLocation, ProjectName, WAMPVisualizationPath;
        public CloneAnalyzer(String _ProjectName)
        {
            projectREF = Guid.NewGuid().ToString();
            ProjectName = _ProjectName;
            workingDirectory = ConfigurationSettings.AppSettings["DefaultSourceLocation"];
            DefaultOutputLocation = ConfigurationSettings.AppSettings["DefaultOutputLocation"];
            batchFileLocation = ConfigurationSettings.AppSettings["CONQATEngineBATPath"];
            WAMPVisualizationPath = ConfigurationSettings.AppSettings["WAMPVisualizationPath"];


        }

        /// <summary>
        /// process local folder path
        /// </summary>
        /// <param name="LocalFilePath"></param>
        public Boolean ProcessLocalFilePath(string LocalFilePath, string ProjectName, string GitHubUrl, string TimeInterval, out string analyzeId)
        {
            Boolean isSuccess = true;
            String strStatus = "";
            analyzeId = "";
            if (!String.IsNullOrEmpty(workingDirectory))
            {
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
                                        psi.CreateNoWindow = false;
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
                                            strStatus = "Clones not found!";
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
                                            sourceAnalyzer.source_path = LocalFilePath;
                                            source source = new source();
                                            source.create_date = DateTime.Now;
                                            source.location = LocalFilePath;
                                            source.source_id = projectREF;
                                            source.source_name = ProjectName;
                                            source.git_url = GitHubUrl;
                                            source.time_intervals_id = TimeInterval;
                                            using (var _cycloneEntities = new cycloneEntities())
                                            {
                                                _cycloneEntities.sources.Add(source);
                                                _cycloneEntities.SaveChanges();
                                                foreach (source_file sourceFile in sourceFileList)
                                                {
                                                    sourceFile.source_id = source.source_id;
                                                    _cycloneEntities.source_file.Add(sourceFile);
                                                }
                                                _cycloneEntities.SaveChanges();

                                                _cycloneEntities.source_analyzer.Add(sourceAnalyzer);
                                                _cycloneEntities.SaveChanges();
                                                analyzeId = sourceAnalyzer.analyzer_id;
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
                                            }
                                        }
                                        TypeDetector _typeDetector = new TypeDetector();
                                        _typeDetector.getTypes(LocalFilePath);
                                        SetUpFilesForVisualizaton(analyzeId);

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
                                    string cqrFile = DefaultOutputLocation + projectREF + @"\" + projectREF + ".cqr";
                                    var command = @batchFileLocation + "conqat.bat" + @" -f  " + @"""" + cqrFile + @"""";
                                    File.WriteAllText(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".bat", command);
                                    ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval, out analyzeId);
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
                                    ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval, out analyzeId);
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
                                    .Replace("{rootDir}", LocalFilePath)
                                    .Replace("{xmlOutput}", @DefaultOutputLocation + projectREF + @"\")
                                    .Replace("{htmlPresentationOutput}", @DefaultOutputLocation + projectREF + @"\");
                                File.WriteAllText(workingDirectory + "\\" + projectREF + "\\" + projectREF + ".cqb", cqbText);
                                ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval, out analyzeId);
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(workingDirectory + "\\" + projectREF);
                        ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval, out analyzeId);
                    }
                }
                else
                {
                    Directory.CreateDirectory(workingDirectory);
                    ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval, out analyzeId);
                }
            }
            return isSuccess;
        }

        private void SetUpFilesForVisualizaton(string analyzeId)
        {
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

        public Boolean CreateBaseLocationForSource()
        {
            try
            {
                if (!String.IsNullOrEmpty(workingDirectory))
                {
                    if (Directory.Exists(workingDirectory))
                    {
                        grantAccess(workingDirectory);
                        if (Directory.Exists(workingDirectory + "\\GitSources"))
                        {
                            grantAccess(workingDirectory + "\\GitSources");
                        }
                        else
                        {
                            Directory.CreateDirectory(workingDirectory + "\\GitSources");
                            CreateBaseLocationForSource();
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(workingDirectory);
                        CreateBaseLocationForSource();
                    }
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

        public List<source> GetAllOldAnalyzeDetails()
        {
            using (var _cycloneEntities = new cycloneEntities())
            {
                return _cycloneEntities.sources.ToList();
            }
        }
        public List<source_analyzer> GetAllDetailsByID(String id)
        {
            using (var _cycloneEntities = new cycloneEntities())
            {
                return _cycloneEntities.source_analyzer.Where(a => a.source_id == id).OrderByDescending(a => a.processed_time).ToList();
            }
        }

        public source_analyzer GetAnalyzeDetailsByID(string _sourceAnalyzeId)
        {
            using (var _cycloneEntities = new cycloneEntities())
            {
                return _cycloneEntities.source_analyzer.Where(a => a.analyzer_id == _sourceAnalyzeId).FirstOrDefault();
            }
        }
    }
}
