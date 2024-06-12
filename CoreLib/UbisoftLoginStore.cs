﻿using Google.Protobuf;
using System.IO.Compression;
using System.IO.IsolatedStorage;
using Uplay.UserDatFile;
using Uplay.RememberDeviceFile;

namespace CoreLib
{
    public class UbisoftLoginStore
    {
        public Cache UserDatCache;
        public UserLoginCache RememberCache;
        UbisoftLoginStore()
        {
            UserDatCache = new()
            {
                Prod = new()
                {
                    StartupEntry = new(),
                    Users = { }
                }
            };
            RememberCache = new()
            { 
                Users = { },
                Version = 1
            };
            FileName = "";
        }
        string FileName;
        public static UbisoftLoginStore Instance;
        public static bool UseIsolatedStorage = true;
        static readonly IsolatedStorageFile IsolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
        static bool Loaded
        {
            get { return Instance != null; }
        }

        public static void LoadFromFile(string filename)
        {
            if (Loaded && Instance.FileName == filename)
                return;

            FileStream? fs = null;
            if (IsolatedStorage.FileExists(filename) && UseIsolatedStorage )
                fs = IsolatedStorage.OpenFile(filename, FileMode.Open, FileAccess.Read);
            else if (File.Exists(filename))
            {
                fs = File.Open(filename, FileMode.Open, FileAccess.Read);
            }
            else
            {
                Instance = new();
            }
            if (fs != null)
            {
                try
                {
                    using var ds = new DeflateStream(fs, CompressionMode.Decompress);
                    using var ms = new MemoryStream();
                    ds.CopyTo(ms);
                    ds.Dispose();
                    Instance = new();
                    var array = ms.ToArray();
                    var userdatalen = BitConverter.ToInt32(array[..4]);
                    Instance.UserDatCache = Cache.Parser.ParseFrom(array[4..(4 + userdatalen)]);
                    var remdatalen = BitConverter.ToInt32(array[(4 + userdatalen)..(8 + userdatalen)]);
                    Instance.RememberCache = UserLoginCache.Parser.ParseFrom(array[(userdatalen + 8)..(userdatalen + remdatalen + 8)]);
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Failed to load account settings: {0}", ex.Message);
                    Instance = new();
                }
            }
            Instance.FileName = filename;
        }

        public static void Save()
        {
            if (!Loaded)
                LoadFromFile("LoginStore.dat");

            try
            {
                var fs = Stream.Null;
                if (UseIsolatedStorage)
                    fs = IsolatedStorage.OpenFile(Instance.FileName, FileMode.Create, FileAccess.Write);
                else
                {
                    fs = File.Open(Instance.FileName, FileMode.Create, FileAccess.Write);
                }

                if (fs != Stream.Null)
                {
                    using var ds = new DeflateStream(fs, CompressionMode.Compress);
                    var datacache = Instance.UserDatCache.ToByteArray();
                    ds.Write(BitConverter.GetBytes(datacache.Length));
                    ds.Write(datacache);
                    var remcache = Instance.RememberCache.ToByteArray();
                    ds.Write(BitConverter.GetBytes(remcache.Length));
                    ds.Write(remcache);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("Failed to save account settings: {0}", ex.Message);
            }
        }
    }
}