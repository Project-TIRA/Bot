using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SearchBot.Bot.Dialogs.Service
{
    
    [DataContract]
    public class Summary
    {
        [DataMember]
        public string query { get; set; }

        [DataMember]
        public string queryType { get; set; }

        [DataMember]
        public int queryTime { get; set; }

        [DataMember]
        public int numResults { get; set; }

        [DataMember]
        public int offset { get; set; }

        [DataMember]
        public int totalResults { get; set; }

        [DataMember]
        public int fuzzyLevel { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember]
        public string municipality { get; set; }

        [DataMember]
        public string countrySecondarySubdivision { get; set; }

        [DataMember]
        public string countryTertiarySubdivision { get; set; }

        [DataMember]
        public string countrySubdivision { get; set; }

        [DataMember]
        public string countryCode { get; set; }

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public string countryCodeISO3 { get; set; }

        [DataMember]
        public string freeformAddress { get; set; }

        [DataMember]
        public string countrySubdivisionName { get; set; }

        [DataMember]
        public string streetNumber { get; set; }

        [DataMember]
        public string streetName { get; set; }

        [DataMember]
        public string municipalitySubdivision { get; set; }

        [DataMember]
        public string postalCode { get; set; }

        [DataMember]
        public string extendedPostalCode { get; set; }

        [DataMember]
        public string localName { get; set; }
    }

    public class Position
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class TopLeftPoint
    {
        [DataMember]
        public double lat { get; set; }
        
        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class BtmRightPoint
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class Viewport
    {
        [DataMember]
        public TopLeftPoint topLeftPoint { get; set; }
        
        [DataMember]
        public BtmRightPoint btmRightPoint { get; set; }
    }

    [DataContract]
    public class TopLeftPoint2
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class BtmRightPoint2
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class BoundingBox
    {
        [DataMember]
        public TopLeftPoint2 topLeftPoint { get; set; }

        [DataMember]
        public BtmRightPoint2 btmRightPoint { get; set; }
    }

    [DataContract]
    public class Geometry
    {
        [DataMember]
        public string id { get; set; }
    }

    [DataContract]
    public class DataSources
    {
        [DataMember]
        public Geometry geometry { get; set; }
    }

    [DataContract]
    public class CategorySet
    {
        [DataMember]
        public int id { get; set; }
    }

    [DataContract]
    public class Name
    {
        [DataMember]
        public string nameLocale { get; set; }
        
        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class Classification
    {
        [DataMember]
        public string code { get; set; }

        [DataMember]
        public List<Name> names { get; set; }
    }

    [DataContract]
    public class Brand
    {
        [DataMember]
        public string name { get; set; }
    }

    [DataContract]
    public class Poi
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string phone { get; set; }

        [DataMember]
        public List<CategorySet> categorySet { get; set; }

        [DataMember]
        public List<string> categories { get; set; }

        [DataMember]
        public List<Classification> classifications { get; set; }

        [DataMember]
        public List<Brand> brands { get; set; }

        [DataMember]
        public string url { get; set; }
    }

    [DataContract]
    public class Position2
    {
        [DataMember]
        public double lat { get; set; }

        [DataMember]
        public double lon { get; set; }
    }

    [DataContract]
    public class EntryPoint
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public Position2 position { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string id { get; set; }

        [DataMember]
        public double score { get; set; }

        [DataMember]
        public string entityType { get; set; }

        [DataMember]
        public Address address { get; set; }

        [DataMember]
        public Position position { get; set; }

        [DataMember]
        public Viewport viewport { get; set; }

        [DataMember]
        public BoundingBox boundingBox { get; set; }

        [DataMember]
        public DataSources dataSources { get; set; }

        [DataMember]
        public string info { get; set; }

        [DataMember]
        public Poi poi { get; set; }

        [DataMember]
        public List<EntryPoint> entryPoints { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public Summary summary { get; set; }

        [DataMember]
        public List<Result> results { get; set; }
    }
}
