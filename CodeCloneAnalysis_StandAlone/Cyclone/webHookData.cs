using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyclone
{
    public class webHookData
    {
        public String after;
        public String before;
        public String entryTime;
        public String headCommitAdded;
        public String headCommitId;
        public String headCommitMessage;
        public String headCommitModified;
        public String headCommitRemoved;
        public String headCommitUrl;
        public String isSync;
        public String repositoryId;
        public String repositoryUrl;
        public String TimeSync;
        public String webHookRefId;
        public files[] files;
        public commits[] commits;
    }
}
