using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeCloneAnalysis_BL;
using CodeCloneAnalysis_App;
using CodeCloneAnalysis_DB;

namespace CodeCloneAnalysis_App
{
    public partial class Form3 : MetroFramework.Forms.MetroForm
    {
        DataTable dataTable;
        CloneAnalyzer _cloneAnalyzer;
        String _sourceAnalyzeId;
        String _sourceId;

        public Form3(String sourceAnalyzeId)
        {
            InitializeComponent();
            this._sourceAnalyzeId = sourceAnalyzeId;
            dataTable = new DataTable();
            dataTable.Columns.Add("Source ID");
            dataTable.Columns.Add("Analyzer ID");
            dataTable.Columns.Add("Processed Time");
            dataTable.Columns.Add("Source File Count");
            dataTable.Columns.Add("Line of Code Count");
            dataTable.Columns.Add("Clone Class Count");
            dataTable.Columns.Add("Clones Count");
           
            getAnalysisResult();
        }
        private void getAnalysisResult()
        {
            dataGridView1.DataSource = dataTable;
            _cloneAnalyzer = new CloneAnalyzer("");
            source_analyzer anlyzer = _cloneAnalyzer.GetAnalyzeDetailsByID(_sourceAnalyzeId);
            cycloneEntities _cycloneEntities = new cycloneEntities();
            if (anlyzer != null)
            {
                DataRow dr = dataTable.NewRow();
                dataGridView1.AllowUserToAddRows = false;
                dr["Source ID"] = anlyzer.source_id;
                _sourceId = anlyzer.source_id;
                dr["Analyzer ID"] = _sourceAnalyzeId;
                dr["Processed Time"] = anlyzer.processed_time;
                dr["Source File Count"] = anlyzer.source_file_count;
                dr["Line of Code Count"] = anlyzer.line_of_code_count;
                dr["Clone Class Count"] = anlyzer.clone_classes_count;
                dr["Clones Count"] = anlyzer.clones_count;
                dataTable.Rows.Add(dr);
            }
            
            dataGridView1.Refresh();
        }

        private void btnVisualize_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:8080/Visclone/WebContent/pages/index.php?analyzisid=" + _sourceAnalyzeId + "&sourceid=" + _sourceId);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
