namespace UI.Forms
{
    partial class TunnelContextForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TunnelContextForm));
            this.comboBox_ti = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.comboBox_pi = new System.Windows.Forms.ComboBox();
            this.textBox_px = new System.Windows.Forms.TextBox();
            this.textBox_py = new System.Windows.Forms.TextBox();
            this.textBox_pz = new System.Windows.Forms.TextBox();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_t = new DevExpress.XtraEditors.TextEdit();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.addButtom = new DevExpress.XtraBars.BarButtonItem();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textEdit3 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
            this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
            this.button2 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBox_t.Properties)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox_ti
            // 
            this.comboBox_ti.FormattingEnabled = true;
            this.comboBox_ti.Location = new System.Drawing.Point(60, 27);
            this.comboBox_ti.Name = "comboBox_ti";
            this.comboBox_ti.Size = new System.Drawing.Size(50, 20);
            this.comboBox_ti.TabIndex = 1;
            this.comboBox_ti.SelectedValueChanged += new System.EventHandler(this.comboBox_ti_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(143, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "温度：";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(322, 25);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 4;
            this.SaveButton.Text = "确定";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // comboBox_pi
            // 
            this.comboBox_pi.FormattingEnabled = true;
            this.comboBox_pi.Location = new System.Drawing.Point(66, 42);
            this.comboBox_pi.Name = "comboBox_pi";
            this.comboBox_pi.Size = new System.Drawing.Size(50, 20);
            this.comboBox_pi.TabIndex = 6;
            this.comboBox_pi.SelectedValueChanged += new System.EventHandler(this.comboBox_pi_SelectedValueChanged);
            // 
            // textBox_px
            // 
            this.textBox_px.Location = new System.Drawing.Point(160, 42);
            this.textBox_px.Name = "textBox_px";
            this.textBox_px.Size = new System.Drawing.Size(75, 21);
            this.textBox_px.TabIndex = 7;
            this.textBox_px.TextChanged += new System.EventHandler(this.textBox_px_TextChanged);
            // 
            // textBox_py
            // 
            this.textBox_py.Location = new System.Drawing.Point(241, 41);
            this.textBox_py.Name = "textBox_py";
            this.textBox_py.Size = new System.Drawing.Size(75, 21);
            this.textBox_py.TabIndex = 8;
            this.textBox_py.TextChanged += new System.EventHandler(this.textBox_py_TextChanged);
            // 
            // textBox_pz
            // 
            this.textBox_pz.Location = new System.Drawing.Point(322, 41);
            this.textBox_pz.Name = "textBox_pz";
            this.textBox_pz.Size = new System.Drawing.Size(75, 21);
            this.textBox_pz.TabIndex = 9;
            this.textBox_pz.TextChanged += new System.EventHandler(this.textBox_pz_TextChanged);
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 1;
            this.bar1.FloatLocation = new System.Drawing.Point(219, 346);
            this.bar1.Offset = 69;
            this.bar1.Text = "Tools";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 12);
            this.label3.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(275, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 9);
            this.label4.TabIndex = 11;
            this.label4.Text = "▲";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            this.label4.MouseEnter += new System.EventHandler(this.label4_MouseEnter);
            this.label4.MouseHover += new System.EventHandler(this.label4_MouseHover);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(275, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 9);
            this.label5.TabIndex = 12;
            this.label5.Text = "▼";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            this.label5.MouseEnter += new System.EventHandler(this.label5_MouseEnter);
            this.label5.MouseHover += new System.EventHandler(this.label5_MouseHover);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textBox_t);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.comboBox_ti);
            this.groupBox1.Controls.Add(this.SaveButton);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(407, 72);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "温度监测";
            // 
            // textBox_t
            // 
            this.textBox_t.Location = new System.Drawing.Point(198, 24);
            this.textBox_t.Name = "textBox_t";
            this.textBox_t.Properties.NullText = "1~100";
            this.textBox_t.Size = new System.Drawing.Size(75, 20);
            this.textBox_t.TabIndex = 15;
            this.textBox_t.EditValueChanged += new System.EventHandler(this.textBox_t_EditValueChanged);
            this.textBox_t.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_t_Validating);
            this.textBox_t.Validated += new System.EventHandler(this.textBox_t_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(11, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 14;
            this.label2.Text = "分段：";
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(0, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 100);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "巷道属性";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.textBox_px);
            this.groupBox3.Controls.Add(this.textBox_py);
            this.groupBox3.Controls.Add(this.textBox_pz);
            this.groupBox3.Controls.Add(this.comboBox_pi);
            this.groupBox3.Location = new System.Drawing.Point(1, 79);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(407, 122);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "巷道属性";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(322, 87);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "添加节点";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(131, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 19);
            this.label7.TabIndex = 11;
            this.label7.Text = "→";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(11, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "节点：";
            // 
            // addButtom
            // 
            this.addButtom.Caption = "添加巷道";
            this.addButtom.Id = 0;
            this.addButtom.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("addButtom.ImageOptions.Image")));
            this.addButtom.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("addButtom.ImageOptions.LargeImage")));
            this.addButtom.Name = "addButtom";
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "添加巷道";
            this.barButtonItem1.Id = 0;
            this.barButtonItem1.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.Image")));
            this.barButtonItem1.ImageOptions.LargeImage = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.ImageOptions.LargeImage")));
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textEdit3);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.textEdit2);
            this.groupBox4.Controls.Add(this.textEdit1);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Location = new System.Drawing.Point(1, 207);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(407, 126);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "新增节点";
            this.groupBox4.Visible = false;
            // 
            // textEdit3
            // 
            this.textEdit3.Location = new System.Drawing.Point(218, 51);
            this.textEdit3.Name = "textEdit3";
            this.textEdit3.Properties.NullText = "Z";
            this.textEdit3.Size = new System.Drawing.Size(100, 20);
            this.textEdit3.TabIndex = 6;
            // 
            // textEdit2
            // 
            this.textEdit2.Location = new System.Drawing.Point(112, 51);
            this.textEdit2.Name = "textEdit2";
            this.textEdit2.Properties.NullText = "Y";
            this.textEdit2.Size = new System.Drawing.Size(100, 20);
            this.textEdit2.TabIndex = 5;
            // 
            // textEdit1
            // 
            this.textEdit1.Location = new System.Drawing.Point(6, 51);
            this.textEdit1.Name = "textEdit1";
            this.textEdit1.Properties.NullText = "X";
            this.textEdit1.Size = new System.Drawing.Size(100, 20);
            this.textEdit1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(322, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "确定";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(115, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 21);
            this.label8.TabIndex = 13;
            this.label8.Text = "✚";
            this.toolTip1.SetToolTip(this.label8, "增加分段");
            this.label8.Click += new System.EventHandler(this.label8_Click);
            this.label8.MouseHover += new System.EventHandler(this.label8_MouseHover);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(146, 14);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 15;
            this.button3.Text = "确定";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(119, 51);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(53, 21);
            this.textBox1.TabIndex = 14;
            this.toolTip1.SetToolTip(this.textBox1, "输入分段数后回车");
            this.textBox1.Visible = false;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // TunnelContextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 316);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Name = "TunnelContextForm";
            this.Text = "巷道详细属性";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TunnelContextForm_FormClosed);
            this.Load += new System.EventHandler(this.TunnelContextForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBox_t.Properties)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEdit3.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox_ti;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ComboBox comboBox_pi;
        private System.Windows.Forms.TextBox textBox_px;
        private System.Windows.Forms.TextBox textBox_py;
        private System.Windows.Forms.TextBox textBox_pz;
        private DevExpress.XtraBars.Bar bar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private DevExpress.XtraBars.BarButtonItem addButtom;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.TextEdit textBox_t;
        private DevExpress.XtraEditors.TextEdit textEdit3;
        private DevExpress.XtraEditors.TextEdit textEdit2;
        private DevExpress.XtraEditors.TextEdit textEdit1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
    }
}