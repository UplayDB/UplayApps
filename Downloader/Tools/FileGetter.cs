using RestSharp;
using System;

namespace Downloader.Tools;

internal class FileGetter
{
    static RestClient Rest;

    static FileGetter()
    {
        Rest = new RestClient();
    }

    public static byte[]? DownloadFromURL(string url)
    {
        return Rest.DownloadData(new(url));
    }
}
