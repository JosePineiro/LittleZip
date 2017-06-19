using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using ClsParallel;

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

        /// <summary>Select the source directory</summary>
        private void buttonSource_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    if (Directory.Exists(this.textBoxSouce.Text))
                        folderBrowserDialog.SelectedPath = this.textBoxSouce.Text;
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        this.textBoxSouce.Text = folderBrowserDialog.SelectedPath;
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
            string[] files = Directory.GetFiles(this.textBoxSouce.Text, "*.*", SearchOption.AllDirectories);
            this.progressBar.Maximum = files.Length;

            if (File.Exists(this.textBoxZipFile.Text))
            {
                //ZIP file exist append the files
                using (LittleZip zip = LittleZip.Open(this.textBoxZipFile.Text))
                {
                    clsParallel.For(0, files.Length, delegate(int f)
                    //for (int f = 0; f < files.Length; f++)
                    {
                        zip.AddFile(files[f], files[f].Substring(this.textBoxSouce.Text.Length), "");
                        this.progressBar.Value++;
                        Application.DoEvents();
                    });
                }
            }
            else
            {
                //ZIP file not exist. Create one and store the files
                using (LittleZip zip = LittleZip.Create(this.textBoxZipFile.Text))
                {
                    clsParallel.For(0, files.Length, delegate(int f)
                    //for (int f = 0; f < files.Length; f++)
                    {
                        zip.AddFile(files[f], files[f].Substring(this.textBoxSouce.Text.Length), "");
                        this.progressBar.Value++;
                        Application.DoEvents();
                    });
                }
            }

            this.progressBar.Value = 0;
        }
    }
}
