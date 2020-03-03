using CianPlatform.Interface;
using CianPlatform.Models;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using System.Collections.Generic;
using System;
using Polly;
using System.Net.Http;
using CianPlatform.Extension;
using CianLayerProcessing.UploaderCrm;
using Serilog.Core;
using Newtonsoft.Json;

namespace CianPlatform
{
    public class DataProcessingCian : IDataProcessingCian
    {
        ResultModel _resultModel;
        BasicModel _basicModel;
        UploaderCianData _uploader;
        int countOffers = 0;
        public DataProcessingCian(ResultModel resultModel, BasicModel basicModel, UploaderCianData uploader)
        {
            _resultModel = resultModel;
            _basicModel = basicModel;
            _uploader = uploader;
        }

        public async Task<ResultModel> DataProcessingAsync(string project, string building)
        {
            RequestParamIsNull(project, building);

            int projectId = HandlerParseException(project);

            List<OffersSerializedResponse> serializedResponses = await GetCollectionOffers(projectId);
            int countOffers = serializedResponses.Count();

            _resultModel.Data = new List<Dictionary<string, dynamic>>(countOffers);

            foreach (OffersSerializedResponse offer in serializedResponses)
            {
                CianData cianData = new CianData(offer, building);
                try
                {
                    await _uploader.DeleteAndUploadAsync(cianData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"message {ex.Message}, method: DeleteAndUploadAsync");
                }
            }

            _resultModel.Count = countOffers;

            return _resultModel;
        }

        private static void RequestParamIsNull(string project, string building)
        {
            if (string.IsNullOrEmpty(project) || project == null)
            {
                Console.WriteLine("param project is null or empty");
                throw new ArgumentException("message", nameof(project));
            }

            if (string.IsNullOrEmpty(building) || building == null)
            {
                Console.WriteLine("param building is null or empty");
                throw new ArgumentException("message", nameof(building));
            }
        }

        private int HandlerParseException(string project)
        {
            int projectId;
            try
            {
                projectId = int.Parse(project);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"message {ex.Message}, method: HandlerParseException");
                throw new FormatException(ex.Message, ex.InnerException);
            }

            return projectId;
        }

        private async Task<List<OffersSerializedResponse>> GetCollectionOffers(int project)
        {
            List<OffersSerializedResponse> offersSerialized = new List<OffersSerializedResponse>();
            ApiOffers offers = new ApiOffers(project);

            do
            {
                try
                {
                    ApiOffersResponse apiOffersResponse = await PostOffersAsync(offers);

                    countOffers = apiOffersResponse.Data.SerializedResponse.Count();

                    offersSerialized.AddRange(apiOffersResponse.Data.SerializedResponse.Select(apiOfferResponse => apiOfferResponse));
                    offers.IncrementingPage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"message: {ex.Message} // method: GetCollectionOffers ");
                }
            } while (countOffers > 0);

            return offersSerialized;
        }

        private async Task<ApiOffersResponse> PostOffersAsync(ApiOffers offers)
        {
            IFlurlResponse httpResponse = await Policy
                .Handle<HttpRequestException>()
                .Or<FlurlHttpException>()
                .OrResult<IFlurlResponse>(result => !result.ResponseMessage.IsSuccessStatusCode)
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromSeconds(1) })
                .ExecuteAsync(async () => await _basicModel.BasicApiOffers.PostJsonAsync(offers));

            ApiOffersResponse apiOffersResponse = await httpResponse.GetJsonAsync<ApiOffersResponse>();
            return apiOffersResponse;
        }
    }
}
