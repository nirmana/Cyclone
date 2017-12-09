using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeCloneAnalysis_DB;

namespace CodeCloneAnalysis_BL
{
    public class TypeDetector
    {
        public void getTypes(String LocalFilePath)
        {
            var ext = new List<string> {".java"};
            //TODO: Project path eka UI eken aragen danda
            var myFiles = Directory.GetFiles(@LocalFilePath, "*.*", SearchOption.AllDirectories)
                      .Where(s => ext.Any(e => s.EndsWith(e)));

             using(var _cycloneEntities = new cycloneEntities())
             {
               string analyzerid= _cycloneEntities.source_analyzer.OrderByDescending(a=>a.processed_time).FirstOrDefault().analyzer_id;
               var filesList = _cycloneEntities.clone_class.Where(a => a.analyzer_id == analyzerid).ToList();
                 foreach(clone_class cloneclass in filesList){
                  
                     var fragmentList = _cycloneEntities.clone_fragment.Where(a => a.analyzer_id == analyzerid && a.clone_class_id==cloneclass.cloneclass_id).ToList();
                     foreach (clone_fragment cf in fragmentList)
                     {
                         int i = 0;
                         if (i > 0) break;
                         String CodeBlockI = getCodeByStartAndEnd(cf.source_path, cf.start_line, cf.end_line);
                         bool isTypeII = false;
                         for (int j = 0; j < fragmentList.Count; j++)
                         {
                             if (i < j)
                             {
                                 String CodeBlockII = getCodeByStartAndEnd(fragmentList[j].source_path,
                                    fragmentList[j].start_line, fragmentList[j].end_line);
                                 Boolean status = CompareCodeBlocks(CodeBlockI, CodeBlockII);
                                 
                                 if (!status)
                                 {
                                     isTypeII = true;
                                     break;
                                 }
                             } 
                         }
                         clone_class cloneClass = _cycloneEntities.clone_class.Where(a => a.cloneclass_id == cloneclass.cloneclass_id && a.analyzer_id == cloneclass.analyzer_id).FirstOrDefault();
                         if (isTypeII)
                         {
                             cloneClass.clone_class_type = "Type2";
                         }
                         else
                         {
                             cloneClass.clone_class_type = "Type1";
                         }
                         _cycloneEntities.clone_class.Attach(cloneClass);
                        var entry = _cycloneEntities.Entry(cloneClass);
                        entry.Property(e => e.clone_class_type).IsModified = true;
                        _cycloneEntities.SaveChanges();
                         i++;
                     }
                 }
             }
        }

        private bool CompareCodeBlocks(string CodeBlockI, string CodeBlockII)
        {
            if (CodeBlockI.Trim() == CodeBlockII.Trim())
                return true;
            return false;
        }

        private string getCodeByStartAndEnd(string path, int? startline, int? endline)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                String selectedCode = "";
                int processingLine = 0;
                while (sr.ReadLine() != null)
                {
                    processingLine++;
                    if (startline <= processingLine && endline >= processingLine)
                        selectedCode += sr.ReadLine();
                    if (processingLine > endline) break;
                }
                return selectedCode;
            }
        }
    }
}
