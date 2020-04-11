using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BannerlordModuleInstaller
{
    public partial class Form1 : Form
    {
        static public string BannerlordDir;
        static public string InstallationDir;
        static public string ModulePath;

        static public ModuleInfo ModuleInfo;
        bool DoBackup = false;

        public Form1()
        {
            InitializeComponent();

            BannerlordDir = BannerlordDirectoryFinder.Find();
            InstallationDir = Path.Combine(BannerlordDir, "Modules");
            
            textBoxInstallationDir.Text = InstallationDir;

            using (FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read))
            {
                byte[] magicGuidBytes = Program.MagicGuidBytes;
                fs.Position = fs.Length - magicGuidBytes.Length;
                byte[] comparisonBytes = new byte[magicGuidBytes.Length];
                fs.Read(comparisonBytes, 0, comparisonBytes.Length);
                if (Program.ByteArrayCompare(ref magicGuidBytes, ref comparisonBytes))
                {
                    ModuleInfo = ModuleInfo.FromPackedStream(fs);
                }
                else
                {
                    MessageBox.Show("Creating ModuleInfo.txt, BuildInstaller.bat and a ModuleFolder.", "Empty Installer");

                    using (StreamWriter sw = new StreamWriter("ModuleInfo.txt"))
                    {
                        sw.WriteLine(ModuleInfo.NameBeginsWith);
                        sw.WriteLine(ModuleInfo.VersionBeginsWith);
                        sw.WriteLine(ModuleInfo.AuthorBeginsWith);
                        sw.WriteLine(ModuleInfo.AuthorLinkBeginsWith);
                        sw.WriteLine(ModuleInfo.AuthorIsWebLinkBeginsWith);
                        sw.WriteLine(ModuleInfo.ImagePathBeginsWith);
                        sw.WriteLine(ModuleInfo.IconPathBeginsWith);
                    }

                    using (StreamWriter sw = new StreamWriter("BuildInstaller.bat"))
                    {
                        sw.WriteLine("\"" + Path.GetFileName(Application.ExecutablePath) + "\" -packModule ModuleInfo.txt ModuleFolder");
                    }

                    Directory.CreateDirectory("ModuleFolder");

                    buttonInstall.Enabled = false;
                    Environment.Exit(0);
                }
            }

            if(ModuleInfo != null)
            {
#if DEBUG
                //ConsoleHelper.Initialize();
                //Console.WriteLine(ModuleInfo.Name);
                //Console.WriteLine(ModuleInfo.Version);
                //Console.WriteLine(ModuleInfo.Author);
                //Console.WriteLine(ModuleInfo.AuthorLink);
                //Console.WriteLine(ModuleInfo.AuthorIsWebLink);
                //Console.WriteLine(ModuleInfo.Image);
                //Console.WriteLine(ModuleInfo.Icon);

                //ModuleInfo.Image.Save("image.jpg", ImageFormat.Jpeg);
                //using(FileStream fs = new FileStream("icon.ico", FileMode.Create))
                //    ModuleInfo.Icon.Save(fs);
#endif

                ModulePath = Path.Combine(InstallationDir, ModuleInfo.FolderName);

                pictureBoxImage.Image = ModuleInfo.Image;
                this.Icon = ModuleInfo.Icon;
                this.Text = ModuleInfo.Name + " " + ModuleInfo.Version + " Installer";
                linkLabelAuthor.Text = ModuleInfo.Author;

                if((ModuleInfo.AuthorIsWebLink == false && ModuleInfo.AuthorLink == "") == false)
                    linkLabelAuthor.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAuthor_LinkClicked);

                //Check if Module Already Exists
                if (Directory.Exists(ModulePath))
                {
                    string message = "It appears that this Module is already installed.\n\n" +
                                    "A backup will be created.\n\n" +
                                    "Do you want to continue?";

                    DialogResult dialogResult = MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        DoBackup = true;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        buttonInstall.Enabled = false;
                        Environment.Exit(0);
                    }
                }
            }
        }

        private void linkLabelAuthor_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            linkLabelAuthor.LinkVisited = true;

            try
            {
                System.Diagnostics.Process.Start(ModuleInfo.AuthorIsWebLink ? ModuleInfo.Author : ModuleInfo.AuthorLink);
            }
            catch(System.Exception exception)
            {
                string message = "Trying to Open Link: \"" + ModuleInfo.Author + "\" failed.\n" + exception.Message + "\n\n(IF YOU ARE MOD AUTHOR)\nIt's probably not a valid link, or try using \"authorLink\" field in ModuleInfo.txt";
                MessageBox.Show(message);
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            bool backupSuccessful = DoBackup ? Backup() : true;
            if (backupSuccessful == false) //This check may be redundant
                return;

            bool installSuccessful = Install();

            if (installSuccessful)
            {
                MessageBox.Show("Module Installed.", "Finished");
                buttonInstall.Enabled = false;
            }
        }

        private bool Install()
        {
            return Program.UnpackModule();
        }

        //FUN
        private bool Backup()
        {
            bool backupSuccessful = false;
            try
            {
                string movePath = ModulePath + "_backup";
                for (int i = 0; i < 10; i++)
                {
                    string temp = movePath + (i > 0 ? i.ToString() : "");
                    if (Directory.Exists(temp))
                        continue;

                    Directory.Move(ModulePath, temp);
                    goto BackupDone;
                }

                string promptValue = Prompt.ShowDialog("You have way too many backup folders.\n\nPlease enter a name for backup folder.", "-_-");
                goto SkipPrompAgain;

            PromptAgain:
                promptValue = Prompt.ShowDialog("Please enter another name for backup folder.", "Folder Exists");

            SkipPrompAgain:
                if (promptValue == null || promptValue.Length == 0)
                {
                    MessageBox.Show("Fine, have it your way.", "Installation Cancelled");
                    Environment.Exit(0);
                    return false;
                }
                else
                {
                    string newPath = Path.Combine(InstallationDir, promptValue);
                    if (Directory.Exists(newPath))
                    {
                        goto PromptAgain;
                    }
                    else
                    {
                        Directory.Move(ModulePath, newPath);
                    }
                }

            BackupDone:
                backupSuccessful = true;
            }
            catch (System.Exception exception)
            {
                MessageBox.Show(exception.Message + "\n\nInstallation will cancel.", "Backup Failed");
                return false;
            }

            return backupSuccessful;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/SpaceDandy-Tama/BannerlordModuleInstaller");
        }
    }
}