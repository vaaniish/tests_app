using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TESTS
{
    public partial class start : Form
    {
        private List<Test> tests = new List<Test>();
        private float uiScale = 1f;

        public start()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(920, 540);
            WindowState = FormWindowState.Maximized;
            richTextBox1.ShortcutsEnabled = false;
            richTextBox1.SelectionChanged += richTextBox1_SelectionChanged;
            Load += Start_Load;
            Resize += Start_Resize;
            Shown += Start_Shown;

            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void Start_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            LoadTests();
            BindComboBox();
        }

        private void LoadTests()
        {
            try
            {
                tests = TestStorage.LoadOrCreateDefaultTests(
                    AppDomain.CurrentDomain.BaseDirectory
                );
            }
            catch (Exception ex)
            {
                tests = new List<Test>();
                MessageBox.Show("Ошибка загрузки тестов:\n" + ex.Message);
            }
        }

        private void BindComboBox()
        {
            comboBoxTests.DataSource = null;
            comboBoxTests.DisplayMember = "Title";
            comboBoxTests.DataSource = tests;
            UpdateComboDropDownWidth();

            if (tests.Count > 0)
                comboBoxTests.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedTest = comboBoxTests.SelectedItem as Test;
            if (selectedTest == null)
            {
                MessageBox.Show("Выберите тест");
                return;
            }

            using (var testForm = new test_panel(selectedTest))
            {
                Hide();
                testForm.ShowDialog(this);
                Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var adminForm = new admin_login())
            {
                Hide();
                adminForm.ShowDialog(this);
                Show();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.SelectionLength = 0;
            }
        }

        private void comboBoxTests_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var selectedTest = comboBoxTests.SelectedItem as Test;

            if (selectedTest == null)
            {
                richTextBox1.Clear();
                return;
            }

            richTextBox1.Text = BuildMainScreenDescription(selectedTest);
        }

        private void Start_Resize(object sender, EventArgs e)
        {
            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            var margin = UiTheme.ScalePx(12, uiScale);
            var gap = UiTheme.ScalePx(8, uiScale);
            var comboHeight = UiTheme.ScalePx(36, uiScale);
            comboBoxTests.SetBounds(
                margin,
                margin,
                Math.Max(UiTheme.ScalePx(360, uiScale), ClientSize.Width - margin * 2),
                comboHeight
            );
            UpdateComboDropDownWidth();

            var contentTop = comboBoxTests.Bottom + gap;
            var contentHeight = Math.Max(UiTheme.ScalePx(240, uiScale), ClientSize.Height - contentTop - margin);

            var rightWidth = Clamp(
                (int)(ClientSize.Width * 0.24f),
                UiTheme.ScalePx(280, uiScale),
                UiTheme.ScalePx(440, uiScale)
            );
            var maxRightByLayout = ClientSize.Width - margin * 2 - gap - UiTheme.ScalePx(340, uiScale);
            rightWidth = Clamp(rightWidth, UiTheme.ScalePx(240, uiScale), Math.Max(UiTheme.ScalePx(240, uiScale), maxRightByLayout));

            var rightX = ClientSize.Width - margin - rightWidth;
            var buttonMin = UiTheme.ScalePx(90, uiScale);
            var buttonMax = UiTheme.ScalePx(190, uiScale);
            var buttonHeight = Math.Min(buttonMax, Math.Max(buttonMin, (contentHeight - gap) / 2));

            button1.SetBounds(rightX, contentTop, rightWidth, buttonHeight);
            button2.SetBounds(rightX, button1.Bottom + gap, rightWidth, buttonHeight);

            var leftWidth = Math.Max(UiTheme.ScalePx(340, uiScale), rightX - margin - gap);
            richTextBox1.SetBounds(margin, contentTop, leftWidth, contentHeight);
        }

        private void UpdateComboDropDownWidth()
        {
            var width = comboBoxTests.Width;
            using (var g = comboBoxTests.CreateGraphics())
            {
                foreach (var test in tests)
                {
                    var measured = TextRenderer.MeasureText(g, test.Title ?? string.Empty, comboBoxTests.Font).Width + 40;
                    if (measured > width)
                        width = measured;
                }
            }

            var screen = Screen.FromControl(this).WorkingArea;
            var max = Math.Max(comboBoxTests.Width, screen.Width - UiTheme.ScalePx(24, uiScale));
            comboBoxTests.DropDownWidth = Math.Min(Math.Max(comboBoxTests.Width, width), max);
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(1133, 500));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StyleInput(comboBoxTests, uiScale);
            UiTheme.StyleInput(richTextBox1, uiScale);
            UiTheme.StylePrimaryButton(button1, uiScale);
            UiTheme.StyleSecondaryButton(button2, uiScale);
        }

        private static string BuildMainScreenDescription(Test selectedTest)
        {
            if (selectedTest == null)
                return string.Empty;

            var title = (selectedTest.Title ?? string.Empty).Trim();
            var description = (selectedTest.Description ?? string.Empty).Replace("\r\n", "\n").Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                var attempt = Math.Min(20, Math.Max(1, selectedTest.Questions.Count));
                return "Время: " + attempt + " мин. (1 вопрос = 1 мин.)\n" +
                       "В банке: " + selectedTest.Questions.Count + "\n" +
                       "В попытке: " + attempt;
            }

            var lines = new List<string>();
            foreach (var line in description.Split('\n'))
            {
                lines.Add((line ?? string.Empty).TrimEnd());
            }

            // Убираем дубли названия теста в начале описания.
            while (true)
            {
                var firstNonEmptyIndex = -1;
                for (var i = 0; i < lines.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        firstNonEmptyIndex = i;
                        break;
                    }
                }

                if (firstNonEmptyIndex < 0)
                    break;

                var firstLine = lines[firstNonEmptyIndex].Trim();
                if (!string.Equals(firstLine, title, StringComparison.OrdinalIgnoreCase))
                    break;

                lines.RemoveAt(firstNonEmptyIndex);
            }

            // Убираем служебную строку версии банка из текста для студента/преподавателя.
            lines = lines
                .Where(line =>
                {
                    var text = (line ?? string.Empty).TrimStart();
                    return !text.StartsWith("Версия банка:", StringComparison.OrdinalIgnoreCase);
                })
                .ToList();

            return string.Join("\n", lines).Trim();
        }

        private static int Clamp(int value, int min, int max)
        {
            if (max < min)
                max = min;

            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}
