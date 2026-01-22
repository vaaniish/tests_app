namespace TESTS
{
    partial class start
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.выборТестаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.тестПоТестуДляТестаНаТестеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.входДляПреподавателяToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.Window;
            this.menuStrip1.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выборТестаToolStripMenuItem,
            this.входДляПреподавателяToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(836, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // выборТестаToolStripMenuItem
            // 
            this.выборТестаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.тестПоТестуДляТестаНаТестеToolStripMenuItem,
            this.аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem});
            this.выборТестаToolStripMenuItem.Name = "выборТестаToolStripMenuItem";
            this.выборТестаToolStripMenuItem.Size = new System.Drawing.Size(160, 31);
            this.выборТестаToolStripMenuItem.Text = "Выбор теста";
            // 
            // тестПоТестуДляТестаНаТестеToolStripMenuItem
            // 
            this.тестПоТестуДляТестаНаТестеToolStripMenuItem.Name = "тестПоТестуДляТестаНаТестеToolStripMenuItem";
            this.тестПоТестуДляТестаНаТестеToolStripMenuItem.Size = new System.Drawing.Size(609, 32);
            this.тестПоТестуДляТестаНаТестеToolStripMenuItem.Text = "Тест по тесту для теста на тесте";
            // 
            // аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem
            // 
            this.аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem.Name = "аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem";
            this.аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem.Size = new System.Drawing.Size(609, 32);
            this.аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem.Text = "АБАБАБАБАБАБАБАБАБАБАБАЬААЬАЬАЬАБАБ";
            // 
            // входДляПреподавателяToolStripMenuItem
            // 
            this.входДляПреподавателяToolStripMenuItem.Name = "входДляПреподавателяToolStripMenuItem";
            this.входДляПреподавателяToolStripMenuItem.Size = new System.Drawing.Size(294, 31);
            this.входДляПреподавателяToolStripMenuItem.Text = "Вход для преподавателя";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 57);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(655, 280);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "Здесь должно быть описание теста";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Black", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(673, 57);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 280);
            this.button1.TabIndex = 2;
            this.button1.Text = "Начать тест";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 349);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "start";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem выборТестаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem тестПоТестуДляТестаНаТестеToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem аБАБАБАБАБАБАБАБАБАБАБАToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem входДляПреподавателяToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button button1;
    }
}

