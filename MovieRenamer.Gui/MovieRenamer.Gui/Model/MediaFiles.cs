using System.Collections.Generic;

namespace MovieRenamer.Gui.Model
{
    public class MediaFiles
    {
        public MediaFiles(string[] files)
        {
            Files = new List<MediaFile>();

            foreach (string file in files) {
                Files.Add(new MediaFile(file));
            }
        }

        public List<MediaFile> Files { get; private set; }
    }
}