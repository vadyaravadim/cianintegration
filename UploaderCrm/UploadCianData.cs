using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ODataLayerCrm.AuthorizeSpace;
using Flurl.Http;
using CianPlatform.Models;

namespace CianLayerProcessing.UploaderCrm
{
    public class UploaderCianData
    {
        Authorize authorize;
        TokenResponse token;
        string _odata;
        public UploaderCianData(string resource, string login, string password, string odata)
        {
            _odata = odata;
            authorize = new Authorize(resource, login, password);
            Task taskAuth = AuthorizeStart();
        }

        async Task AuthorizeStart()
        {
            token = await authorize.GetTokenRequestAsync();
            Console.WriteLine("SUCCESSFUL AUTHORIZE CRM");
        }

        private async Task<T> SelectAsync<T>(CianData data)
        {
            return await $"{_odata}?$select=cianid,flaturl&$filter=flaturl eq '{data.FlatUrl}'".WithOAuthBearerToken(token.Content).GetJsonAsync<T>().ConfigureAwait(false);
        }

        private async Task<IFlurlResponse> UploadAsync(CianData data)
        {
            return await _odata.WithOAuthBearerToken(token.Content).PostJsonAsync(data).ConfigureAwait(false);
        }
        private async Task<IFlurlResponse> DeleteAsync(Guid recordId)
        {
            return await $"{_odata}({recordId})".WithOAuthBearerToken(token.Content).DeleteAsync().ConfigureAwait(false);
        }

        public async Task DeleteAndUploadAsync(CianData data)
        {
            ODataSelectCianRecord taskDelete = await Delete(data);
            IFlurlResponse taskUpload = await Upload(data);

            async Task<IFlurlResponse> Upload(CianData data)
            {
                return await UploadAsync(data).ConfigureAwait(false);
            }

            async Task<ODataSelectCianRecord> Delete(CianData data)
            {
                ODataSelectCianRecord responseSelect = await SelectAsync<ODataSelectCianRecord>(data).ConfigureAwait(false);
                foreach (ODataSelectCianRecordValue itemRecord in responseSelect.Value)
                {
                    IFlurlResponse responseDelete = await DeleteAsync(new Guid(itemRecord.AdCian)).ConfigureAwait(false);
                }

                return responseSelect;
            }
        }
    }

    public class CianData
    {
        public CianData(OffersSerializedResponse offersSerialized, string building)
        {
            Address = offersSerialized.GeoPosition.Address;
            Author = offersSerialized.User.AgencyName;
            Building = building;
            Company = offersSerialized.User.CompanyName;
            CreationDate = offersSerialized.CreationDate;
            FlatUrl = offersSerialized.FlatUrl;
            Name = offersSerialized.Title;

            if (offersSerialized.User.Phones != null)
            {
                PhonesAgent = string.Join(";", offersSerialized.User.Phones.Select(item => item.Number));
            }

            if (offersSerialized.Phones != null)
            {
                PhonesFlat = string.Join(";", offersSerialized.Phones.Select(item => item.Number));
            }

            Price = offersSerialized.BargainTerms.Price;
            TotalArea = Convert.ToInt32(offersSerialized.TotalArea);
        }

        private string _building { get; set; } 

        [JsonProperty("_address")]
        public string Address { get; set; }
        [JsonProperty("_author")]
        public string Author { get; set; }
        [JsonProperty("buildingid@odata.bind")]
        public string Building { 
            get
            {
                return _building;
            }
            set
            {
                _building = $"/buildings({new Guid(value)})";
            }
        }
        [JsonProperty("_company")]
        public string Company { get; set; }
        [JsonProperty("_creationdate_cian")]
        public DateTime CreationDate { get; set; }
        [JsonProperty("_flaturl")]
        public string FlatUrl { get; set; }
        [JsonProperty("_totalarea")]
        public int TotalArea { get; set; }
        [JsonProperty("_price")]
        public decimal Price { get; set; }
        [JsonProperty("_phonesflat")]
        public string PhonesFlat { get; set; }
        [JsonProperty("_phoneagent2")]
        public string PhonesAgent { get; set; }
        [JsonProperty("_name")]
        public string Name { get; set; }
    }


}
