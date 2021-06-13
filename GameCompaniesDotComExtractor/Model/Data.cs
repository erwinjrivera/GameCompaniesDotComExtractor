using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCompaniesDotComExtractor.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Tag
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string __typename { get; set; }
    }

    public class Location
    {
        public string _id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string formattedAddress { get; set; }
        public string __typename { get; set; }
    }

    public class CountryDoc
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string adjective { get; set; }
        public string __typename { get; set; }
    }

    public class RegionDoc
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string __typename { get; set; }
    }

    public class CityDoc
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string __typename { get; set; }
    }

    public class Headquarters
    {
        public string _id { get; set; }
        public string fullAddress { get; set; }
        public CountryDoc countryDoc { get; set; }
        public RegionDoc regionDoc { get; set; }
        public CityDoc cityDoc { get; set; }
        public string __typename { get; set; }
    }

    public class FirstTag
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string __typename { get; set; }
    }

    public class FirstLocation
    {
        public string _id { get; set; }
        public CountryDoc countryDoc { get; set; }
        public RegionDoc regionDoc { get; set; }
        public CityDoc cityDoc { get; set; }
        public string __typename { get; set; }
    }

    public class Company
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string @short { get; set; }
        public string description { get; set; }
        public string picRelUrl { get; set; }
        public string coverRelUrl { get; set; }
        public string website { get; set; }
        public string jobWebsite { get; set; }
        public List<object> imgs { get; set; }
        public List<Tag> tags { get; set; }
        public string nrOfEmployees { get; set; }
        public string established { get; set; }
        public List<Location> locations { get; set; }
        public Headquarters headquarters { get; set; }
        public FirstTag firstTag { get; set; }
        public FirstLocation firstLocation { get; set; }
        public string __typename { get; set; }
    }

    public class Result
    {
        public int total { get; set; }
        public string industrySlug { get; set; }
        public List<Company> companies { get; set; }
        public string __typename { get; set; }
    }

    public class Data
    {
        public Result result { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    }

}
