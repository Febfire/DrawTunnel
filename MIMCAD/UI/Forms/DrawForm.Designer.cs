namespace UI.Forms
{
    partial class DrawForm
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
            this.DrawButton = new DevExpress.XtraEditors.SimpleButton();
            this.typeEditLabel = new DevExpress.XtraEditors.LabelControl();
            this.TypeEdit = new DevExpress.XtraEditors.ComboBoxEdit();
            this.nameEditLabel = new DevExpress.XtraEditors.LabelControl();
            this.nameEdit = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.TypeEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // DrawButton
            // 
            this.DrawButton.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.DrawButton.Location = new System.Drawing.Point(150, 174);
            this.DrawButton.Name = "DrawButton";
            this.DrawButton.Size = new System.Drawing.Size(75, 23);
            this.DrawButton.TabIndex = 21;
            this.DrawButton.Text = "绘制";
            this.DrawButton.Click += new System.EventHandler(this.DrawButton_Click);
            // 
            // typeEditLabel
            // 
            this.typeEditLabel.Location = new System.Drawing.Point(22, 57);
            this.typeEditLabel.Name = "typeEditLabel";
            this.typeEditLabel.Size = new System.Drawing.Size(24, 14);
            this.typeEditLabel.TabIndex = 23;
            this.typeEditLabel.Text = "类型";
            // 
            // TypeEdit
            // 
            this.TypeEdit.Location = new System.Drawing.Point(74, 54);
            this.TypeEdit.Name = "TypeEdit";
            this.TypeEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.TypeEdit.Properties.DropDownRows = 3;
            this.TypeEdit.Properties.Items.AddRange(new object[] {
            "方形巷道",
            "圆形巷道"});
            this.TypeEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.TypeEdit.Size = new System.Drawing.Size(100, 20);
            this.TypeEdit.TabIndex = 22;
            // 
            // nameEditLabel
            // 
            this.nameEditLabel.Location = new System.Drawing.Point(22, 12);
            this.nameEditLabel.Name = "nameEditLabel";
            this.nameEditLabel.Size = new System.Drawing.Size(24, 14);
            this.nameEditLabel.TabIndex = 24;
            this.nameEditLabel.Text = "名称";
            // 
            // nameEdit
            // 
            this.nameEdit.Location = new System.Drawing.Point(74, 9);
            this.nameEdit.Name = "nameEdit";
            this.nameEdit.Size = new System.Drawing.Size(100, 20);
            this.nameEdit.TabIndex = 25;
            // 
            // DrawForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 242);
            this.Controls.Add(this.nameEdit);
            this.Controls.Add(this.nameEditLabel);
            this.Controls.Add(this.typeEditLabel);
            this.Controls.Add(this.TypeEdit);
            this.Controls.Add(this.DrawButton);
            this.Name = "DrawForm";
            this.Text = "DrawForm";
            ((System.ComponentModel.ISupportInitialize)(this.TypeEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nameEdit.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton DrawButton;
        private DevExpress.XtraEditors.LabelControl typeEditLabel;
        private DevExpress.XtraEditors.ComboBoxEdit TypeEdit;
        private DevExpress.XtraEditors.LabelControl nameEditLabel;
        private DevExpress.XtraEditors.TextEdit nameEdit;
    }
}