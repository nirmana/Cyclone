using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeCloneAnalysis_BL;
using CodeCloneAnalysis_App;
using System.Threading;
using System.Net;
using System.Configuration;
using System.IO.Compression;
using CodeCloneAnalysis_DB;

namespace CodeCloneAnalysis_StandAlone
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        CloneAnalyzer _cloneAnalyzer;
        Waiting waitingForm;
        DataTable tblItems;
        

        public Form1()
        {          
            InitializeComponent();
            tblItems = new DataTable();
            tblItems.Columns.Add("Source ID");
            tblItems.Columns.Add("Source Name");
            tblItems.Columns.Add("Source Location");
            tblItems.Columns.Add("Last Analysed Time");
            tblItems.Columns.Add("Line of Code in Last Analysis");
            tblItems.Columns.Add("Clone Count in Last Analysis");
            //tblItems.Columns.Add("Analysis History");
            LoadTimeIntervalsDB();
            getAllItems();
        }


        private void LoadTimeIntervalsDB()
        {

            using (var _cycloneEntities = new cycloneEntities())
            {
                cmbInterval.DataSource = _cycloneEntities.time_intervals
                    .Select(a => new
                    {
                        a.time_intervals_id,
                        a.display_value

                    }).ToList();
                cmbInterval.ValueMember = "time_intervals_id";
                cmbInterval.DisplayMember = "display_value";
            }
        }


        private void getAllItems()
        {
            OldAnalysisGrid.DataSource = tblItems;
            _cloneAnalyzer = new CloneAnalyzer("");
            List<source> sourceList = _cloneAnalyzer.GetAllOldAnalyzeDetails();
            cycloneEntities _cycloneEntities = new cycloneEntities();
            foreach (source i in sourceList)
            {
                DataRow dr = tblItems.NewRow();
                dr["Source ID"] = i.source_id;
                dr["Source Name"] = i.source_name;
                dr["Source Location"] = i.location;
                dr["Last Analysed Time"] = i.create_date;
                dr["Line of Code in Last Analysis"] = _cycloneEntities.source_analyzer
                    .Where(a => a.source_id == i.source_id)
                    .OrderByDescending(a => a.processed_time).FirstOrDefault().line_of_code_count;
                dr["Clone Count in Last Analysis"] = _cycloneEntities.source_analyzer
                    .Where(a => a.source_id == i.source_id)
                    .OrderByDescending(a => a.processed_time).FirstOrDefault().clones_count;
                DataGridViewButtonColumn visualizeButtonColumn = new DataGridViewButtonColumn();
                visualizeButtonColumn.Name = "Analysis History";
                visualizeButtonColumn.Text = "View";
                visualizeButtonColumn.UseColumnTextForButtonValue = true;
                int columnIndex = 6;
                if (OldAnalysisGrid.Columns["Analysis History"] == null)
                {
                    OldAnalysisGrid.Columns.Insert(columnIndex, visualizeButtonColumn);
                }           
                tblItems.Rows.Add(dr);
            }
            OldAnalysisGrid.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == OldAnalysisGrid.Columns["Analysis History"].Index)
            {
                try
                {
                    object value = OldAnalysisGrid.Rows[e.RowIndex].Cells[1].Value;
                    Form2 form = new Form2(value.ToString());
                    form.Show();
                }
                catch (Exception)
                {
                }
           
            }
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            disableCkeckboxes();
           // Waiting waitingForm = new Waiting();
        }


        private void btnBrowseLocalSource_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            txtLocalSourceUrl.Text = fbd.SelectedPath;
            EnableCkeckboxes();
            txtGitSourceUrl.Text = "";
            //MessageBox.Show(fbd.SelectedPath);
        }
        private void disableCkeckboxes()
        {
            chkOneTimeAnalyse.Enabled = false;
            chkTimeToTimeAnalyze.Enabled = false;
            cmbInterval.Enabled = false;
        }
        private void EnableCkeckboxes()
        {
            chkOneTimeAnalyse.Enabled = true;
            chkTimeToTimeAnalyze.Enabled = true;
            cmbInterval.Enabled = true;
        }

        private void txtLocalSourceUrl_Enter(object sender, EventArgs e)
        {
            txtGitSourceUrl.Text = "";
        }

        private void txtGitSourceUrl_Enter(object sender, EventArgs e)
        {
            txtLocalSourceUrl.Text = "";
        }

        private void btnFetchFromGit_Click(object sender, EventArgs e)
        {
            txtLocalSourceUrl.Text = "";
        }

        private void chkOneTimeAnalyse_CheckStateChanged(object sender, EventArgs e)
        {
            chkTimeToTimeAnalyze.Checked = !chkOneTimeAnalyse.Checked;
        }

        private void chkTimeToTimeAnalyze_CheckedChanged(object sender, EventArgs e)
        {
            chkOneTimeAnalyse.Checked = !chkTimeToTimeAnalyze.Checked;
        }

        private void btnAnalyzeCode_Click(object sender, EventArgs e)
        {
            try
            {
                String LocalFilePath = txtLocalSourceUrl.Text;
                String GitHubUrl = txtGitSourceUrl.Text;
                String ProjectName = txtProjectName.Text;
               
                if (String.IsNullOrEmpty(LocalFilePath) && String.IsNullOrEmpty(GitHubUrl))
                {
                    MessageBox.Show("Please specify either Local File Path or GIT Url");
                }
                else
                {
                    if (!String.IsNullOrEmpty(ProjectName))
                    {
                        Boolean IsOneTime = chkOneTimeAnalyse.Checked;
                        Boolean IsTimeToTime = chkTimeToTimeAnalyze.Checked;
                        if (IsOneTime || IsTimeToTime)
                        {
                            if (!String.IsNullOrEmpty(LocalFilePath))
                            {
                                if (Directory.Exists(LocalFilePath) && Directory.EnumerateFiles(LocalFilePath).Count() > 0)
                                {
                                    //setup cqr and cqb and update db
                                    
                                    _cloneAnalyzer = new CloneAnalyzer(ProjectName);
                                    var TimeInterval = cmbInterval.SelectedValue;
                                    if(IsOneTime){
                                        TimeInterval = "";
                                    }
                                    Waiting waitingForm = new Waiting();
                                    waitingForm.Show();
                                    string analyzeId = "";
                                    var status = _cloneAnalyzer.ProcessLocalFilePath(LocalFilePath, ProjectName, GitHubUrl, TimeInterval.ToString(), out analyzeId);
                                    waitingForm.Hide();
                                    waitingForm.Dispose();
                                    if (analyzeId != "")
                                    {
                                        Form3 frmJustResult = new Form3(analyzeId);
                                        frmJustResult.Show();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Specified directory not exist or empty");
                                }
                            }
                            else if (!String.IsNullOrEmpty(GitHubUrl))
                            {
                                //git hub implementation
                                if (GitHubUrl.Contains(".git"))
                                {
                                    //assume that the master branch always as /archive/master.zip
                                    Waiting waitingForm = new Waiting();
                                    waitingForm.Show();
                                    Application.DoEvents();
                                    var GitHubUrlforMain=GitHubUrl.Replace(".git", "/archive/master.zip");
                                    string url = @GitHubUrlforMain;
                                    WebClient client = new WebClient();
                                   //client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                                    var workingDirectory = ConfigurationSettings.AppSettings["DefaultSourceLocation"];
                                    CloneAnalyzer ca = new CloneAnalyzer("");
                                    string analyzeId = "";
                                    if (ca.CreateBaseLocationForSource())
                                    {
                                        String uniqueProjectRef = Guid.NewGuid().ToString();
                                        if (!Directory.Exists(workingDirectory + "\\GitSources\\" + uniqueProjectRef))
                                        {
                                            Directory.CreateDirectory(workingDirectory + "\\GitSources\\" + uniqueProjectRef);
                                        }

                                        client.DownloadFile(new Uri(url), @workingDirectory + "\\GitSources\\" + uniqueProjectRef + "\\" + uniqueProjectRef+".zip");
                                        ZipFile.ExtractToDirectory(@workingDirectory + "\\GitSources\\" + uniqueProjectRef + "\\" + uniqueProjectRef + ".zip", @workingDirectory + "\\GitSources\\" + uniqueProjectRef + "\\");
                                        ca = new CloneAnalyzer("");
                                        var TimeInterval = cmbInterval.SelectedValue;
                                        if (IsOneTime)
                                        {
                                            TimeInterval = "";
                                        }
                                        ca.ProcessLocalFilePath(@workingDirectory + "\\GitSources\\" + uniqueProjectRef, ProjectName, GitHubUrl, TimeInterval.ToString(),out analyzeId );
                                    }
                                    waitingForm.Hide();
                                    waitingForm.Dispose();
                                    if (analyzeId != "")
                                    {
                                        Form3 frmJustResult = new Form3(analyzeId);
                                        frmJustResult.Show();
                                    }
                                    txtGitSourceUrl.Text = "";
                                    txtLocalSourceUrl.Text = "";
                                    txtProjectName.Text = "";
                                    chkOneTimeAnalyse.Checked = false;
                                    chkTimeToTimeAnalyze.Checked = false;
                                    chkOneTimeAnalyse.Enabled = false;
                                    chkTimeToTimeAnalyze.Enabled = false;
                                }
                                else
                                {
                                    MessageBox.Show("Seems like provided GIT Url is invalid.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Please specify either Local File Path or GIT Url");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please specify a analysis method");

                        }
                    }
                    else
                    {
                        MessageBox.Show("Please specify a Project Name");

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("File downloaded");
        }
        
       
        private void LoadOldSourceDetails()
        {
            //_cloneAnalyzer = new CloneAnalyzer();
           // _cloneAnalyzer.GetAllOldAnalyzeDetails();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void txtProjectName_TextChanged(object sender, EventArgs e)
        {
         
        }

        private void cmbInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
             
        }

        private void btnResetForm_Click(object sender, EventArgs e)
        {
            txtGitSourceUrl.Text = "";
            txtLocalSourceUrl.Text = "";
            txtProjectName.Text = "";
            chkOneTimeAnalyse.Checked = false;
            chkTimeToTimeAnalyze.Checked = false;
            chkOneTimeAnalyse.Enabled = false;
            chkTimeToTimeAnalyze.Enabled = false;

        }

        private void txtGitSourceUrl_TextChanged(object sender, EventArgs e)
        {
            EnableCkeckboxes();
            txtLocalSourceUrl.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

       

    }
}
