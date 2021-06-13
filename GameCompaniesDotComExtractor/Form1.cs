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

namespace GameCompaniesDotComExtractor
{
    public partial class Form1 : Form
    {
        private string URL = "https://api.devkittens.com/gamecompanies/graphql";
        private string DATA = Properties.Settings.Default.Data1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateObject();
        }

        private void CreateObject()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.ContentType = "application/json";

            DATA = Properties.Settings.Default.Data1;
            DATA = DATA.Replace("%industrySlug%", cbIndustries.SelectedItem.ToString());

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
                    Debug.WriteLine(response);

                    myDeserializedClass = JsonConvert.DeserializeObject<Root>(response);
                }

                dataGridView1.DataSource = TestYield(myDeserializedClass.data.result.companies).ToList();
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
    }
}
