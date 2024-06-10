using RestSharp;

namespace CoreLib
{
    public class Appname
    {
        public static string GetAppName(uint prodId)
        {
            var basic = "Unknown";
            var client = new RestClient($"https://raw.githubusercontent.com/UplayDB/JustDumps/main/prodname/{prodId}.txt");
            var request = new RestRequest();
            try
            {
                RestResponse response = client.Get(request);
                if (response.Content != null)
                {
                    return response.Content;
                }
                return basic;
            }
            catch
            {
                return basic;
            }
            
        }
    }
}
