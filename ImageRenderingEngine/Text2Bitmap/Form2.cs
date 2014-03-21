using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;

namespace Text2Bitmap
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ClsConnection objConnection = new ClsConnection();
            comboBox1.DataSource = objConnection.GetLabelTemplates();
            comboBox1.DisplayMember = "TemplateName";
            comboBox1.ValueMember = "ID";
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            if (cmbAction.Items.Count > 0)
            {
                cmbAction.SelectedIndex = 0;
            }

            cmbAsset.DataSource = objConnection.GetAsset();
            cmbAsset.DisplayMember = "SummaryName";
            cmbAsset.ValueMember = "ID";
            if (cmbAsset.Items.Count > 0)
            {
                cmbAsset.SelectedIndex = 0;
            }
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private void cmbAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAction.Text.Contains("Asset"))
            {
                label3.Visible = true;
                cmbAsset.Visible = true;
            }
            else
            {
                label3.Visible = false;
                cmbAsset.Visible = false;
                if (cmbAsset.Items.Count > 0)
                {
                    cmbAsset.SelectedIndex = 0;
                }
            }

            if (cmbAction.Text.Contains("Param"))
            {
                panel1.Visible = true;
                pictureBox_BitmapPreview.Visible = false;
            }
            else
            {
                panel1.Visible = false;
                pictureBox_BitmapPreview.Visible = true;
                pictureBox_BitmapPreview.Image = null;
                pictureBox_BitmapPreview.Refresh();
            }
        }

        public void Execute()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:30526/");
            string actionName = string.Empty;
            if (cmbAction.SelectedIndex == 0 || cmbAction.SelectedIndex == 1)
            {
                actionName = "GenerateLabelPreview";
            }
            else if (cmbAction.SelectedIndex == 2)
            {
                actionName = "GenerateLabelPreviewTextOnly";
            }
            else if (cmbAction.SelectedIndex == 3)
            {
                actionName = "GetLabelParameters";
            }

            int labelTemplateID = Convert.ToInt32(comboBox1.SelectedValue);
            int? assetID = null;
            if (cmbAsset.Visible)
            {
                assetID = Convert.ToInt32(cmbAsset.SelectedValue);
            }
            string url = string.Format("api/customimage/{0}/{1}/{2}", actionName, labelTemplateID, assetID);

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                if (pictureBox_BitmapPreview.Visible)
                {
                    pictureBox_BitmapPreview.Image = byteArrayToImage(response.Content.ReadAsByteArrayAsync().Result);
                }
                else
                {
                    var dict = response.Content.ReadAsAsync<Dictionary<string, string>>().Result;
                    listBox1.Items.Clear();                   
                    foreach (KeyValuePair<string, string> pair in dict)
                    {
                        listBox1.Items.Add(pair.Value + "\t\t\t" + pair.Key);
                    }
                }
            }
            else
            {
                MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

    }
}
