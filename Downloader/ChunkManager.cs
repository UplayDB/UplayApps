using Uplay.Download;
using File = Uplay.Download.File;

namespace Downloader
{
    internal class ChunkManager
    {
        public static List<File> RemoveNonEnglish(Manifest parsedManifest)
        {
            List<Chunk> reqChunks = parsedManifest.Chunks.Where(x => String.IsNullOrEmpty(x.Language) && x.Type == Chunk.Types.ChunkType.Required).ToList(); // filter out non-English
            List<Chunk> optChunks = parsedManifest.Chunks.Where(x => String.IsNullOrEmpty(x.Language) && x.Type == Chunk.Types.ChunkType.Optional).ToList(); // filter out non-English

            List<File> files = new();

            foreach (var chunk in reqChunks)
                files = files.Concat(chunk.Files).ToList();

            foreach (var chunk in optChunks)
                files = files.Concat(chunk.Files).ToList();

            return files;
        }

        public static List<File> AllFiles(Manifest parsedManifest)
        {
            var chunks = parsedManifest.Chunks.ToList();
            List<File> files = new();

            foreach (var chunk in chunks)
                files = files.Concat(chunk.Files).ToList();

            return files;
        }

        public static List<File> AddLanguage(Manifest parsedManifest, string Lang)
        {
            List<Chunk> reqChunks = parsedManifest.Chunks.Where(x => x.Language.Equals(Lang) && x.Type == Chunk.Types.ChunkType.Required).ToList();
            List<Chunk> optChunks = parsedManifest.Chunks.Where(x => x.Language.Equals(Lang) && x.Type == Chunk.Types.ChunkType.Optional).ToList();

            List<File> files = new();

            foreach (var chunk in reqChunks)
                files = files.Concat(chunk.Files).ToList();

            foreach (var chunk in optChunks)
                files = files.Concat(chunk.Files).ToList();

            return files;
        }

        public static List<File> RemoveSkipFiles(List<File> files, List<string> skip_files)
        {
            List<File> to_remove = new();
            if (skip_files.Count != 0)
            {
                foreach (var file in files)
                {
                    foreach (var skip in skip_files)
                    {
                        if (file.Name.Contains(skip))
                        {
                            to_remove.Add(file);
                        }
                    }
                }
                foreach (var remove in to_remove)
                {
                    files.Remove(remove);
                }

            }

            return files;
        }

        public static List<File> AddDLOnlyFiles(List<File> files, List<string> add_files)
        {
            List<File> output = new();
            if (add_files.Count != 0)
            {
                foreach (var file in files)
                {
                    foreach (var add in add_files)
                    {
                        if (file.Name.Contains(add))
                        {
                            output.Add(file);
                        }
                    }
                }
            }

            return output;
        }
    }
}
