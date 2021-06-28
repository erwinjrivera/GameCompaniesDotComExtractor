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
            dataGridView1.DataSource = table;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                this.button1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                table = CreateDataTable();
                cbIndustries.Enabled = false;
                button1.Text = "Initializing...";
                lblStatus.Text = "Please wait. Extraction in progress...";
                backgroundWorker1.RunWorkerAsync(cbIndustries.SelectedItem.ToString());
            }
        }

        private System.Data.DataTable CreateDataTable()
        {
            System.Data.DataTable table = new System.Data.DataTable();

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Website", typeof(string));
            table.Columns.Add("LogoURL", typeof(string));
            table.Columns.Add("Tag", typeof(string));
            table.Columns.Add("NumberOfEmployees", typeof(string));
            table.Columns.Add("Established", typeof(string));
            table.Columns.Add("Address", typeof(string));
            table.Columns.Add("Country", typeof(string));
            table.Columns.Add("Region", typeof(string));
            table.Columns.Add("City", typeof(string));
            table.Columns.Add("Headquarters", typeof(string));
            table.Columns.Add("Profile", typeof(string));

            return table;
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
                string filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + cbIndustries.SelectedItem.ToString() + "-output.txt";
                File.WriteAllText(filename, sb.ToString());

                //Notepad.SendText(sb.ToString());

                Process notepad = Process.Start(filename);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        List<Item> items = new List<Item>();
        DataTable table = new DataTable();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";

            DATA = Properties.Settings.Default.Data1;
            DATA = DATA.Replace("%industrySlug%", e.Argument.ToString());

            request.ContentLength = DATA.Length;

            using (Stream webStream = request.GetRequestStream())
            {
                backgroundWorker1.ReportProgress(100, "Sending request to server...");

                using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                {
                    requestWriter.Write(DATA);
                }
            }

            try
            {
                backgroundWorker1.ReportProgress(100, "Awaiting response from the server...");
                Root myDeserializedClass = null;
                WebResponse webResponse = request.GetResponse();
                using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    backgroundWorker1.ReportProgress(100, "Extracting data...");
                    string response = responseReader.ReadToEnd();
                    //Debug.WriteLine(response);

                    
                    myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                }

                items = TestYield(myDeserializedClass.data.result.companies).ToList();

                foreach (var item in items)
                {
                    DataRow row = table.NewRow();

                    row["Name"] = item.Name;
                    row["Description"] = item.Description;
                    row["Profile"] = item.Profile;
                    row["LogoURL"] = item.LogoURL;
                    row["Website"] = item.Website;
                    row["Tag"] = item.Tag;
                    row["NumberOfEmployees"] = item.NumberOfEmployees;
                    row["Established"] = item.Established;
                    row["Address"] = item.Address;
                    row["Country"] = item.Country;
                    row["Region"] = item.Region;
                    row["City"] = item.City;
                    row["Headquarters"] = item.Headquarters;

                    table.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-----------------");
                Debug.WriteLine(ex.Message);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.button1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button1.AutoSize = true;
            button1.Text = e.UserState.ToString();
            
            button1.Invalidate();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = table;

            if (dataGridView1.Rows.Count > 0)
                dataGridView1.Columns["Description"].Width = 400;

            cbIndustries.Enabled = true;
            lblStatus.Text = string.Empty;

            this.button1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            button1.Text = "Start";
            button1.AutoSize = true;
            
            button1.Invalidate();

        }
    }
}
