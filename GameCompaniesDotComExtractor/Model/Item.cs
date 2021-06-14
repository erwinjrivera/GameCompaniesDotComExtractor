using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompaniesDotComExtractor.Model
{
    public class Item
    {
        private string _logoUrl;

        public string Name { get; set; }

        public string Description { get; set; }


        public string Website { get; set; }

        [DisplayNameAttribute("Logo URL")]
        public string LogoURL
        {
            get { return @"https://img.gamecompanies.com/" + _logoUrl; }
            set { _logoUrl = value; }
        }


        public string Tag { get; set; }

        [DisplayNameAttribute("Number of Employees")]
        public string NumberOfEmployees { get; set; }

        public string Established { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public string Headquarters { get; set; }

        public string Profile { get; set; }
    }
}
