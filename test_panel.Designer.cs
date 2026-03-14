namespace TESTS
{
    partial class test_panel
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
            this.btnNext_Click = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.FlowLayoutPanel();
            this.btnBack = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.labelTimer = new System.Windows.Forms.Label();
            this.labelQNum = new System.Windows.Forms.Label();
            this.buttonFinishEarly = new System.Windows.Forms.Button();
            this.groupNavigation = new System.Windows.Forms.GroupBox();
            this.panelNavigation = new System.Windows.Forms.FlowLayoutPanel();
            this.groupNavigation.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnNext_Click
            // 
            this.btnNext_Click.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnNext_Click.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext_Click.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnNext_Click.Location = new System.Drawing.Point(524, 650);
            this.btnNext_Click.Name = "btnNext_Click";
            this.btnNext_Click.Size = new System.Drawing.Size(492, 45);
            this.btnNext_Click.TabIndex = 5;
            this.btnNext_Click.Text = "Следующий вопрос";
            this.btnNext_Click.UseVisualStyleBackColor = false;
            this.btnNext_Click.Click += new System.EventHandler(this.btnNext_Click_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.AutoScroll = true;
            this.pnlContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlContent.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlContent.Location = new System.Drawing.Point(12, 64);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(15);
            this.pnlContent.Size = new System.Drawing.Size(1004, 580);
            this.pnlContent.TabIndex = 6;
            this.pnlContent.WrapContents = false;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnBack.Location = new System.Drawing.Point(12, 650);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(506, 45);
            this.btnBack.TabIndex = 7;
            this.btnBack.Text = "Вернуться назад";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(667, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 32);
            this.label1.TabIndex = 8;
            this.label1.Text = "Времени осталось:";
            // 
            // labelTimer
            // 
            this.labelTimer.AutoSize = true;
            this.labelTimer.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTimer.Location = new System.Drawing.Point(930, 17);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(86, 32);
            this.labelTimer.TabIndex = 9;
            this.labelTimer.Text = "label2";
            // 
            // labelQNum
            // 
            this.labelQNum.AutoSize = true;
            this.labelQNum.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelQNum.Location = new System.Drawing.Point(12, 17);
            this.labelQNum.Name = "labelQNum";
            this.labelQNum.Size = new System.Drawing.Size(86, 32);
            this.labelQNum.TabIndex = 10;
            this.labelQNum.Text = "label2";
            // 
            // buttonFinishEarly
            // 
            this.buttonFinishEarly.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(81)))), ((int)(((byte)(81)))));
            this.buttonFinishEarly.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFinishEarly.Font = new System.Drawing.Font("Arial Black", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonFinishEarly.ForeColor = System.Drawing.Color.White;
            this.buttonFinishEarly.Location = new System.Drawing.Point(18, 518);
            this.buttonFinishEarly.Name = "buttonFinishEarly";
            this.buttonFinishEarly.Size = new System.Drawing.Size(218, 45);
            this.buttonFinishEarly.TabIndex = 11;
            this.buttonFinishEarly.Text = "Завершить досрочно";
            this.buttonFinishEarly.UseVisualStyleBackColor = false;
            this.buttonFinishEarly.Click += new System.EventHandler(this.buttonFinishEarly_Click);
            // 
            // groupNavigation
            // 
            this.groupNavigation.Controls.Add(this.panelNavigation);
            this.groupNavigation.Location = new System.Drawing.Point(18, 64);
            this.groupNavigation.Name = "groupNavigation";
            this.groupNavigation.Size = new System.Drawing.Size(998, 122);
            this.groupNavigation.TabIndex = 12;
            this.groupNavigation.TabStop = false;
            this.groupNavigation.Text = "Навигация по тесту";
            // 
            // panelNavigation
            // 
            this.panelNavigation.AutoScroll = true;
            this.panelNavigation.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelNavigation.Location = new System.Drawing.Point(10, 24);
            this.panelNavigation.Name = "panelNavigation";
            this.panelNavigation.Size = new System.Drawing.Size(982, 92);
            this.panelNavigation.TabIndex = 0;
            this.panelNavigation.WrapContents = true;
            // 
            // test_panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 707);
            this.Controls.Add(this.groupNavigation);
            this.Controls.Add(this.buttonFinishEarly);
            this.Controls.Add(this.labelQNum);
            this.Controls.Add(this.labelTimer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.btnNext_Click);
            this.Name = "test_panel";
            this.Text = "test_panel";
            this.Load += new System.EventHandler(this.test_panel_Load);
            this.groupNavigation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnNext_Click;
        private System.Windows.Forms.FlowLayoutPanel pnlContent;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTimer;
        private System.Windows.Forms.Label labelQNum;
        private System.Windows.Forms.Button buttonFinishEarly;
        private System.Windows.Forms.GroupBox groupNavigation;
        private System.Windows.Forms.FlowLayoutPanel panelNavigation;
    }
}
