using RestSharp;
using System.Reflection;

namespace CoreLib
{
    public class VersionCheck
    {
        public static bool Check()
        {
            var callAss = Assembly.GetCallingAssembly();
            var TypesVersion = callAss.GetTypes().Where(z => z.GetFields().Where(b => b.Name == "Version").Any()).ToArray();
            var Namespace = TypesVersion.Select(t => t.Namespace).Distinct().First();

            var versionField = TypesVersion.Select(x=>x.GetField("Version")).First();
            if (versionField != null)
            {        
                var version = versionField.GetValue(callAss) as string;

                var client = new RestClient($"https://raw.githubusercontent.com/UplayDB/UplayApps/main/Released/{Namespace}/{version}.txt");
                var request = new RestRequest();

                try
                {
                    RestResponse response = client.Get(request);
                    if (response.Content != null)
                    {
                        return (response.Content == version);
                    }
                }
                catch
                {
                }
            }
            return true;
        }
    }
}
