namespace HPMS
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.lboxLog = new System.Windows.Forms.ListBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnT100_3001_02 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTasol = new System.Windows.Forms.Button();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lboxLog
            // 
            this.lboxLog.FormattingEnabled = true;
            this.lboxLog.ItemHeight = 12;
            this.lboxLog.Location = new System.Drawing.Point(12, 248);
            this.lboxLog.Name = "lboxLog";
            this.lboxLog.Size = new System.Drawing.Size(761, 256);
            this.lboxLog.TabIndex = 26;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(679, 534);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(94, 32);
            this.btnExit.TabIndex = 25;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.btnT100_3001_02);
            this.groupBox4.Location = new System.Drawing.Point(27, -116);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(239, 53);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "공통";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(25, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 32);
            this.button1.TabIndex = 7;
            this.button1.Text = "T100_3001_00";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnT100_3001_02
            // 
            this.btnT100_3001_02.Location = new System.Drawing.Point(125, 15);
            this.btnT100_3001_02.Name = "btnT100_3001_02";
            this.btnT100_3001_02.Size = new System.Drawing.Size(94, 32);
            this.btnT100_3001_02.TabIndex = 5;
            this.btnT100_3001_02.Text = "T100_3001_02";
            this.btnT100_3001_02.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTasol);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(761, 207);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "공통";
            // 
            // btnTasol
            // 
            this.btnTasol.Location = new System.Drawing.Point(25, 15);
            this.btnTasol.Name = "btnTasol";
            this.btnTasol.Size = new System.Drawing.Size(94, 32);
            this.btnTasol.TabIndex = 7;
            this.btnTasol.Text = "타설량 변경";
            this.btnTasol.UseVisualStyleBackColor = true;
            this.btnTasol.Click += new System.EventHandler(this.btnTasol_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 575);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lboxLog);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.groupBox4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListBox lboxLog;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnT100_3001_02;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnTasol;
    }
}

