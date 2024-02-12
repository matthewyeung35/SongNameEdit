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
            string[] fileList = Directory.GetFiles(folderPath.Text);
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
                        if (!string.IsNullOrEmpty(songTitle))
                        {
                            string validSongTitle = Regex.Replace(songTitle, "[\\/:*?\"<>|]", "");
                            newFileName = validSongTitle + ".flac";
                            newFileName = new string(newFileName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
                            newFileName = Path.Combine(Path.GetDirectoryName(file), newFileName);
                        }
                    }
                    File.Move(file, newFileName);
                    //Remove trailing numbers from file name
                    int i;
                    for (i = 0; i < file.Length; i++)
                    {
                        if (!trailingNumbers.Contains(fileName[i]))
                        {
                            break;
                        }
                    }
                    if (i > 0)
                    {
                        newFileName = Path.Combine(Path.GetDirectoryName(file), fileName.Substring(i));
                        File.Move(file, newFileName);
                    }
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