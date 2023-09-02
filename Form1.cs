using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DigitalSign
{
    public partial class Form1 : Form
    {
        public string username = string.Empty;
        public string generalpath = string.Empty;
        public string sourcepath_ = string.Empty;
        public string destinationpath_ = string.Empty;
        public Form1()
        {
            InitializeComponent();
         
            textBox1.Enabled = false;
            username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\').Last().ToString();

            CheckDirectory();
            ClearSigningPath();

        }
        void CheckDirectory()
        {
            generalpath = @"C:\Users\" + username + @"\AppData\Roaming\Autodesk\Revit\Signing";

            if (!Directory.Exists(generalpath))
            {
                Directory.CreateDirectory(generalpath);
            }
        }

        void ClearSigningPath()
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(generalpath);

                if (!di.GetFiles().Count().Equals(0))
                {
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Length.Equals(0))
            {
                //YAPILACAK
                GetDllFile();
                Process(destinationpath_);
            }
        }

        private void GetFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "Executable Files|*.dll;*.exe;*.msi|All Files (*.*)|*.*"; // Filter for specific file types
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                textBox1.Text = selectedFilePath; // Display the selected file's path in a TextBox or another control
            }
        }
        void Process(string filepath)
        {
            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false; // Set this to false

                // Set the working directory to the location of signtool.exe
                cmd.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64";

                cmd.Start();

                cmd.StandardInput.WriteLine(@"signtool sign /a /tr http://timestamp.globalsign.com/tsa/r6advanced1 /td SHA256 /fd SHA256 " + filepath);
                cmd.StandardInput.Flush();

                // Remove the line that closes the input stream
                // cmd.StandardInput.Close();

                string output = cmd.StandardOutput.ReadToEnd();
                cmd.WaitForExit();

                // Display the output for debugging
                Console.WriteLine(output);

                // Check if the process exited with an error
                //if (cmd.ExitCode != 0)
                //{
                //    System.Windows.Forms.MessageBox.Show("Error occurred: " + output);
                //}

                OpenSignedFile();
            }
            catch 
            {
            }
        }
        void GetDllFile()
        {
            sourcepath_ = textBox1.Text;
            destinationpath_ = generalpath + @"\" + sourcepath_.Split('\u005c').Last();

            try
            {
                File.Move(sourcepath_, destinationpath_);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
        void OpenSignedFile()
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", generalpath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GetFile();
        }
    }
}
