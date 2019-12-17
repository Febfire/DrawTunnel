namespace UI.Forms
{
    partial class DisplayModeForm
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
            this.LModeButton = new System.Windows.Forms.Button();
            this.DLModeButton = new System.Windows.Forms.Button();
            this.RModeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LModeButton
            // 
            this.LModeButton.Location = new System.Drawing.Point(12, 12);
            this.LModeButton.Name = "LModeButton";
            this.LModeButton.Size = new System.Drawing.Size(75, 23);
            this.LModeButton.TabIndex = 2;
            this.LModeButton.Text = "单线模式";
            this.LModeButton.UseVisualStyleBackColor = true;
            this.LModeButton.Click += new System.EventHandler(this.LModeButton_Click);
            // 
            // DLModeButton
            // 
            this.DLModeButton.Location = new System.Drawing.Point(93, 12);
            this.DLModeButton.Name = "DLModeButton";
            this.DLModeButton.Size = new System.Drawing.Size(75, 23);
            this.DLModeButton.TabIndex = 3;
            this.DLModeButton.Text = "双线模式";
            this.DLModeButton.UseVisualStyleBackColor = true;
            this.DLModeButton.Click += new System.EventHandler(this.DLModeButton_Click);
            // 
            // RModeButton
            // 
            this.RModeButton.Location = new System.Drawing.Point(174, 12);
            this.RModeButton.Name = "RModeButton";
            this.RModeButton.Size = new System.Drawing.Size(75, 23);
            this.RModeButton.TabIndex = 4;
            this.RModeButton.Text = "三维模式";
            this.RModeButton.UseVisualStyleBackColor = true;
            this.RModeButton.Click += new System.EventHandler(this.RModeButton_Click);
            // 
            // DisplayModeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(258, 47);
            this.Controls.Add(this.RModeButton);
            this.Controls.Add(this.DLModeButton);
            this.Controls.Add(this.LModeButton);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(274, 86);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(274, 86);
            this.Name = "DisplayModeForm";
            this.ShowIcon = false;
            this.Text = "显示模式";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button LModeButton;
        private System.Windows.Forms.Button DLModeButton;
        private System.Windows.Forms.Button RModeButton;
    }
}