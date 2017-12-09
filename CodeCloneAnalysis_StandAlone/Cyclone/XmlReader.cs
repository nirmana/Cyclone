using CodeCloneAnalysis_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Cyclone
{
    public class XmlReader
    {
        public XmlReader() { 
        
        
        }

        internal source_analyzer ReadCloneXml(String _xmlpath,String sourceId,
            out List<source_file> _sourceFileList,out List<clone_class> _cloneClassList,
            out List<clone_fragment> _cloneFragmentList)
        {
            source_analyzer sourceAnalyzer = new source_analyzer();
            List<source_file> sourceFileList = new List<source_file>();
            List<clone_class> cloneClassList = new List<clone_class>();
            List<clone_fragment> cloneFragmentList = new List<clone_fragment>();
            sourceAnalyzer.analyzer_id = Guid.NewGuid().ToString();
            XmlDocument xml = new XmlDocument();
            XmlNamespaceManager manager = new XmlNamespaceManager(xml.NameTable);
            manager.AddNamespace("ns",
                "http://conqat.cs.tum.edu/ns/clonereport");
            xml.Load(_xmlpath);
          
            XmlNodeList sourceFileXMLList = xml.SelectNodes("/ns:cloneReport/ns:sourceFile", manager);
            XmlNodeList cloneClassXMLList = xml.SelectNodes("/ns:cloneReport/ns:cloneClass", manager);
            int totalLOC = 0;
            foreach (XmlNode xmlNode in sourceFileXMLList)
            {
                string id = xmlNode.Attributes["id"].Value;
                string path = xmlNode.Attributes["path"].Value;
                string location = xmlNode.Attributes["location"].Value;
                string length = xmlNode.Attributes["length"].Value;
                string fingerprint = xmlNode.Attributes["fingerprint"].Value;
                source_file source =new source_file();
                source.file_id = id;
                source.file_path = location;
                source.line_of_code = Convert.ToInt32(length);
                source.source_id = sourceId;
                source.analyzer_id = sourceAnalyzer.analyzer_id;
                totalLOC += Convert.ToInt32(length);
                sourceFileList.Add(source);
            }

            foreach (XmlNode xmlNode in cloneClassXMLList)
            {
               // XmlNodeList cloneNodeList = xmlNode.ChildNodes.SelectNodes("/ns:clone", manager);
                clone_class cloneClass = new clone_class();
                cloneClass.cloneclass_id = xmlNode.Attributes["id"].Value;
                cloneClass.clone_fragments_count = xmlNode.ChildNodes.Count;
                cloneClass.clone_lines_count = Convert.ToInt32(xmlNode.Attributes["normalizedLength"].Value);
                cloneClass.analyzer_id = sourceAnalyzer.analyzer_id;
                cloneClass.source_id = sourceAnalyzer.source_id;
            //    cloneClass.source_analyzer_analyzer_id = sourceAnalyzer.analyzer_id;
                cloneClassList.Add(cloneClass);

                foreach (XmlNode _xmlNode in xmlNode.ChildNodes)
                {
                    clone_fragment cloneFragment = new clone_fragment();
                    cloneFragment.clone_fragment_id = _xmlNode.Attributes["id"].Value;
                    cloneFragment.clone_lines_count =Convert.ToInt32( _xmlNode.Attributes["lengthInUnits"].Value);
                    cloneFragment.start_line = Convert.ToInt32(_xmlNode.Attributes["startLine"].Value);
                    cloneFragment.end_line = Convert.ToInt32(_xmlNode.Attributes["endLine"].Value);
                    cloneFragment.clone_class_id= cloneClass.cloneclass_id;
                    cloneFragment.source_file_id = _xmlNode.Attributes["sourceFileId"].Value;
                    cloneFragment.analyzer_id = sourceAnalyzer.analyzer_id;
                    cloneFragment.source_path= sourceFileList.Where(a => a.file_id == cloneFragment.source_file_id).FirstOrDefault().file_path;
                    cloneFragmentList.Add(cloneFragment);
                }
            }

            sourceAnalyzer.source_file_count = sourceFileXMLList.Count;
            sourceAnalyzer.clone_classes_count = cloneClassXMLList.Count;
            sourceAnalyzer.source_id = sourceId;
            sourceAnalyzer.processed_time = DateTime.Now;
            sourceAnalyzer.clones_count = cloneFragmentList.Count;
            sourceAnalyzer.line_of_code_count = totalLOC;
            _cloneClassList = cloneClassList;
            _cloneFragmentList = cloneFragmentList;
            _sourceFileList = sourceFileList;
            return sourceAnalyzer;
        }
    }
}
