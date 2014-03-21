using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Data.SqlClient;

namespace Text2Bitmap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClsConnection objConnection = new ClsConnection();
            comboBox1.DataSource = objConnection.GetLabelTemplates();
            comboBox1.DisplayMember = "TemplateName";
            comboBox1.ValueMember = "ID";
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            //if (cmbAction.Items.Count > 0)
            //{
            //    cmbAction.SelectedIndex = 0;
            //}
        }

        private void cmbAction_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }

    public class ClsConnection
    {
        private SqlConnection con = null;
        private SqlCommand com = null;

        public ClsConnection()
        {
            con = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"]);
        }

        public List<LabelTemplate> GetLabelTemplates()
        {
            com = new SqlCommand("SELECT ID,TemplateName FROM LabelTemplate WHERE FK_ID_LabelTemplateType = 2 ORDER BY TemplateName", con);
            con.Open();
            SqlDataReader reader = com.ExecuteReader();
            List<LabelTemplate> lstLabelTemplate = new List<LabelTemplate>();

            while (reader.Read())
            {
                lstLabelTemplate.Add(new LabelTemplate() { ID = reader.GetInt32(0), TemplateName = reader.GetString(1) });
            }
            con.Close();
            return lstLabelTemplate;
        }

        public List<Asset> GetAsset()
        {
            com = new SqlCommand("SELECT ID,SummaryName FROM Asset ORDER BY SummaryName", con);
            con.Open();
            SqlDataReader reader = com.ExecuteReader();
            List<Asset> lstAsset = new List<Asset>();

            while (reader.Read())
            {
                lstAsset.Add(new Asset() { ID = reader.GetInt32(0), SummaryName = reader.GetString(1) });
            }
            con.Close();
            return lstAsset;
        }

        public struct LabelTemplate
        {
            public int ID { get; set; }
            public string TemplateName { get; set; }
        }

        public struct Asset
        {
            public int ID { get; set; }
            public string SummaryName { get; set; }
        }
    }
}
