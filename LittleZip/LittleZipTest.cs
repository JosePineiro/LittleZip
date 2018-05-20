using ClsParallel;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace LittleZipTest
{
    public partial class LittleZipTest : Form
    {
        public LittleZipTest()
        {
            InitializeComponent();
            if (IntPtr.Size == 8)
                this.Text = Application.ProductName + " v" + Application.ProductVersion + " [x64]";
            else
                this.Text = Application.ProductName + " v" + Application.ProductVersion + " [x86]";

            CheckForIllegalCrossThreadCalls = false;
        }

        private void LittleZipTest_Load(object sender, EventArgs e)
        {
            // Handle command line arguments
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 3)
            {
                this.textBoxSource.Text = args[1];
                this.textBoxZipFile.Text = args[2];
                buttonZIP_Click(null, null);
                System.Windows.Forms.Application.Exit();
            }
        }

        /// <summary>Select the source directory</summary>
        private void buttonSource_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    if (Directory.Exists(this.textBoxSource.Text))
                        folderBrowserDialog.SelectedPath = this.textBoxSource.Text;
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        this.textBoxSource.Text = folderBrowserDialog.SelectedPath;
                        this.textBoxZipFile.Text = folderBrowserDialog.SelectedPath + ".zip";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\nEn frmMain.buttonInPath_Click\r\nCapture esta pantalla y reporte en el foro de soporte.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Select the destination ZIP file</summary>
        private void buttonZipFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            if (Directory.Exists(Path.GetDirectoryName(this.textBoxZipFile.Text)))
                saveFileDialog.InitialDirectory = Path.GetDirectoryName(this.textBoxZipFile.Text);
            saveFileDialog.Filter = "Zip (*.zip)|*.zip";
            saveFileDialog.DefaultExt = ".zip";
            saveFileDialog.OverwritePrompt = false;
            if (DialogResult.OK == saveFileDialog.ShowDialog())
            {
                this.textBoxZipFile.Text = saveFileDialog.FileName;
            }
        }

        /// <summary>Zip source files in destination ZIP file</summary>
        private void buttonZIP_Click(object sender, System.EventArgs e)
        {
            //Get the files in dir
            if (!Directory.Exists(this.textBoxSource.Text))
            {
                MessageBox.Show("Please, select a source path.");
                return;
            }
            string[] files = Directory.GetFiles(this.textBoxSource.Text, "*.*", SearchOption.AllDirectories);
            this.progressBar.Maximum = files.Length;

            Stopwatch sw = new Stopwatch();

            sw.Start();

            //ZIP all files
            using (LittleZip zip = new LittleZip(this.textBoxZipFile.Text))
            {
                clsParallel.For(0, files.Length, delegate(int f)
                ///for (int f = 0; f < files.Length; f++)
                {
                    zip.AddFile(files[f], files[f].Substring(this.textBoxSource.Text.Length), 12, "");
                    this.progressBar.Value++;
                    Application.DoEvents();
                } );
            }
            sw.Stop();
            MessageBox.Show("Elapsed=" + sw.Elapsed);

            this.progressBar.Value = 0;
        }
    }
}

