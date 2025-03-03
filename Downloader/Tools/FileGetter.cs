using RestSharp;

namespace Downloader.Tools;

internal class FileGetter
{
    static readonly RestClient Rest;

    static FileGetter()
    {
        Rest = new RestClient();
    }

    public static byte[]? DownloadFromURL(string url)
    {
        return Rest.DownloadData(new(url));
    }
}
