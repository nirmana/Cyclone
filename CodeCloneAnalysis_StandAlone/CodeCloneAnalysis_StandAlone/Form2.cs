using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeCloneAnalysis_BL;
using CodeCloneAnalysis_App;
using CodeCloneAnalysis_DB;

namespace CodeCloneAnalysis_App
{
    public partial class Form2 : MetroFramework.Forms.MetroForm
    {
        DataTable dataTable;
        CloneAnalyzer _cloneAnalyzer;
        String sourceId;
        public Form2(String sourceID)
        {
            InitializeComponent();
            this.sourceId = sourceID;
            dataTable = new DataTable();
            dataTable.Columns.Add("Source ID");
            dataTable.Columns.Add("Analyzer ID");
            dataTable.Columns.Add("Processed Time");
            dataTable.Columns.Add("Source File Count");
            dataTable.Columns.Add("Line of Code Count");
            dataTable.Columns.Add("Clone Class Count");
            dataTable.Columns.Add("Clones Count");           
            getPreviousData();
        }
        private void getPreviousData()
        {
            dataGridView.DataSource = dataTable;
            _cloneAnalyzer = new CloneAnalyzer("");
            List<source_analyzer> anlyzerList = _cloneAnalyzer.GetAllDetailsByID(sourceId);
            cycloneEntities _cycloneEntities = new cycloneEntities();
            foreach (source_analyzer i in anlyzerList)
            {
                DataRow dr = dataTable.NewRow();
                dataGridView.AllowUserToAddRows = false;
                dr["Source ID"] = sourceId;
                dr["Analyzer ID"] = i.analyzer_id.PadLeft(4, '0');
                dr["Processed Time"] = i.processed_time;
                dr["Source File Count"] = i.source_file_count;
                dr["Line of Code Count"] = i.line_of_code_count;
                dr["Clone Class Count"] = i.clone_classes_count;
                dr["Clones Count"] = i.clones_count;               
                DataGridViewButtonColumn visualizeButtonColumn = new DataGridViewButtonColumn();
                visualizeButtonColumn.Name = "Visualize Statistics";
                visualizeButtonColumn.Text = "Visualize";
                visualizeButtonColumn.UseColumnTextForButtonValue = true;
                int columnIndex = 7;
                if (dataGridView.Columns["Visualize Statistics"] == null)
                {
                    dataGridView.Columns.Insert(columnIndex,visualizeButtonColumn);
                }
               
                dataTable.Rows.Add(dr);
            }
            dataGridView.Refresh();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView.Columns["Visualize Statistics"].Index)
            {
                try
                {
                    object analysisID = dataGridView.Rows[e.RowIndex].Cells[2].Value;
                    object sourceID = dataGridView.Rows[e.RowIndex].Cells[1].Value;
                    System.Diagnostics.Process.Start("http://localhost:8080/Visclone/WebContent/pages/index.php?analyzisid=" + analysisID + "&sourceid=" + sourceID);
                }
                catch (Exception)
                {
                }

            }
        }
    }
}
