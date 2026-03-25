using System;
using System.Drawing;
using System.Windows.Forms;

namespace TESTS
{
    public sealed class TestEditorForm : Form
    {
        private readonly TextBox textTitle;
        private readonly TextBox textDescription;
        private readonly NumericUpDown numericTime;
        private readonly Button buttonOk;
        private readonly Button buttonCancel;
        private readonly FlowLayoutPanel footer;
        private float uiScale = 1f;

        public string TestTitle { get; private set; }
        public string TestDescription { get; private set; }
        public int TimeMinutes { get; private set; }

        public TestEditorForm(Test source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            Text = "Редактирование теста";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(700, 360);
            MinimumSize = new Size(620, 320);
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = true;
            MaximizeBox = true;
            Resize += TestEditorForm_Resize;
            Shown += TestEditorForm_Shown;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(12)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(panel);

            panel.Controls.Add(new Label { Text = "Название", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 0);
            textTitle = new TextBox { Dock = DockStyle.Fill, Text = source.Title ?? string.Empty };
            panel.Controls.Add(textTitle, 1, 0);

            panel.Controls.Add(new Label { Text = "Описание", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 1);
            textDescription = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = source.Description ?? string.Empty
            };
            panel.Controls.Add(textDescription, 1, 1);

            panel.Controls.Add(new Label { Text = "Время (мин)", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 2);
            numericTime = new NumericUpDown
            {
                Minimum = 5,
                Maximum = 180,
                Value = Math.Max(5, source.TimeMinutes > 0 ? source.TimeMinutes : 20),
                Width = 120
            };
            panel.Controls.Add(numericTime, 1, 2);

            footer = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(8, 10, 8, 10)
            };
            buttonOk = new Button
            {
                Text = "Сохранить",
                Width = 190,
                Height = 42
            };
            buttonCancel = new Button
            {
                Text = "Отменить и закрыть",
                Width = 190,
                Height = 42,
                DialogResult = DialogResult.Cancel
            };
            buttonOk.Click += buttonOk_Click;
            footer.Controls.Add(buttonOk);
            footer.Controls.Add(buttonCancel);
            Controls.Add(footer);

            AcceptButton = buttonOk;
            CancelButton = buttonCancel;
            ApplyAdaptiveTypography();
        }

        private void TestEditorForm_Shown(object sender, EventArgs e)
        {
            UiTheme.EnsureFormFitsOnScreen(this, center: true);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var title = (textTitle.Text ?? string.Empty).Trim();
            var description = (textDescription.Text ?? string.Empty).Trim();

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

            TestTitle = title;
            TestDescription = description;
            TimeMinutes = (int)numericTime.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void TestEditorForm_Resize(object sender, EventArgs e)
        {
            ApplyAdaptiveTypography();
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(700, 360));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StyleInput(textTitle, uiScale);
            UiTheme.StyleInput(textDescription, uiScale);
            UiTheme.StyleInput(numericTime, uiScale);
            UiTheme.StylePrimaryButton(buttonOk, uiScale);
            UiTheme.StyleSecondaryButton(buttonCancel, uiScale);

            var buttonHeight = UiTheme.ScalePx(42, uiScale);
            buttonOk.Height = buttonHeight;
            buttonCancel.Height = buttonHeight;

            var horizontalPadding = UiTheme.ScalePx(40, uiScale);
            var minButtonWidth = UiTheme.ScalePx(180, uiScale);
            buttonOk.Width = GetButtonWidthByText(buttonOk, horizontalPadding, minButtonWidth);
            buttonCancel.Width = GetButtonWidthByText(buttonCancel, horizontalPadding, minButtonWidth);
            footer.Height = UiTheme.ScalePx(74, uiScale);
        }

        private static int GetButtonWidthByText(Button button, int horizontalPadding, int minWidth)
        {
            var measuredWidth = TextRenderer.MeasureText(button.Text ?? string.Empty, button.Font).Width + horizontalPadding;
            return Math.Max(minWidth, measuredWidth);
        }
    }
}

