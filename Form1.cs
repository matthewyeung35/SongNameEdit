using FlacLibSharp;
using System.Text.RegularExpressions;

namespace SongNameEdit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string trailingNumbers = "0123456789. ";

        private void start_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(folderPath.Text)) return;
            string[] fileList = Directory.GetFiles(folderPath.Text);
            // Backup Folder
            string backupFolderPath = Path.Combine(folderPath.Text, "backup");
            Directory.CreateDirectory(backupFolderPath);
            foreach (string file in fileList)
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    string destinationFile = Path.Combine(backupFolderPath, fileName);
                    File.Copy(file, destinationFile);
                }
                catch
                {
                }
            }

            // Rename
            foreach (string file in fileList)
            {
                try {
                    string fileName = Path.GetFileName(file);
                    if (Path.GetExtension(file) != ".flac")
                    {
                        continue;
                    }
                    // Use Metadata to rename .flac file
                    string newFileName = "";
                    using (FlacFile flacFile = new FlacFile(file))
                    {
                        string songTitle = flacFile.VorbisComment.Title.ToString();
                        string trackId = flacFile.VorbisComment.TrackNumber.ToString();
                        if (!string.IsNullOrEmpty(trackId))
                        {
                            string validId = Regex.Replace(trackId, "[\\/:*?\"<>|]", "");
                            newFileName += validId + " ";
                            newFileName = new string(newFileName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        }

                        if (!string.IsNullOrEmpty(songTitle))
                        {
                            string validSongTitle = Regex.Replace(songTitle, "[\\/:*?\"<>|]", "");
                            newFileName += validSongTitle;
                            newFileName = new string(newFileName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                        }
                    }
                    newFileName = Path.Combine(Path.GetDirectoryName(file), newFileName);
                    File.Move(file, newFileName + ".flac");

                    ////Remove trailing numbers from file name
                    //int i;
                    //for (i = 0; i < file.Length; i++)
                    //{
                    //    if (!trailingNumbers.Contains(fileName[i]))
                    //    {
                    //        break;
                    //    }
                    //}
                    //if (i > 0)
                    //{
                    //    newFileName = Path.Combine(Path.GetDirectoryName(file), fileName.Substring(i));
                    //    File.Move(file, newFileName);
                    //}
                }
                catch 
                { 
                }
            }

        }

        private void browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyMusic;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderBrowserDialog.SelectedPath;
                folderPath.Text = selectedFolder;

                
            }
        }
    }
}