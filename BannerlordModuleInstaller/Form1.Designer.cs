namespace BannerlordModuleInstaller
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxInstallationDir = new System.Windows.Forms.TextBox();
            this.labelInstallationDir = new System.Windows.Forms.Label();
            this.pictureBoxImage = new System.Windows.Forms.PictureBox();
            this.buttonInstall = new System.Windows.Forms.Button();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.linkLabelAuthor = new System.Windows.Forms.LinkLabel();
            this.linkLabelInstallerAuthorGithub = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxInstallationDir
            // 
            this.textBoxInstallationDir.Location = new System.Drawing.Point(94, 605);
            this.textBoxInstallationDir.Name = "textBoxInstallationDir";
            this.textBoxInstallationDir.Size = new System.Drawing.Size(1484, 20);
            this.textBoxInstallationDir.TabIndex = 0;
            // 
            // labelInstallationDir
            // 
            this.labelInstallationDir.AutoSize = true;
            this.labelInstallationDir.Location = new System.Drawing.Point(12, 608);
            this.labelInstallationDir.Name = "labelInstallationDir";
            this.labelInstallationDir.Size = new System.Drawing.Size(76, 13);
            this.labelInstallationDir.TabIndex = 1;
            this.labelInstallationDir.Text = "Installation Dir:";
            // 
            // pictureBoxImage
            // 
            this.pictureBoxImage.Location = new System.Drawing.Point(-1, -1);
            this.pictureBoxImage.Name = "pictureBoxImage";
            this.pictureBoxImage.Size = new System.Drawing.Size(1600, 600);
            this.pictureBoxImage.TabIndex = 2;
            this.pictureBoxImage.TabStop = false;
            // 
            // buttonInstall
            // 
            this.buttonInstall.Location = new System.Drawing.Point(1503, 631);
            this.buttonInstall.Name = "buttonInstall";
            this.buttonInstall.Size = new System.Drawing.Size(75, 23);
            this.buttonInstall.TabIndex = 3;
            this.buttonInstall.Text = "Install";
            this.buttonInstall.UseVisualStyleBackColor = true;
            this.buttonInstall.Click += new System.EventHandler(this.buttonInstall_Click);
            // 
            // labelAuthor
            // 
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(12, 636);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(41, 13);
            this.labelAuthor.TabIndex = 4;
            this.labelAuthor.Text = "Author:";
            // 
            // linkLabelAuthor
            // 
            this.linkLabelAuthor.AutoSize = true;
            this.linkLabelAuthor.Location = new System.Drawing.Point(94, 636);
            this.linkLabelAuthor.Name = "linkLabelAuthor";
            this.linkLabelAuthor.Size = new System.Drawing.Size(36, 13);
            this.linkLabelAuthor.TabIndex = 5;
            this.linkLabelAuthor.TabStop = true;
            this.linkLabelAuthor.Text = "github";
            // 
            // linkLabelInstallerAuthorGithub
            // 
            this.linkLabelInstallerAuthorGithub.AutoSize = true;
            this.linkLabelInstallerAuthorGithub.Location = new System.Drawing.Point(1305, 636);
            this.linkLabelInstallerAuthorGithub.Name = "linkLabelInstallerAuthorGithub";
            this.linkLabelInstallerAuthorGithub.Size = new System.Drawing.Size(192, 13);
            this.linkLabelInstallerAuthorGithub.TabIndex = 6;
            this.linkLabelInstallerAuthorGithub.TabStop = true;
            this.linkLabelInstallerAuthorGithub.Text = "https://github.com/SpaceDandy-Tama";
            this.linkLabelInstallerAuthorGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1590, 659);
            this.Controls.Add(this.linkLabelInstallerAuthorGithub);
            this.Controls.Add(this.linkLabelAuthor);
            this.Controls.Add(this.labelAuthor);
            this.Controls.Add(this.buttonInstall);
            this.Controls.Add(this.pictureBoxImage);
            this.Controls.Add(this.labelInstallationDir);
            this.Controls.Add(this.textBoxInstallationDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Bannerlord Module Installer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxInstallationDir;
        private System.Windows.Forms.Label labelInstallationDir;
        private System.Windows.Forms.PictureBox pictureBoxImage;
        private System.Windows.Forms.Button buttonInstall;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.LinkLabel linkLabelAuthor;
        private System.Windows.Forms.LinkLabel linkLabelInstallerAuthorGithub;
    }
}

