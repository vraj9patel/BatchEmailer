namespace BatchMailer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnChooseData;
        private System.Windows.Forms.TextBox txtDataPath;
        private System.Windows.Forms.Button btnChooseFolder;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grpSmtp;
        private System.Windows.Forms.TextBox txtSmtpHost;
        private System.Windows.Forms.TextBox txtSmtpPort;
        private System.Windows.Forms.TextBox txtSenderEmail;
        private System.Windows.Forms.TextBox txtSenderPass;
        private System.Windows.Forms.Label lblSuccess;
        private System.Windows.Forms.Label lblFailed;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblBody;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnChooseData = new Button();
            txtDataPath = new TextBox();
            btnChooseFolder = new Button();
            txtFolderPath = new TextBox();
            btnSend = new Button();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            grpSmtp = new GroupBox();
            txtSmtpHost = new TextBox();
            txtSmtpPort = new TextBox();
            txtSenderEmail = new TextBox();
            txtSenderPass = new TextBox();
            lblSuccess = new Label();
            lblFailed = new Label();
            btnPreview = new Button();
            lblSubject = new Label();
            txtSubject = new TextBox();
            lblBody = new Label();
            txtBody = new TextBox();
            grpSmtp.SuspendLayout();
            SuspendLayout();
            // 
            // btnChooseData
            // 
            btnChooseData.Location = new Point(12, 12);
            btnChooseData.Name = "btnChooseData";
            btnChooseData.Size = new Size(120, 28);
            btnChooseData.TabIndex = 4;
            btnChooseData.Text = "Select CSV/XLSX";
            btnChooseData.Click += BtnChooseData_Click;
            // 
            // txtDataPath
            // 
            txtDataPath.Location = new Point(138, 14);
            txtDataPath.Name = "txtDataPath";
            txtDataPath.ReadOnly = true;
            txtDataPath.Size = new Size(540, 23);
            txtDataPath.TabIndex = 5;
            // 
            // btnChooseFolder
            // 
            btnChooseFolder.Location = new Point(12, 52);
            btnChooseFolder.Name = "btnChooseFolder";
            btnChooseFolder.Size = new Size(120, 28);
            btnChooseFolder.TabIndex = 6;
            btnChooseFolder.Text = "Select Attachments Folder";
            btnChooseFolder.Click += BtnChooseFolder_Click;
            // 
            // txtFolderPath
            // 
            txtFolderPath.Location = new Point(138, 54);
            txtFolderPath.Name = "txtFolderPath";
            txtFolderPath.ReadOnly = true;
            txtFolderPath.Size = new Size(540, 23);
            txtFolderPath.TabIndex = 7;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(510, 412);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(168, 30);
            btnSend.TabIndex = 10;
            btnSend.Text = "Send Emails";
            btnSend.Click += BtnSend_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 448);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(666, 22);
            progressBar.TabIndex = 11;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(24, 473);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(666, 20);
            lblStatus.TabIndex = 12;
            lblStatus.Text = "Ready";
            // 
            // grpSmtp
            // 
            grpSmtp.Controls.Add(txtSmtpHost);
            grpSmtp.Controls.Add(txtSmtpPort);
            grpSmtp.Controls.Add(txtSenderEmail);
            grpSmtp.Controls.Add(txtSenderPass);
            grpSmtp.Location = new Point(12, 92);
            grpSmtp.Name = "grpSmtp";
            grpSmtp.Size = new Size(666, 130);
            grpSmtp.TabIndex = 8;
            grpSmtp.TabStop = false;
            grpSmtp.Text = "SMTP Settings";
            // 
            // txtSmtpHost
            // 
            txtSmtpHost.Location = new Point(12, 22);
            txtSmtpHost.Name = "txtSmtpHost";
            txtSmtpHost.Size = new Size(300, 23);
            txtSmtpHost.TabIndex = 0;
            txtSmtpHost.Text = "smtp.gmail.com";
            // 
            // txtSmtpPort
            // 
            txtSmtpPort.Location = new Point(320, 22);
            txtSmtpPort.Name = "txtSmtpPort";
            txtSmtpPort.Size = new Size(60, 23);
            txtSmtpPort.TabIndex = 1;
            txtSmtpPort.Text = "587";
            // 
            // txtSenderEmail
            // 
            txtSenderEmail.Location = new Point(12, 58);
            txtSenderEmail.Name = "txtSenderEmail";
            txtSenderEmail.PlaceholderText = "Sender email";
            txtSenderEmail.Size = new Size(368, 23);
            txtSenderEmail.TabIndex = 2;
            // 
            // txtSenderPass
            // 
            txtSenderPass.Location = new Point(12, 92);
            txtSenderPass.Name = "txtSenderPass";
            txtSenderPass.PlaceholderText = "App password (or SMTP password)";
            txtSenderPass.Size = new Size(368, 23);
            txtSenderPass.TabIndex = 3;
            txtSenderPass.UseSystemPasswordChar = true;
            // 
            // lblSuccess
            // 
            lblSuccess.Location = new Point(24, 513);
            lblSuccess.Name = "lblSuccess";
            lblSuccess.Size = new Size(200, 20);
            lblSuccess.TabIndex = 13;
            lblSuccess.Text = "Success: 0";
            // 
            // lblFailed
            // 
            lblFailed.Location = new Point(230, 513);
            lblFailed.Name = "lblFailed";
            lblFailed.Size = new Size(200, 20);
            lblFailed.TabIndex = 14;
            lblFailed.Text = "Failed: 0";
            // 
            // btnPreview
            // 
            btnPreview.Location = new Point(12, 412);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(120, 30);
            btnPreview.TabIndex = 9;
            btnPreview.Text = "Preview (Dry Run)";
            btnPreview.Click += BtnPreview_Click;
            // 
            // lblSubject
            // 
            lblSubject.Location = new Point(12, 230);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(80, 20);
            lblSubject.TabIndex = 0;
            lblSubject.Text = "Subject:";
            // 
            // txtSubject
            // 
            txtSubject.Location = new Point(90, 228);
            txtSubject.Name = "txtSubject";
            txtSubject.Size = new Size(588, 23);
            txtSubject.TabIndex = 1;
            txtSubject.Text = "Your Photos";
            // 
            // lblBody
            // 
            lblBody.Location = new Point(12, 260);
            lblBody.Name = "lblBody";
            lblBody.Size = new Size(80, 20);
            lblBody.TabIndex = 2;
            lblBody.Text = "Body:";
            // 
            // txtBody
            // 
            txtBody.Location = new Point(90, 260);
            txtBody.Multiline = true;
            txtBody.Name = "txtBody";
            txtBody.Size = new Size(588, 130);
            txtBody.TabIndex = 3;
            txtBody.Text = "Jay Swaminarayan {FirstName} {LastName},\r\n\r\nPlease find your photos attached.\r\n\r\nBest regards,";
            // 
            // MainForm
            // 
            ClientSize = new Size(770, 575);
            Controls.Add(lblSubject);
            Controls.Add(txtSubject);
            Controls.Add(lblBody);
            Controls.Add(txtBody);
            Controls.Add(btnChooseData);
            Controls.Add(txtDataPath);
            Controls.Add(btnChooseFolder);
            Controls.Add(txtFolderPath);
            Controls.Add(grpSmtp);
            Controls.Add(btnPreview);
            Controls.Add(btnSend);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(lblSuccess);
            Controls.Add(lblFailed);
            Name = "MainForm";
            Text = "Batch Mailer";
            grpSmtp.ResumeLayout(false);
            grpSmtp.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }
    }
}
