namespace DesktopPdfSigner
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtUsbDonglePassword = new System.Windows.Forms.TextBox();
            this.btnSign = new System.Windows.Forms.Button();
            this.bckWorker = new System.ComponentModel.BackgroundWorker();
            this.chBoxPassword = new System.Windows.Forms.CheckBox();
            this.btnFileUpload = new System.Windows.Forms.Button();
            this.fileUpload = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Usb-Dongle Şifreniz :";
            // 
            // txtUsbDonglePassword
            // 
            this.txtUsbDonglePassword.Location = new System.Drawing.Point(113, 18);
            this.txtUsbDonglePassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtUsbDonglePassword.Name = "txtUsbDonglePassword";
            this.txtUsbDonglePassword.PasswordChar = '*';
            this.txtUsbDonglePassword.Size = new System.Drawing.Size(131, 20);
            this.txtUsbDonglePassword.TabIndex = 1;
            // 
            // btnSign
            // 
            this.btnSign.Location = new System.Drawing.Point(14, 58);
            this.btnSign.Margin = new System.Windows.Forms.Padding(2);
            this.btnSign.Name = "btnSign";
            this.btnSign.Size = new System.Drawing.Size(230, 34);
            this.btnSign.TabIndex = 2;
            this.btnSign.Text = "İmzala";
            this.btnSign.UseVisualStyleBackColor = true;
            this.btnSign.Click += new System.EventHandler(this.btnSign_Click);
            // 
            // chBoxPassword
            // 
            this.chBoxPassword.AutoSize = true;
            this.chBoxPassword.Location = new System.Drawing.Point(249, 20);
            this.chBoxPassword.Name = "chBoxPassword";
            this.chBoxPassword.Size = new System.Drawing.Size(15, 14);
            this.chBoxPassword.TabIndex = 3;
            this.chBoxPassword.UseVisualStyleBackColor = true;
            this.chBoxPassword.CheckedChanged += new System.EventHandler(this.chBoxPassword_CheckedChanged);
            // 
            // btnFileUpload
            // 
            this.btnFileUpload.Location = new System.Drawing.Point(14, 97);
            this.btnFileUpload.Name = "btnFileUpload";
            this.btnFileUpload.Size = new System.Drawing.Size(230, 34);
            this.btnFileUpload.TabIndex = 4;
            this.btnFileUpload.Text = "Dosya Yükle";
            this.btnFileUpload.UseVisualStyleBackColor = true;
            this.btnFileUpload.Click += new System.EventHandler(this.btnFileUpload_Click);
            // 
            // fileUpload
            // 
            this.fileUpload.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 146);
            this.Controls.Add(this.btnFileUpload);
            this.Controls.Add(this.chBoxPassword);
            this.Controls.Add(this.btnSign);
            this.Controls.Add(this.txtUsbDonglePassword);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "PDF İmzala";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUsbDonglePassword;
        private System.Windows.Forms.Button btnSign;
        private System.ComponentModel.BackgroundWorker bckWorker;
        private System.Windows.Forms.CheckBox chBoxPassword;
        private System.Windows.Forms.Button btnFileUpload;
        private System.Windows.Forms.OpenFileDialog fileUpload;
    }
}

