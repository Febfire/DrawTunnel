namespace UI.Forms
{
    partial class AnimateForm
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
            this.StartButton = new System.Windows.Forms.Button();
            this.EndButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(25, 12);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(86, 23);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "风向动画开始";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // EndButton
            // 
            this.EndButton.Location = new System.Drawing.Point(138, 12);
            this.EndButton.Name = "EndButton";
            this.EndButton.Size = new System.Drawing.Size(85, 23);
            this.EndButton.TabIndex = 2;
            this.EndButton.Text = "风向动画关";
            this.EndButton.UseVisualStyleBackColor = true;
            this.EndButton.Click += new System.EventHandler(this.EndButton_Click);
            // 
            // AnimateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 47);
            this.Controls.Add(this.EndButton);
            this.Controls.Add(this.StartButton);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(271, 86);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(271, 86);
            this.Name = "AnimateForm";
            this.ShowIcon = false;
            this.Text = "风向动画";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AnimateForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button EndButton;
        private System.Windows.Forms.Timer timer1;
    }
}