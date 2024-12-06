using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO.Compression;
using ZstdNet;
using LzhamWrapper;

namespace CoreLib;

public class DeComp
{
    public static byte[] Decompress(bool IsCompressed, string CompressionMethod, byte[] bytesToDecompress, ulong outputsize)
    {
        if (!IsCompressed)
        {
            return bytesToDecompress;
        }

        if (CompressionMethod.Contains("CompressionMethod_"))
            CompressionMethod = CompressionMethod.Replace("CompressionMethod_", "");

        CompressionMethod = CompressionMethod.ToLower();
        switch (CompressionMethod) // check compression method
        {
            case "zstd":
                using (Decompressor decompressorZstd = new())
                {
                    byte[] returner = decompressorZstd.Unwrap(bytesToDecompress);
                    return returner;
                }
            case "deflate":
                using (InflaterInputStream decompressor = new(new MemoryStream(bytesToDecompress), new(false)))
                {
                    MemoryStream ms = new((int)outputsize);
                    decompressor.CopyTo(ms);
                    return ms.ToArray();
                }
            case "lzham":
                DecompressionParameters d = new()
                {
                    Flags = LzhamWrapper.Enums.DecompressionFlag.ComputeAdler32 | LzhamWrapper.Enums.DecompressionFlag.ReadZlibStream,
                    DictionarySize = 15,
                    UpdateRate = LzhamWrapper.Enums.TableUpdateRate.Default
                };
                MemoryStream mem = new((int)outputsize);
                using (LzhamStream lzhamStream = new(new MemoryStream(bytesToDecompress), d))
                {
                    lzhamStream.CopyTo(mem);
                    return mem.ToArray();
                }   
        }
        return bytesToDecompress;
    }

    public static byte[] Compress(bool IsCompressed, string CompressionMethod, byte[] bytesToCompress)
    {
        if (!IsCompressed)
        {
            return bytesToCompress;
        }

        if (CompressionMethod.Contains("CompressionMethod_"))
            CompressionMethod = CompressionMethod.Replace("CompressionMethod_", "");

        CompressionMethod = CompressionMethod.ToLower();

        switch (CompressionMethod) // check compression method
        {
            case "zstd":
                Compressor compressZstd = new();
                byte[] returner = compressZstd.Wrap(bytesToCompress);
                compressZstd.Dispose();
                return returner;
            case "deflate":
                MemoryStream ms = new();
                ZLibStream compressor = new(new MemoryStream(bytesToCompress), CompressionLevel.SmallestSize);
                ms.CopyTo(compressor);
                compressor.Close();
                return ms.ToArray();
            case "lzham":
                Console.WriteLine("Compressing with Lzham hasnt been actually reversed yet!");
                //return LzhamWrapper.Compress(downloadedSlice, outputsize);
                return bytesToCompress;
        }
        return bytesToCompress;
    }
}
