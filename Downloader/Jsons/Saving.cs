using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZstdNet;

namespace Downloader
{
    internal class Saving
    {
        public class Root
        {
            [JsonPropertyName("UplayId")]
            public int UplayId { get; set; } = 0;

            [JsonPropertyName("ManifestSHA1")]
            public string ManifestSHA1 { get; set; } = string.Empty;

            [JsonPropertyName("Version")]
            public int Version { get; set; } = 0;

            [JsonPropertyName("Compression")]
            public Compression Compression { get; set; } = new();

            [JsonPropertyName("Work")]
            public Work Work { get; set; } = new();

            [JsonPropertyName("Verify")]
            public Verify Verify { get; set; } = new();
        }
        public class Compression
        {
            [JsonPropertyName("Method")]
            public string Method { get; set; } = string.Empty;

            [JsonPropertyName("IsCompressed")]
            public bool IsCompressed { get; set; }

            [JsonPropertyName("HasSliceSHA")]
            public bool HasSliceSHA { get; set; }
        }
        public class Work
        {
            [JsonPropertyName("FileInfos")]
            public List<FileInfo> FileInfos { get; set; } = new();
        }
        public class Verify
        {
            [JsonPropertyName("Files")]
            public List<File> Files { get; set; } = new();
        }
        public class File
        {
            [JsonPropertyName("Name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("SliceInfo")]
            public List<SliceInfo> SliceInfo { get; set; } = new();
        }
        public class SliceInfo
        {
            [JsonPropertyName("CompressedSHA")]
            public string CompressedSHA { get; set; } = string.Empty;

            [JsonPropertyName("DecompressedSHA")]
            public string DecompressedSHA { get; set; } = string.Empty;

            [JsonPropertyName("DownloadedSize")]
            public int DownloadedSize { get; set; } = 0;

            [JsonPropertyName("DecompressedSize")]
            public int DecompressedSize { get; set; } = 0;

            public override string ToString()
            {
                return $"C SHA: {CompressedSHA}, D SHA: {DecompressedSHA}, DL Size: {DownloadedSize}, DEC Size: {DecompressedSize}";
            }
        }

        public class FileInfo
        {
            [JsonPropertyName("Name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("IDs")]
            public IDs IDs { get; set; } = new();

            [JsonPropertyName("CurrentId")]
            public string CurrentId { get; set; } = string.Empty;

            [JsonPropertyName("NextId")]
            public string NextId { get; set; } = string.Empty;
        }

        public class IDs
        {
            [JsonPropertyName("Slices")]
            public List<string> Slices { get; set; } = new();

            [JsonPropertyName("SliceList")]
            public List<string> SliceList { get; set; } = new();
        }

        public static void Save(Root root)
        {
            if (string.IsNullOrEmpty(Config.VerifyBinPath))
                return;
            var ser = JsonSerializer.Serialize(root);
            var bytes = Encoding.UTF8.GetBytes(ser);
            Compressor compressor = new();
            var returner = compressor.Wrap(bytes);
            compressor.Dispose();
            System.IO.File.WriteAllBytes(Config.VerifyBinPath, returner);
        }

        public static Root Read()
        {
            if (string.IsNullOrEmpty(Config.VerifyBinPath))
                return new();
            var filebytes = System.IO.File.ReadAllBytes(Config.VerifyBinPath);
            Decompressor decompressorZstd = new();
            var decompressed = decompressorZstd.Unwrap(filebytes);
            decompressorZstd.Dispose();
            var ser = Encoding.UTF8.GetString(decompressed);
            var root = JsonSerializer.Deserialize<Root>(ser);
            if (root == null)
                root = new();
            return root;
        }

        public static Root MakeNew(uint productId, string manifest, Uplay.Download.Manifest parsedManifest)
        {
            return new()
            {
                UplayId = (int)productId,
                ManifestSHA1 = manifest,
                Version = (int)parsedManifest.Version,
                Compression = new()
                {
                    IsCompressed = parsedManifest.IsCompressed,
                    Method = parsedManifest.CompressionMethod.ToString(),
                    HasSliceSHA = parsedManifest.Chunks.First().Files.First().SliceList.First().HasDownloadSha1
                },
                Work = new()
                {
                    FileInfos = new()
                },
                Verify = new()
                {
                    Files = new()
                }
            };
        }
    }
}
/*
{
    "UplayId": 0,
    "ManifestSHA1": "",
    "Version": 3,
    "Compression": {
        "Method":"Zstd",
        "IsCompressed":True
    },
    "Work":{
        "FileInfo":{
            "Name":"x.txt",
            "IDs":{
                "Slices": [],
                "SliceList": [ "" ]
            }
        },
        "CurrentId": "",
        "NextId": ""

    },
    "Verify": {
        "Files": [
            {
                "Name":"x.txt",
                "DecompressedSlices": [ {"CompressedSHA":"","DecompressedSHA":"","DownloadedSize":10} ]
            }
        ]

    }
}
 */