namespace TESTS
{
    partial class admin_panel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.названиеТестаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.добавитьВопросToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.аФЫАБФАБАБАБАБToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Question = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Answers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrueAnswer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Edit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Question,
            this.Answers,
            this.TrueAnswer,
            this.Edit,
            this.Delete});
            this.dataGridView1.Location = new System.Drawing.Point(12, 53);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1344, 697);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.названиеТестаToolStripMenuItem,
            this.добавитьВопросToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1368, 35);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // названиеТестаToolStripMenuItem
            // 
            this.названиеТестаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.аФЫАБФАБАБАБАБToolStripMenuItem,
            this.аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem});
            this.названиеТестаToolStripMenuItem.Name = "названиеТестаToolStripMenuItem";
            this.названиеТестаToolStripMenuItem.Size = new System.Drawing.Size(196, 31);
            this.названиеТестаToolStripMenuItem.Text = "Название теста";
            // 
            // добавитьВопросToolStripMenuItem
            // 
            this.добавитьВопросToolStripMenuItem.Name = "добавитьВопросToolStripMenuItem";
            this.добавитьВопросToolStripMenuItem.Size = new System.Drawing.Size(209, 31);
            this.добавитьВопросToolStripMenuItem.Text = "Добавить вопрос";
            // 
            // аФЫАБФАБАБАБАБToolStripMenuItem
            // 
            this.аФЫАБФАБАБАБАБToolStripMenuItem.Name = "аФЫАБФАБАБАБАБToolStripMenuItem";
            this.аФЫАБФАБАБАБАБToolStripMenuItem.Size = new System.Drawing.Size(444, 32);
            this.аФЫАБФАБАБАБАБToolStripMenuItem.Text = "АФЫАБФАБАБАБАБ";
            // 
            // аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem
            // 
            this.аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem.Name = "аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem";
            this.аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem.Size = new System.Drawing.Size(444, 32);
            this.аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem.Text = "АБАБАБАБАБАБАБАББАБАБАБА";
            // 
            // Question
            // 
            this.Question.HeaderText = "Вопрос";
            this.Question.Name = "Question";
            this.Question.Width = 500;
            // 
            // Answers
            // 
            this.Answers.HeaderText = "Ответы";
            this.Answers.Name = "Answers";
            this.Answers.Width = 400;
            // 
            // TrueAnswer
            // 
            this.TrueAnswer.HeaderText = "Правильный ответ";
            this.TrueAnswer.Name = "TrueAnswer";
            this.TrueAnswer.Width = 200;
            // 
            // Edit
            // 
            this.Edit.HeaderText = "Изменить";
            this.Edit.Name = "Edit";
            this.Edit.Text = "Изменить";
            this.Edit.ToolTipText = "Изменить";
            this.Edit.UseColumnTextForButtonValue = true;
            // 
            // Delete
            // 
            this.Delete.HeaderText = "Удалить";
            this.Delete.Name = "Delete";
            this.Delete.Text = "Удалить";
            this.Delete.ToolTipText = "Удалить";
            this.Delete.UseColumnTextForButtonValue = true;
            // 
            // admin_panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1368, 762);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.dataGridView1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "admin_panel";
            this.Text = "admin_panel";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem названиеТестаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem добавитьВопросToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem аФЫАБФАБАБАБАБToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem аБАБАБАБАБАБАБАББАБАБАБАToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Question;
        private System.Windows.Forms.DataGridViewTextBoxColumn Answers;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrueAnswer;
        private System.Windows.Forms.DataGridViewButtonColumn Edit;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
    }
}