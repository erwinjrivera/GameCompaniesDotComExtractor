using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using GameCompaniesDotComExtractor.Model;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using GameCompaniesDotComExtractor.Utilities;

namespace GameCompaniesDotComExtractor
{
    public partial class Form1 : Form
    {
        private string URL = "https://api.devkittens.com/gamecompanies/graphql";
        private string DATA = Properties.Settings.Default.Data1;

        public Form1()
        {
            InitializeComponent();

            cbIndustries.SelectedIndex = 0;
            lblStatus.Text = string.Empty;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " (v" + Assembly.GetEntryAssembly().GetName().Version.ToString() + ")";

            lblDownload.Visible = false;
            dataGridView1.DataSource = items;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                cbIndustries.Enabled = false;
                lblStatus.Text = "Processing...";
                backgroundWorker1.RunWorkerAsync(cbIndustries.SelectedItem.ToString());
            }
        }

        private void CreateObject(string industry)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";

            DATA = Properties.Settings.Default.Data1;
            DATA = DATA.Replace("%industrySlug%", industry);

            request.ContentLength = DATA.Length;

            using (Stream webStream = request.GetRequestStream())
            using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
            {
                requestWriter.Write(DATA);
            }

            try
            {
                Root myDeserializedClass = null;
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    //Debug.WriteLine(response);

                    myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                }

                items = TestYield(myDeserializedClass.data.result.companies).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine("-----------------");
                Debug.WriteLine(e.Message);
            }
        }

        public IEnumerable<Item> TestYield(List<Company> companies)
        {
            //for (int i = 0; i < 3; i++) yield return 4;
            foreach (var company in companies)
            {
                Item item = new Item
                {
                    Name = company.name,
                    Description = company.@short,
                    Profile = company.description,
                    LogoURL = company.picRelUrl,
                    Website = company.website,
                    Tag = company.firstTag != null ? company.firstTag.name : null,
                    NumberOfEmployees = company.nrOfEmployees,
                    Established = company.established,
                    Address = company.locations != null ? company.locations[0].formattedAddress : "",
                    Country = company.firstLocation != null ? company.firstLocation.countryDoc != null ? company.firstLocation.countryDoc.name : "" : "",
                    Region = company.firstLocation != null ? company.firstLocation.regionDoc != null ? company.firstLocation.regionDoc.name : "" : "",
                    City = company.firstLocation != null ? company.firstLocation.cityDoc != null ? company.firstLocation.cityDoc.name : "" : "",
                    Headquarters = company.headquarters != null ? company.headquarters.fullAddress : ""
                };

                yield return item;
            }
        }

        private void dataGridView1_DataSourceChanged(object sender, EventArgs e)
        {
            lblDownload.Visible = dataGridView1.Rows.Count > 0;

            lblDownload.Text = "Download result (" + string.Format("{0:#,0}", dataGridView1.Rows.Count) + ")";
        }

        private void lblDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                sb.Append(col.Name);
                sb.Append("\t");
            }


            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                sb.Append(Environment.NewLine);

                foreach (DataGridViewCell c in row.Cells)
                {
                    sb.Append(c.Value != null ? c.Value.ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "") : string.Empty);
                    sb.Append("\t");
                }
            }


            try
            {
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cbIndustries.SelectedItem.ToString() + "-output.txt", sb.ToString());

                Notepad.SendText(sb.ToString());
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        List<Item> items = new List<Item>();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            CreateObject(e.Argument.ToString());
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = items;

            cbIndustries.Enabled = true;
            lblStatus.Text = string.Empty;
        }
    }
}
