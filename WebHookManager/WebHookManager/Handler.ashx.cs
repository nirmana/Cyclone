using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WebHookManager
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                using (StreamReader inputStream = new StreamReader(HttpContext.Current.Request.InputStream))
                {
                    var jsonString = inputStream.ReadToEnd();
                    webhook_response whRes = JsonConvert.DeserializeObject<webhook_response>(jsonString);
                    if (whRes != null)
                    {
                        using (dbd63c48f65aad4276b507a55c00bad8ffEntities contex = new dbd63c48f65aad4276b507a55c00bad8ffEntities())
                        {
                            using (var dbContextTransaction = contex.Database.BeginTransaction())
                            {
                                try
                                {
                                    webHook wh = new webHook();
                                    wh.after = whRes.after;
                                    wh.before = whRes.before;
                                   // wh.entryTime = DateTime.UtcNow.AddHours(5.30);
                                    wh.entryTime = DateTime.Now;
                                    wh.headCommitAdded = string.Join(",", whRes.head_commit.added);
                                    wh.headCommitId = whRes.head_commit.id;
                                    wh.headCommitMessage = whRes.head_commit.message;
                                    wh.headCommitModified = string.Join(",", whRes.head_commit.modified);
                                    wh.headCommitRemoved = string.Join(",", whRes.head_commit.removed);
                                    wh.headCommitUrl = whRes.head_commit.url;
                                    wh.isSync = false;
                                    wh.repositoryId = whRes.repository.id;
                                    wh.repositoryUrl = whRes.repository.url;
                                    wh.TimeSync = null;

                                    contex.webHooks.Add(wh);
                                    contex.SaveChanges();
                                    List<webHookFile> whf = GetChangesByHeadCommitId(wh.webHookRefId, whRes.head_commit.id, whRes.repository.full_name);
                                    if (whf != null)
                                    {
                                        contex.webHookFiles.AddRange(whf);
                                    }
                                    List<webHookCommit> webhookCommitList = new List<webHookCommit>();
                                    foreach (var item in whRes.commits)
                                    {
                                        webHookCommit whc = new webHookCommit();
                                        whc.added = string.Join(",", item.added);
                                        whc.authorName = item.author.name;
                                        whc.authorUserName = item.author.username;
                                        whc.committerName = item.committer.name;
                                        whc.committerUserName = item.committer.username;
                                        whc.gitCommitId = item.id;
                                        whc.message = item.message;
                                        whc.modified = string.Join(",", item.modified);
                                        whc.removed = string.Join(",", item.removed);
                                        whc.timestamp = item.timestamp;
                                        whc.url = item.url;
                                        whc.webHookId = wh.webHookRefId;
                                        webhookCommitList.Add(whc);
                                    }
                                    if (webhookCommitList != null)
                                    {
                                        contex.webHookCommits.AddRange(webhookCommitList);
                                    }
                                    contex.SaveChanges();
                                    dbContextTransaction.Commit();
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
                                    dbContextTransaction.Rollback();
                                }
                                finally
                                {
                                    dbContextTransaction.Dispose();
                                }
                            }
                        }
                    }
                }
                context.Response.StatusCode = 200;
                context.Response.Write("success");
            }
            catch (Exception)
            {
                context.Response.StatusCode = 500;
                context.Response.Write("error");
            }
        }

        private List<webHookFile> GetChangesByHeadCommitId(long webHookRefId, string sha, string fullRepoName)
        {
            List<webHookFile> webHookFileList = new List<webHookFile>();
            String url = "https://api.github.com/repos/" + fullRepoName + "/commits/" + sha;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "GET";
            request.Accept = "*";
            request.UserAgent = "Code Sample Web Client";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                if (responseText != "")
                {
                    commitChanges whRes = JsonConvert.DeserializeObject<commitChanges>(responseText);
                    if (whRes.files.Count() > 0)
                    {
                        foreach (file file in whRes.files)
                        {
                            webHookFile whf = new webHookFile();
                            whf.additions = file.additions;
                            whf.changes = file.changes;
                            whf.deletions = file.deletions;
                            whf.fileName = file.filename;
                            whf.patch = file.patch;
                            whf.Status = file.status;
                            whf.webHookId = webHookRefId;
                            webHookFileList.Add(whf);
                        }
                    }
                    else
                    {
                        webHookFileList = null;
                    }
                }
            }
            return webHookFileList;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}