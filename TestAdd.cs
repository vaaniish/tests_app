using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TESTS
{
    public partial class TestAdd : Form
    {
        public Test CreatedTest { get; private set; }
        private float uiScale = 1f;

        public TestAdd()
        {
            InitializeComponent();
            Text = "Добавление теста";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(420, 320);
            FormBorderStyle = FormBorderStyle.Sizable;
            Resize += TestAdd_Resize;
            Shown += TestAdd_Shown;
            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void TestAdd_Shown(object sender, EventArgs e)
        {
            UiTheme.EnsureFormFitsOnScreen(this, center: true);
        }

        private void TestAdd_Load(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var title = richTextBox1.Text.Trim();
            var description = richTextBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название теста");
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Введите описание теста");
                return;
            }

            CreatedTest = new Test
            {
                Id = "test-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                Title = title,
                Description = description,
                TimeMinutes = 20,
                Questions = new List<Question>()
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void TestAdd_Resize(object sender, EventArgs e)
        {
            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            var margin = UiTheme.ScalePx(16, uiScale);
            var gap = UiTheme.ScalePx(8, uiScale);
            var width = Math.Max(UiTheme.ScalePx(220, uiScale), ClientSize.Width - margin * 2);

            label1.Location = new Point(margin, margin);
            richTextBox1.SetBounds(margin, label1.Bottom + 4, width, UiTheme.ScalePx(38, uiScale));

            label2.Location = new Point(margin, richTextBox1.Bottom + gap);
            var descHeight = Math.Max(UiTheme.ScalePx(70, uiScale), ClientSize.Height - label2.Bottom - UiTheme.ScalePx(72, uiScale));
            richTextBox2.SetBounds(margin, label2.Bottom + 4, width, descHeight);

            button1.SetBounds(margin, richTextBox2.Bottom + gap, width, UiTheme.ScalePx(44, uiScale));
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(420, 320));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StyleInput(richTextBox1, uiScale);
            UiTheme.StyleInput(richTextBox2, uiScale);
            UiTheme.StylePrimaryButton(button1, uiScale);
            UiTheme.StyleTitleLabel(label1, uiScale);
            UiTheme.StyleTitleLabel(label2, uiScale);
        }
    }
}
