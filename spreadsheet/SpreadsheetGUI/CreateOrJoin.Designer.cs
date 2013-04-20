namespace SS
{
    partial class CreateOrJoin
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
            this.ipAddress = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.ssName = new System.Windows.Forms.TextBox();
            this.passWord = new System.Windows.Forms.TextBox();
            this.createButton = new System.Windows.Forms.Button();
            this.joinButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ipAddress
            // 
            this.ipAddress.Location = new System.Drawing.Point(179, 22);
            this.ipAddress.Name = "ipAddress";
            this.ipAddress.Size = new System.Drawing.Size(159, 20);
            this.ipAddress.TabIndex = 0;
            this.ipAddress.Text = "IP Address";
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(179, 48);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(159, 20);
            this.port.TabIndex = 1;
            this.port.Text = "Port";
            // 
            // ssName
            // 
            this.ssName.Location = new System.Drawing.Point(179, 74);
            this.ssName.Name = "ssName";
            this.ssName.Size = new System.Drawing.Size(159, 20);
            this.ssName.TabIndex = 2;
            this.ssName.Text = "Spreadsheet Name";
            // 
            // passWord
            // 
            this.passWord.Location = new System.Drawing.Point(179, 100);
            this.passWord.Name = "passWord";
            this.passWord.Size = new System.Drawing.Size(159, 20);
            this.passWord.TabIndex = 3;
            this.passWord.Text = "Password";
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(24, 18);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(116, 23);
            this.createButton.TabIndex = 4;
            this.createButton.Text = "Create Spreadsheet";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // joinButton
            // 
            this.joinButton.Location = new System.Drawing.Point(24, 44);
            this.joinButton.Name = "joinButton";
            this.joinButton.Size = new System.Drawing.Size(116, 23);
            this.joinButton.TabIndex = 5;
            this.joinButton.Text = "Join Spreadsheet";
            this.joinButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(47, 162);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // CreateOrJoin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 211);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.joinButton);
            this.Controls.Add(this.createButton);
            this.Controls.Add(this.passWord);
            this.Controls.Add(this.ssName);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ipAddress);
            this.Name = "CreateOrJoin";
            this.Text = "Create or Join a Spreadsheet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ipAddress;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.TextBox ssName;
        private System.Windows.Forms.TextBox passWord;
        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button joinButton;
        private System.Windows.Forms.Button cancelButton;
    }
}