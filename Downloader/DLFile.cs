using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader
{
    internal class DLFile
    {
        public static List<Uplay.Download.File> FileNormalizer()
        {
            var workingfiles = DLWorker.Config.FilesToDownload;

            workingfiles.ForEach(x => x.Name.Replace('\\', '/'));
            workingfiles = workingfiles.AsParallel().Where(x=>IsFileIncluded(x.Name)).ToList();


            return workingfiles;
        }


        public static bool IsFileIncluded(string filename)
        {
            if (!DLWorker.Config.UsingFileList)
                return true;

            filename = filename.Replace('\\', '/');

            if (DLWorker.Config.FilesToDownload.Where(x=>x.Name.Contains(filename)).Any())
            {
                return true;
            }
            /*
            foreach (var rgx in DLWorker.Config.FilesToDownloadRegex)
            {
                var m = rgx.Match(filename);

                if (m.Success)
                    return true;
            }
            */
            return false;
        }
    }
}
