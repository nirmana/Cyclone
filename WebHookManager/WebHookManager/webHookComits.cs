using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHookManager
{
    public class webHookComits
    {
        public Int64 webHookId;
        public String id;
        public String message;
        public String timestamp;
        public String url;
        public author author;
        public committer committer;
        public String[] added;
        public String[] removed;
        public String[] modified;

    }
}