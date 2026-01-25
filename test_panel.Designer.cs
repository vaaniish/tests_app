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
            this.btnNext_Click = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // btnNext_Click
            // 
            this.btnNext_Click.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnNext_Click.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext_Click.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnNext_Click.Location = new System.Drawing.Point(12, 650);
            this.btnNext_Click.Name = "btnNext_Click";
            this.btnNext_Click.Size = new System.Drawing.Size(1004, 45);
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
            this.pnlContent.Location = new System.Drawing.Point(12, 12);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(15);
            this.pnlContent.Size = new System.Drawing.Size(1004, 632);
            this.pnlContent.TabIndex = 6;
            this.pnlContent.WrapContents = false;
            // 
            // test_panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 707);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.btnNext_Click);
            this.Name = "test_panel";
            this.Text = "test_panel";
            this.Load += new System.EventHandler(this.test_panel_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnNext_Click;
        private System.Windows.Forms.FlowLayoutPanel pnlContent;
    }
}