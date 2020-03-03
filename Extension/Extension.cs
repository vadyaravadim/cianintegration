using CianPlatform.Models;

namespace CianPlatform.Extension
{
    public static class Extension
    {
        public static ApiOffers IncrementingPage(this ApiOffers offers)
        {
            offers.JsonQuery.Page.Value++;
            return offers;
        }
    }
}
