using System.ComponentModel;
using System.IO;

namespace MovieRenamer.Gui.Model
{
    public class MediaFile
    {
        private string destinationFolder;

        public MediaFile(string file)
        {
            this.FileName = file;
            this.DestinationFolder = @"C:\tmp\renamed_files\";
        }

        public string FileName { get; private set; }

        private string RenamedFile { get; set; }

        public string DestinationFolder
        {
            private get
            {
                if (!string.IsNullOrWhiteSpace(this.destinationFolder)) {
                    return !this.destinationFolder.EndsWith(@"\")
                        ? this.destinationFolder + @"\"
                        : this.destinationFolder;
                }
                return string.Empty;
            }
            set { destinationFolder = value; }
        }

        [DisplayName("5")]
        public bool Five { get; set; }
        [DisplayName("6")]
        public bool Six { get; set; }
        [DisplayName("7")]
        public bool Seven { get; set; }
        [DisplayName("8")]
        public bool Eight { get; set; }
        [DisplayName("9")]
        public bool Nine { get; set; }
        [DisplayName("10")]
        public bool Ten { get; set; }
        [DisplayName("11")]
        public bool Eleven { get; set; }
        [DisplayName("12")]
        public bool Twelve { get; set; }

        public string Rename(string delimiter = "_", string gradeName = "Kl_")
        {
            bool anyGradeSelected = this.AnyGradeSelected();
            bool exists = File.Exists(this.FileName);
            if (!anyGradeSelected || !exists) return null;

            string fileName = Path.GetFileName(this.FileName);
            fileName = gradeName + fileName;
            fileName = PrependGrade(fileName, delimiter);
            this.RenamedFile = fileName;
            return this.RenamedFile;
        }

        private bool AnyGradeSelected()
        {
            return Five || Six || Seven || Eight || Nine || Ten || Eleven || Twelve;
        }

        private string PrependGrade(string fileName, string delimiter)
        {
            string result = string.Empty;
            if (Twelve) result = 12 + delimiter + result;
            if (Eleven) result = 11 + delimiter + result;
            if (Ten) result = 10 + delimiter + result;
            if (Nine) result = 9 + delimiter + result;
            if (Eight) result = 8 + delimiter + result;
            if (Seven) result = 7 + delimiter + result;
            if (Six) result = 6 + delimiter + result;
            if (Five) result = 5 + delimiter + result;
            return result + fileName;
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(this.RenamedFile) || string.IsNullOrWhiteSpace(this.DestinationFolder)) {
                // TODO
                return;
            }

            File.Copy(this.FileName, this.DestinationFolder + this.RenamedFile, true);
        }

        public string GetRenamedFile()
        {
            return this.DestinationFolder + this.RenamedFile;
        }
    }
}