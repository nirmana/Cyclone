using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHookManager
{
    public class webhook_response
    {
        public string before;
        public string after;
        public head_commit head_commit;
        public repository repository;
        public webHookComits[] commits;
    }
}