namespace Downloader.Tools;

internal class OnlyChunkWriter
{
    public static void Write(string sliceId, byte[] barray)
    {;
        var slicepath = SliceManager.GetSlicePath(sliceId);
        var fpath = Path.Combine(Config.DownloadDirectory, slicepath);
        var dir2 = Path.GetDirectoryName(fpath);
        if (dir2 != null)
            Directory.CreateDirectory(dir2);
        if (!File.Exists(fpath))
            File.WriteAllBytes(Path.Combine(Config.DownloadDirectory, slicepath), barray);
    }
}
