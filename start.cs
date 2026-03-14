using System;
using System.Collections.Generic;
using System.Drawing;
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
            ApplyAdaptiveTypography();
            richTextBox1.ShortcutsEnabled = false;
            richTextBox1.SelectionChanged += richTextBox1_SelectionChanged;
            Load += Start_Load;
            Resize += Start_Resize;
            Shown += Start_Shown;
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

            richTextBox1.Text =
                selectedTest.Title + "\n\n" +
                selectedTest.Description + "\n\n" +
                "Время: " + selectedTest.TimeMinutes + " мин.\n" +
                "В банке: " + selectedTest.Questions.Count + "\n" +
                "В попытке: 20";
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
            var leftMinWidth = UiTheme.ScalePx(360, uiScale);
            var rightMinWidth = UiTheme.ScalePx(320, uiScale);
            var preferredRight = GetPreferredComboWidth();
            var rightMaxByRatio = (int)(ClientSize.Width * 0.50f);
            var rightMaxByPixels = UiTheme.ScalePx(760, uiScale);
            var rightMaxByLayout = ClientSize.Width - margin * 2 - gap - leftMinWidth;
            var rightMaxWidth = Math.Max(rightMinWidth, Math.Min(Math.Min(rightMaxByRatio, rightMaxByPixels), rightMaxByLayout));
            var rightWidth = Clamp(preferredRight, rightMinWidth, rightMaxWidth);
            var rightX = ClientSize.Width - rightWidth - margin;

            comboBoxTests.SetBounds(rightX, margin, rightWidth, UiTheme.ScalePx(36, uiScale));
            UpdateComboDropDownWidth();

            var buttonsTop = comboBoxTests.Bottom + gap;
            var availableHeight = ClientSize.Height - buttonsTop - margin;
            var buttonMin = UiTheme.ScalePx(90, uiScale);
            var buttonMax = UiTheme.ScalePx(190, uiScale);
            var buttonHeight = Math.Min(buttonMax, Math.Max(buttonMin, (availableHeight - gap) / 2));

            button1.SetBounds(rightX, buttonsTop, rightWidth, buttonHeight);
            button2.SetBounds(rightX, button1.Bottom + gap, rightWidth, buttonHeight);

            var leftWidth = Math.Max(UiTheme.ScalePx(360, uiScale), rightX - margin - gap);
            richTextBox1.SetBounds(margin, margin, leftWidth, ClientSize.Height - margin * 2);
        }

        private int GetPreferredComboWidth()
        {
            var width = UiTheme.ScalePx(320, uiScale);

            using (var g = comboBoxTests.CreateGraphics())
            {
                foreach (var test in tests)
                {
                    var measured = TextRenderer.MeasureText(g, test.Title ?? string.Empty, comboBoxTests.Font).Width + 40;
                    if (measured > width)
                        width = measured;
                }
            }

            return Math.Min(width, Math.Max(UiTheme.ScalePx(320, uiScale), (int)(ClientSize.Width * 0.50)));
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
            var comboLeftOnScreen = comboBoxTests.PointToScreen(Point.Empty).X;
            var availableToRight = screen.Right - comboLeftOnScreen - UiTheme.ScalePx(12, uiScale);
            var max = Math.Max(comboBoxTests.Width, availableToRight);
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
