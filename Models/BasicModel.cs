using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CianPlatform.Models
{
    public class BasicModel
    {
        public readonly string BasicUrlProject;
        public readonly string BasicUrl;
        public readonly string BasicExcelDownload;
        public readonly string BasicApiOffers;

        public BasicModel(string basicUrlProject, string basicUrl, string basicExcelDownload, string basicApiOffers)
        {
            BasicUrlProject = basicUrlProject;
            BasicUrl = basicUrl;
            BasicExcelDownload = basicExcelDownload;
            BasicApiOffers = basicApiOffers;
        }
    }

    #region Api offers request json
    public class ApiOffers
    {
        public ApiOffers(int project, string mainType = "", string engineType = "", string pageType = "", string geoTypeValue = "")
        {
            string _mainType = (string.IsNullOrEmpty(mainType)) ? "flatsale" : mainType;
            string _engineType = (string.IsNullOrEmpty(engineType)) ? "term" : engineType;
            string _pageType = (string.IsNullOrEmpty(pageType)) ? "term" : pageType;
            string _geoTypeValue = (string.IsNullOrEmpty(geoTypeValue)) ? "newobject" : geoTypeValue;

            JsonQuery = new JsonQuery();
            JsonQuery.Type = _mainType;
            JsonQuery.Geo = new JsonGeoQuery(new List<JsonValueQuery>() { new JsonValueQuery(_geoTypeValue, 0, project) });
            JsonQuery.EngineVersion = new JsonValueQuery(_engineType, 2, 0);
            JsonQuery.Page = new JsonValueQuery(_pageType, 1, 0);
        }

        [JsonProperty("jsonQuery")]
        public JsonQuery JsonQuery { get; set; }
    }

    public class JsonQuery
    {
        [JsonProperty("_type")]
        public string Type { get; set; }
        [JsonProperty("geo")]
        public JsonGeoQuery Geo { get; set; }
        [JsonProperty("engine_version")]
        public JsonValueQuery EngineVersion { get; set; }
        [JsonProperty("page")]
        public JsonValueQuery Page { get; set; }
    }

    public class JsonValueQuery
    {
        public JsonValueQuery(string type, int value, int id)
        {
            Type = type;
            Value = value;
            Id = id;
        }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("value")]
        public int Value { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class JsonGeoQuery
    {
        public JsonGeoQuery(List<JsonValueQuery> value)
        {
            Type = "geo";
            Value = value;
        }

        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("value")]
        public List<JsonValueQuery> Value { get; set; }
    }

    #endregion

    #region Api offers response json
    public class ApiOffersResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public ApiOffersResponseData Data { get; set; }
    }

    public class ApiOffersResponseData
    {
        [JsonProperty("offersSerialized")]
        public OffersSerializedResponse[] SerializedResponse { get; set; }
    }

    public class OffersSerializedResponse
    {
        [JsonProperty("cianId")]
        public int CianId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }
        [JsonProperty("kitchenArea")]
        public double? KitchenArea { get; set; }
        [JsonProperty("roomArea")]
        public double? RoomArea { get; set; }
        [JsonProperty("totalArea")]
        public double? TotalArea { get; set; }
        [JsonProperty("fullUrl")]
        public string FlatUrl { get; set; }
        [JsonProperty("phones")]
        public PhonesList[] Phones { get; set; }
        [JsonProperty("user")]
        public UserOffer User { get; set; }
        [JsonProperty("bargainTerms")]
        public BargainTerms BargainTerms { get; set; }
        [JsonProperty("geo")]
        public GeoPosition GeoPosition { get; set; }
    }

    public class GeoPosition
    {
        [JsonProperty("userInput")]
        public string Address { get; set; }
        [JsonProperty("undergrounds")]
        public Undergrounds[] Undergrounds { get; set; }
    }

    public class Undergrounds
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    } 

    public class UserOffer
    {
        [JsonProperty("cianUserId")]
        public string CianUserId { get; set; }
        [JsonProperty("agencyName")]
        public string AgencyName { get; set; }
        [JsonProperty("accountType")]
        public string AccountType { get; set; }
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
        [JsonProperty("phoneNumbers")]
        public PhonesList[] Phones { get; set; }
        [JsonProperty("isCianPartner")]
        public bool IsCianPartner { get; set; }
    }

    public class BargainTerms
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }
    }

    public class PhonesList
    {
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("number")]
        public string Number { get; set; }
    }
    #endregion

    #region OData Response model
    public class ODataSelectCianRecord
    {
        [JsonProperty("value")]
        public ODataSelectCianRecordValue[] Value { get; set; }
    }

    public class ODataSelectCianRecordValue
    {
        [JsonProperty("_adcianid")]
        public string AdCian { get; set; }
        [JsonProperty("_flaturl")]
        public string FlatUrl { get; set; }
    }
    #endregion
}
