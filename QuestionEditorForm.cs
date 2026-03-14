using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TESTS
{
    public sealed class QuestionEditorForm : Form
    {
        private readonly Question originalQuestion;
        private readonly bool hasStoredHiddenAnswer;
        private readonly QuestionType originalType;
        private readonly List<string> originalOptions;

        private readonly TextBox textQuestion;
        private readonly ComboBox comboType;
        private readonly TextBox textOptions;
        private readonly Button buttonChooseAnswer;
        private readonly Button buttonShowAnswer;
        private readonly Label labelAnswerState;
        private readonly Button buttonOk;
        private readonly Button buttonCancel;
        private readonly FlowLayoutPanel footerPanel;

        private string selectedAnswer;
        private float uiScale = 1f;

        public string QuestionText { get; private set; }
        public QuestionType QuestionType { get; private set; }
        public List<string> Options { get; private set; }
        public string Answer { get; private set; }
        public bool KeepExistingStoredAnswer { get; private set; }

        public QuestionEditorForm(Question question)
        {
            originalQuestion = question ?? throw new ArgumentNullException(nameof(question));
            hasStoredHiddenAnswer =
                string.IsNullOrWhiteSpace(question.Answer) &&
                (!string.IsNullOrWhiteSpace(question.AnswerEncrypted) ||
                 !string.IsNullOrWhiteSpace(question.AnswerHash));
            originalType = question.Type;
            originalOptions = NormalizeOptions(question.Options);
            selectedAnswer = question.Answer ?? string.Empty;

            Text = "Редактирование вопроса";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(860, 540);
            MinimumSize = new Size(760, 470);
            FormBorderStyle = FormBorderStyle.Sizable;
            Resize += QuestionEditorForm_Resize;
            Shown += QuestionEditorForm_Shown;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(12)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            Controls.Add(panel);

            panel.Controls.Add(new Label { Text = "Текст вопроса", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 0);
            textQuestion = new TextBox { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            UiTheme.StyleInput(textQuestion);
            textQuestion.Text = question.Text ?? string.Empty;
            panel.Controls.Add(textQuestion, 1, 1);

            panel.Controls.Add(new Label { Text = "Тип вопроса", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 2);
            comboType = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UiTheme.StyleInput(comboType);
            comboType.Items.AddRange(new object[] { "Один вариант", "Несколько вариантов", "Текстовый" });
            comboType.SelectedIndex = TypeToIndex(question.Type);
            comboType.SelectedIndexChanged += comboType_SelectedIndexChanged;
            panel.Controls.Add(comboType, 1, 2);

            panel.Controls.Add(new Label { Text = "Варианты (через ;)", AutoSize = true, Margin = new Padding(3, 8, 3, 0) }, 0, 4);
            textOptions = new TextBox { Multiline = true, Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical };
            UiTheme.StyleInput(textOptions);
            textOptions.Text = string.Join("; ", question.Options ?? new List<string>());
            panel.Controls.Add(textOptions, 1, 4);

            var answerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = true
            };

            buttonChooseAnswer = new Button { Text = "Выбрать правильный ответ...", AutoSize = true };
            UiTheme.StyleSecondaryButton(buttonChooseAnswer);
            buttonChooseAnswer.Click += buttonChooseAnswer_Click;
            answerPanel.Controls.Add(buttonChooseAnswer);

            buttonShowAnswer = new Button { Text = "👁", AutoSize = true, Width = 44 };
            UiTheme.StyleSecondaryButton(buttonShowAnswer);
            buttonShowAnswer.Click += buttonShowAnswer_Click;
            answerPanel.Controls.Add(buttonShowAnswer);

            labelAnswerState = new Label { AutoSize = true, Margin = new Padding(12, 8, 3, 3) };
            answerPanel.Controls.Add(labelAnswerState);
            panel.Controls.Add(answerPanel, 1, 5);

            footerPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 64,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(8, 10, 8, 10)
            };
            buttonOk = new Button { Text = "Сохранить", Width = 170, Height = 42, DialogResult = DialogResult.None };
            buttonCancel = new Button { Text = "Отмена", Width = 170, Height = 42, DialogResult = DialogResult.Cancel };
            UiTheme.StylePrimaryButton(buttonOk);
            UiTheme.StyleSecondaryButton(buttonCancel);
            buttonOk.Click += buttonOk_Click;
            footerPanel.Controls.Add(buttonOk);
            footerPanel.Controls.Add(buttonCancel);
            Controls.Add(footerPanel);

            AcceptButton = buttonOk;
            CancelButton = buttonCancel;

            ApplyAdaptiveTypography();
            RefreshTypeState();
            RefreshAnswerStateLabel();
        }

        private void QuestionEditorForm_Shown(object sender, EventArgs e)
        {
            UiTheme.EnsureFormFitsOnScreen(this, center: true);
        }

        private void comboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshTypeState();
        }

        private void RefreshTypeState()
        {
            var type = GetSelectedType();
            textOptions.Enabled = type != QuestionType.Text;
        }

        private void buttonShowAnswer_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedAnswer))
            {
                MessageBox.Show(selectedAnswer, "Правильный ответ");
                return;
            }

            var hidden = SecurityService.TryDecryptAnswerForStorage(originalQuestion.AnswerEncrypted);
            if (!string.IsNullOrWhiteSpace(hidden))
            {
                MessageBox.Show(hidden, "Правильный ответ");
                return;
            }

            MessageBox.Show("Ответ недоступен для просмотра (есть только хеш).");
        }

        private void buttonChooseAnswer_Click(object sender, EventArgs e)
        {
            var type = GetSelectedType();

            if (type == QuestionType.Text)
            {
                var value = ShowTextAnswerDialog(selectedAnswer);
                if (value != null)
                {
                    selectedAnswer = value;
                    RefreshAnswerStateLabel();
                }
                return;
            }

            var options = NormalizeOptions(ParseOptions(textOptions.Text));
            if (options.Count < 2)
            {
                MessageBox.Show("Введите минимум 2 варианта ответа");
                return;
            }

            if (type == QuestionType.Single)
            {
                var value = ShowSingleAnswerDialog(options, selectedAnswer);
                if (value != null)
                {
                    selectedAnswer = value;
                    RefreshAnswerStateLabel();
                }
                return;
            }

            var many = ShowMultipleAnswerDialog(options, selectedAnswer);
            if (many != null)
            {
                selectedAnswer = many;
                RefreshAnswerStateLabel();
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var text = (textQuestion.Text ?? string.Empty).Trim();

            var type = GetSelectedType();

            var options = type == QuestionType.Text
                ? new List<string>()
                : NormalizeOptions(ParseOptions(textOptions.Text));

            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Введите текст вопроса");
                return;
            }

            if (type != QuestionType.Text && options.Count < 2)
            {
                MessageBox.Show("Для вариантов ответа нужно минимум 2 значения");
                return;
            }

            var answer = (selectedAnswer ?? string.Empty).Trim();
            var typeChanged = type != originalType;
            var optionsChanged = !SetEquals(options, originalOptions);

            KeepExistingStoredAnswer = false;

            if (string.IsNullOrWhiteSpace(answer))
            {
                if (hasStoredHiddenAnswer && !typeChanged && !optionsChanged)
                {
                    KeepExistingStoredAnswer = true;
                }
                else
                {
                    MessageBox.Show("Выберите правильный ответ через отдельное окно");
                    return;
                }
            }

            if (!KeepExistingStoredAnswer)
            {
                if (type == QuestionType.Single && !options.Any(x => Same(x, answer)))
                {
                    MessageBox.Show("Правильный ответ должен совпадать с одним из вариантов");
                    return;
                }

                if (type == QuestionType.Multiple)
                {
                    var answers = ParseOptions(answer);
                    if (answers.Count == 0)
                    {
                        MessageBox.Show("Для множественного вопроса выберите правильные варианты");
                        return;
                    }

                    if (answers.Any(a => !options.Any(o => Same(o, a))))
                    {
                        MessageBox.Show("Выбранные правильные ответы должны быть в списке вариантов");
                        return;
                    }

                    answer = string.Join(";", answers);
                }
            }

            QuestionText = text;
            QuestionType = type;
            Options = options;
            Answer = KeepExistingStoredAnswer ? string.Empty : answer;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void RefreshAnswerStateLabel()
        {
            if (!string.IsNullOrWhiteSpace(selectedAnswer))
            {
                if (GetSelectedType() == QuestionType.Multiple)
                {
                    labelAnswerState.Text = $"Выбрано правильных: {ParseOptions(selectedAnswer).Count}";
                }
                else
                {
                    labelAnswerState.Text = "Правильный ответ выбран";
                }
                return;
            }

            if (hasStoredHiddenAnswer)
            {
                labelAnswerState.Text = "Правильный ответ уже задан (скрыт)";
                return;
            }

            labelAnswerState.Text = "Правильный ответ не задан";
        }

        private QuestionType GetSelectedType()
        {
            switch (comboType.SelectedIndex)
            {
                case 1:
                    return QuestionType.Multiple;
                case 2:
                    return QuestionType.Text;
                default:
                    return QuestionType.Single;
            }
        }

        private static int TypeToIndex(QuestionType type)
        {
            switch (type)
            {
                case QuestionType.Multiple:
                    return 1;
                case QuestionType.Text:
                    return 2;
                case QuestionType.Single:
                default:
                    return 0;
            }
        }

        private static List<string> ParseOptions(string source)
        {
            return (source ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static List<string> NormalizeOptions(IEnumerable<string> source)
        {
            return (source ?? Enumerable.Empty<string>())
                .Select(x => x?.Trim() ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool SetEquals(List<string> left, List<string> right)
        {
            return left
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .SequenceEqual(
                    right.OrderBy(x => x, StringComparer.OrdinalIgnoreCase),
                    StringComparer.OrdinalIgnoreCase
                );
        }

        private static bool Same(string left, string right)
        {
            return string.Equals(
                left?.Trim(),
                right?.Trim(),
                StringComparison.OrdinalIgnoreCase
            );
        }

        private string ShowSingleAnswerDialog(List<string> options, string currentAnswer)
        {
            using (var form = new Form())
            {
                form.Text = "Выбор правильного ответа";
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(520, 420);
                form.MinimumSize = new Size(440, 340);
                form.FormBorderStyle = FormBorderStyle.Sizable;

                var list = new ListBox { Dock = DockStyle.Fill };
                list.Items.AddRange(options.Cast<object>().ToArray());
                if (!string.IsNullOrWhiteSpace(currentAnswer))
                {
                    var index = options.FindIndex(x => Same(x, currentAnswer));
                    if (index >= 0)
                        list.SelectedIndex = index;
                }

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 42,
                    FlowDirection = FlowDirection.RightToLeft
                };

                var ok = new Button { Text = "OK", Width = 100 };
                var cancel = new Button { Text = "Отмена", Width = 100, DialogResult = DialogResult.Cancel };
                footer.Controls.Add(ok);
                footer.Controls.Add(cancel);

                ok.Click += (_, __) =>
                {
                    if (list.SelectedItem == null)
                    {
                        MessageBox.Show("Выберите один правильный ответ");
                        return;
                    }
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                };

                form.Controls.Add(list);
                form.Controls.Add(footer);

                return form.ShowDialog(this) == DialogResult.OK
                    ? list.SelectedItem?.ToString() ?? string.Empty
                    : null;
            }
        }

        private string ShowMultipleAnswerDialog(List<string> options, string currentAnswer)
        {
            using (var form = new Form())
            {
                form.Text = "Выбор правильных ответов";
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(520, 420);
                form.MinimumSize = new Size(440, 340);
                form.FormBorderStyle = FormBorderStyle.Sizable;

                var checkedList = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true };
                checkedList.Items.AddRange(options.Cast<object>().ToArray());

                var selected = ParseOptions(currentAnswer);
                for (var i = 0; i < options.Count; i++)
                {
                    if (selected.Any(s => Same(s, options[i])))
                    {
                        checkedList.SetItemChecked(i, true);
                    }
                }

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 42,
                    FlowDirection = FlowDirection.RightToLeft
                };

                var ok = new Button { Text = "OK", Width = 100 };
                var cancel = new Button { Text = "Отмена", Width = 100, DialogResult = DialogResult.Cancel };
                footer.Controls.Add(ok);
                footer.Controls.Add(cancel);

                ok.Click += (_, __) =>
                {
                    if (checkedList.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Выберите хотя бы один правильный вариант");
                        return;
                    }
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                };

                form.Controls.Add(checkedList);
                form.Controls.Add(footer);

                if (form.ShowDialog(this) != DialogResult.OK)
                    return null;

                var values = checkedList.CheckedItems
                    .Cast<object>()
                    .Select(x => x.ToString())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                return string.Join(";", values);
            }
        }

        private string ShowTextAnswerDialog(string currentAnswer)
        {
            using (var form = new Form())
            {
                form.Text = "Введите правильный текстовый ответ";
                form.StartPosition = FormStartPosition.CenterParent;
                form.ClientSize = new Size(560, 220);
                form.MinimumSize = new Size(480, 180);
                form.FormBorderStyle = FormBorderStyle.Sizable;

                var text = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Text = currentAnswer ?? string.Empty
                };

                var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
                body.Controls.Add(text);
                form.Controls.Add(body);

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 42,
                    FlowDirection = FlowDirection.RightToLeft
                };

                var ok = new Button { Text = "OK", Width = 100 };
                var cancel = new Button { Text = "Отмена", Width = 100, DialogResult = DialogResult.Cancel };
                footer.Controls.Add(ok);
                footer.Controls.Add(cancel);

                ok.Click += (_, __) =>
                {
                    if (string.IsNullOrWhiteSpace(text.Text))
                    {
                        MessageBox.Show("Введите правильный ответ");
                        return;
                    }
                    form.DialogResult = DialogResult.OK;
                    form.Close();
                };

                form.Controls.Add(footer);

                return form.ShowDialog(this) == DialogResult.OK
                    ? text.Text.Trim()
                    : null;
            }
        }

        private void QuestionEditorForm_Resize(object sender, EventArgs e)
        {
            ApplyAdaptiveTypography();
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(860, 540));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StyleInput(textQuestion, uiScale);
            UiTheme.StyleInput(comboType, uiScale);
            UiTheme.StyleInput(textOptions, uiScale);
            UiTheme.StyleSecondaryButton(buttonChooseAnswer, uiScale);
            UiTheme.StyleSecondaryButton(buttonShowAnswer, uiScale);
            UiTheme.StylePrimaryButton(buttonOk, uiScale);
            UiTheme.StyleSecondaryButton(buttonCancel, uiScale);
            footerPanel.Height = UiTheme.ScalePx(72, uiScale);
            buttonOk.Width = UiTheme.ScalePx(170, uiScale);
            buttonOk.Height = UiTheme.ScalePx(42, uiScale);
            buttonCancel.Width = UiTheme.ScalePx(170, uiScale);
            buttonCancel.Height = UiTheme.ScalePx(42, uiScale);
            labelAnswerState.Font = UiTheme.CreateFont(11.5f, uiScale, FontStyle.Regular);
        }
    }
}


