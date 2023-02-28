using RestSharp;
using System.Reflection;

namespace CoreLib
{
    public class VersionCheck
    {
        public static bool Check()
        {
            Assembly callAss = Assembly.GetCallingAssembly();
            Type[] TypesVersion = callAss.GetTypes().Where(z => z.GetFields().Where(b => b.Name == "Version").Any()).ToArray();
            string? Namespace = TypesVersion.Select(t => t.Namespace).Distinct().First();

            FieldInfo? versionField = TypesVersion.Select(x=>x.GetField("Version")).First();
            if (versionField != null)
            {
                string? version = versionField.GetValue(callAss) as string;

                RestClient client = new RestClient($"https://raw.githubusercontent.com/UplayDB/UplayApps/main/Released/{Namespace}/{version}.txt");
                RestRequest request = new RestRequest();

                try
                {
                    RestResponse response = client.Get(request);
                    if (response.Content != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine(response.Content);
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
