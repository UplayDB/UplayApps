using Google.Protobuf;
using System.IO.Compression;
using System.IO.IsolatedStorage;
using Uplay.UserDatFile;

namespace CoreLib
{
    public class UbisoftLoginStore
    {
        public Cache CacheFile;
        UbisoftLoginStore()
        {
            CacheFile = new()
            {
                Prod = new()
                {
                    StartupEntry = new(),
                    Users = { }
                }
            };
        }
        string FileName;
        public static UbisoftLoginStore Instance;
        static readonly IsolatedStorageFile IsolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
        static bool Loaded
        {
            get { return Instance != null; }
        }

        public static void LoadFromFile(string filename)
        {
            if (Loaded)
                throw new Exception("Config already loaded");

            if (IsolatedStorage.FileExists(filename))
            {
                try
                {
                    using var fs = IsolatedStorage.OpenFile(filename, FileMode.Open, FileAccess.Read);
                    using var ds = new DeflateStream(fs, CompressionMode.Decompress);
                    using var ms = new MemoryStream();
                    ds.CopyTo(ms);
                    ds.Dispose();
                    Instance = new();
                    Instance.CacheFile = Cache.Parser.ParseFrom(ms.ToArray());
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to load account settings: {0}", ex.Message);
                    Instance = new();
                }
            }
            else
            {
                Instance = new();
            }

            Instance.FileName = filename;
        }

        public static void Save()
        {
            if (!Loaded)
                throw new Exception("Saved config before loading");

            try
            {
                using var fs = IsolatedStorage.OpenFile(Instance.FileName, FileMode.Create, FileAccess.Write);
                using var ds = new DeflateStream(fs, CompressionMode.Compress);
                ds.Write(Instance.CacheFile.ToByteArray());
            }
            catch (IOException ex)
            {
                Console.WriteLine("Failed to save account settings: {0}", ex.Message);
            }
        }
    }
}
