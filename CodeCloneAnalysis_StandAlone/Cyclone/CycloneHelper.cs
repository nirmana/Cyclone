
using CodeCloneAnalysis_DB;
using Cyclone.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Script.Serialization;

namespace Cyclone
{
    public class CycloneHelper
    {
        onlineDBEntities _onlineDBEntities;
        cycloneEntities _cycloneEntities;
        public void RetriveFromHook()
        {
            using (_onlineDBEntities = new onlineDBEntities())
            {
                //using (TransactionScope dbContextTransaction = new TransactionScope())
                //{
                try
                {
                    List<webHook> webHookList = _onlineDBEntities.webHooks
                   .Where(a => a.isSync != true).ToList();

                    List<CodeCloneAnalysis_DB.webhook> webHookListForMYSQL = new List<webhook>();
                    List<CodeCloneAnalysis_DB.webhook_commits> webHookCommitsForMYSQL = new List<webhook_commits>();
                    List<CodeCloneAnalysis_DB.webhook_files> webHookFilesForMYSQL = new List<webhook_files>();
                    String source_id = "";

                    foreach (webHook _webhook in webHookList)
                    {
                        CodeCloneAnalysis_DB.webhook wh = new CodeCloneAnalysis_DB.webhook();
                        wh.after_id = _webhook.after;
                        wh.before_id = _webhook.before;
                        wh.entry_time = _webhook.entryTime;
                        wh.head_commit_added = _webhook.headCommitAdded;
                        wh.head_commit_id = _webhook.headCommitId;
                        wh.head_commit_message = _webhook.headCommitMessage;
                        wh.head_commit_modified = _webhook.headCommitModified;
                        wh.head_commit_removed = _webhook.headCommitRemoved;
                        wh.head_commit_url = _webhook.headCommitUrl;
                        wh.repository_id = _webhook.repositoryId;
                        wh.repository_url = _webhook.repositoryUrl;
                        wh.webhook_ref_id = _webhook.webHookRefId;
                        wh.source_id = GetSourceIdByUrl(_webhook.repositoryUrl);
                        source_id = wh.source_id;
                        webHookListForMYSQL.Add(wh);

                        foreach (webHookFile _webHookfile in _webhook.webHookFiles)
                        {
                            CodeCloneAnalysis_DB.webhook_files whf = new webhook_files();
                            whf.additions = _webHookfile.additions;
                            whf.changes = _webHookfile.changes;
                            whf.deletions = _webHookfile.deletions;
                            whf.file_id = _webHookfile.fileId;
                            whf.file_name = _webHookfile.fileName;
                            whf.patch = _webHookfile.patch;
                            whf.status = _webHookfile.Status;
                            whf.webhook_ref_id = _webhook.webHookRefId;
                            webHookFilesForMYSQL.Add(whf);
                        }

                        foreach (webHookCommit _webHookCommit in _webhook.webHookCommits)
                        {
                            CodeCloneAnalysis_DB.webhook_commits whc = new webhook_commits();
                            whc.added = _webHookCommit.added;
                            whc.author_name = _webHookCommit.authorName;
                            whc.author_user_name = _webHookCommit.authorUserName;
                            whc.committer_name = _webHookCommit.committerName;
                            whc.committer_user_name = _webHookCommit.committerUserName;
                            whc.git_commit_id = _webHookCommit.gitCommitId;
                            whc.icommit_id = _webHookCommit.icommitId;
                            whc.message = _webHookCommit.message;
                            whc.modified = _webHookCommit.modified;
                            whc.removed = _webHookCommit.removed;
                            whc.timestamp = _webHookCommit.timestamp;
                            whc.url = _webHookCommit.url;
                            whc.webhook_ref_id = _webhook.webHookRefId;
                            webHookCommitsForMYSQL.Add(whc);
                        }
                    }

                    using (_cycloneEntities = new cycloneEntities())
                    {
                        foreach (webhook wh in webHookListForMYSQL)
                        {
                            _cycloneEntities.webhooks.Add(wh);
                        }
                        _cycloneEntities.SaveChanges();
                        foreach (webhook_commits whc in webHookCommitsForMYSQL)
                        {
                            _cycloneEntities.webhook_commits.Add(whc);
                        }
                        foreach (webhook_files whf in webHookFilesForMYSQL)
                        {
                            _cycloneEntities.webhook_files.Add(whf);
                        }
                        _cycloneEntities.SaveChanges();

                        foreach (webHook webHook in webHookList)
                        {
                            webHook.isSync = true;
                            webHook.TimeSync = DateTime.Now;

                            _onlineDBEntities.webHooks.Attach(webHook);
                            var entry = _onlineDBEntities.Entry(webHook);
                            entry.Property(e => e.isSync).IsModified = true;
                            entry.Property(e => e.TimeSync).IsModified = true;
                        }
                        UpdateModifiedUsers(webHookList, source_id);
                    }
                    _onlineDBEntities.SaveChanges();
                    // dbContextTransaction.Complete();
                }
                catch (Exception ex)
                {

                    // dbContextTransaction.Dispose();
                }
                finally
                {
                    // dbContextTransaction.Dispose();
                }
                //}
            }
        }
        private void UpdateModifiedUsers(List<webHook> webHook, string source_id)
        {
            using (_cycloneEntities = new cycloneEntities())
            {
                if (webHook.Count > 0)
                {
                    string lastAnalysisId = _cycloneEntities.source_analyzer.Where(a => a.source_id == source_id)
                        .OrderByDescending(a => a.processed_time).FirstOrDefault().analyzer_id;

                    List<clone_fragment> cloneFragments = _cycloneEntities.clone_fragment
                                         .Where(a => a.analyzer_id == lastAnalysisId).ToList();
                    foreach (webHook wh in webHook)
                    {
                        foreach (webHookFile whf in wh.webHookFiles)
                        {
                            if (whf.patch == null) continue;
                            String[] changes = whf.patch.Split(new[] { "@@" }, StringSplitOptions.RemoveEmptyEntries);
                            List<String> StartLines = new List<string>();
                            for (int i = 0; i < changes.Length; i++)
                            {
                                if (changes[i].Trim().StartsWith("-"))
                                {
                                    StartLines.Add(changes[i].ToString());
                                }
                            }

                            for (int i = 0; i < cloneFragments.Count(); i++)
                            {
                                if (cloneFragments[i].source_path.ToString().Split('\\').Last() == whf.fileName.Split('/').Last())
                                {
                                    bool changed = false;
                                    foreach (String str in StartLines)
                                    {
                                        int startingLine = Convert.ToInt32(str.Replace("-", "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).First());
                                        if (startingLine >= cloneFragments[i].start_line && startingLine <= cloneFragments[i].end_line)
                                        {
                                            changed = true;
                                        }
                                    }
                                    if (!changed) continue;
                                    clones_modified_users cmu = new clones_modified_users();
                                    cmu.analyzer_id = lastAnalysisId;
                                    cmu.file_name = whf.fileName;
                                    cmu.modified_line_number = Convert.ToInt32(cloneFragments[i].start_line);
                                    cmu.modification_id = Guid.NewGuid().ToString();
                                    cmu.source_id = source_id;
                                    cmu.status = whf.Status;
                                    cmu.timestamp = wh.entryTime;
                                    cmu.user_name = _cycloneEntities.webhook_commits.Where(a => a.webhook_ref_id == wh.webHookRefId).OrderBy(a => a.icommit_id).FirstOrDefault().committer_name;

                                    _cycloneEntities.clones_modified_users.Add(cmu);
                                    _cycloneEntities.SaveChanges();

                                    break;

                                }
                            }
                        }
                    }
                }

            }
        }
        public void AutomatedAnalyse()
        {
            using (_cycloneEntities = new cycloneEntities())
            {
                List<source> sourceList = _cycloneEntities.sources.Where(a => a.time_intervals_id != null && a.time_intervals_id!="").ToList();
                foreach (source source in sourceList)
                {
                    source_analyzer lastAnalyse = _cycloneEntities.source_analyzer.OrderByDescending(a=>a.processed_time)
                        .Where(a => a.source_id == source.source_id).FirstOrDefault();
                    if (lastAnalyse != null)
                    {
                        DateTime lastAnalyseTime = Convert.ToDateTime(lastAnalyse.processed_time);
                        double intervalInHours = Convert.ToDouble(_cycloneEntities.time_intervals.Where(a => a.time_intervals_id == source.time_intervals_id)
                            .FirstOrDefault().hours_value);

                        if (lastAnalyseTime.AddHours(intervalInHours) <= DateTime.Now)
                        {
                            CloneAnalyzer cloneAnalyzer = new CloneAnalyzer();
                            Boolean status = cloneAnalyzer.TimeToTimeAnalyse(source);
                        }
                    }

                }

            }
        }
        private string GetSourceIdByUrl(string url)
        {
            string sourceId;
            using (_cycloneEntities = new cycloneEntities())
            {
                try
                {
                    sourceId = _cycloneEntities.sources.Where(a => a.git_url.Contains(url))
                 .OrderByDescending(a => a.create_date).FirstOrDefault().source_id;
                }
                catch (Exception)
                {

                    sourceId = null;
                }
                return sourceId;
            }
        }
    }
}
