using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TESTS
{
    public partial class admin_panel : Form
    {
        private readonly string baseDirectory;
        private List<Test> tests = new List<Test>();
        private Test selectedTest;
        private readonly ToolTip comboToolTip = new ToolTip();
        private float uiScale = 1f;

        public admin_panel()
        {
            InitializeComponent();
            Text = "Панель преподавателя";

            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1080, 640);
            WindowState = FormWindowState.Maximized;
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            ApplyAdaptiveTypography();

            comboBox1.DisplayMember = "Title";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.DoubleClick += comboBox1_DoubleClick;

            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 236, 241);
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.CellToolTipTextNeeded += dataGridView1_CellToolTipTextNeeded;

            InitTypeColumnAsText();
            EnsureShowAnswerColumn();
            ConfigureGridBehavior();

            Resize += admin_panel_Resize;
            Shown += admin_panel_Shown;
            ApplyResponsiveLayout();

            LoadTests();
        }

        private void admin_panel_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void admin_panel_Resize(object sender, EventArgs e)
        {
            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            var margin = UiTheme.ScalePx(12, uiScale);
            var gap = UiTheme.ScalePx(8, uiScale);
            var leftMinWidth = UiTheme.ScalePx(520, uiScale);
            var rightMinWidth = UiTheme.ScalePx(320, uiScale);
            var preferredRight = GetPreferredComboWidth();
            var rightMaxByRatio = (int)(ClientSize.Width * 0.42f);
            var rightMaxByPixels = UiTheme.ScalePx(680, uiScale);
            var rightMaxByLayout = ClientSize.Width - margin * 2 - gap - leftMinWidth;
            var rightMaxWidth = Math.Max(rightMinWidth, Math.Min(Math.Min(rightMaxByRatio, rightMaxByPixels), rightMaxByLayout));
            var rightWidth = Clamp(preferredRight, rightMinWidth, rightMaxWidth);
            var rightX = ClientSize.Width - rightWidth - margin;

            comboBox1.SetBounds(rightX, margin, rightWidth, UiTheme.ScalePx(36, uiScale));
            UpdateComboDropDownWidth();

            var buttonsTop = comboBox1.Bottom + gap;
            var freeHeight = ClientSize.Height - buttonsTop - margin;
            var buttonMin = UiTheme.ScalePx(84, uiScale);
            var buttonMax = UiTheme.ScalePx(180, uiScale);
            var buttonHeight = Math.Min(buttonMax, Math.Max(buttonMin, (freeHeight - gap * 2) / 3));

            button1.SetBounds(rightX, buttonsTop, rightWidth, buttonHeight);
            button2.SetBounds(rightX, button1.Bottom + gap, rightWidth, buttonHeight);
            button3.SetBounds(rightX, button2.Bottom + gap, rightWidth, buttonHeight);

            var gridWidth = Math.Max(UiTheme.ScalePx(420, uiScale), rightX - margin - gap);
            dataGridView1.SetBounds(margin, margin, gridWidth, ClientSize.Height - margin * 2);
        }

        private int GetPreferredComboWidth()
        {
            var width = UiTheme.ScalePx(300, uiScale);

            using (var g = comboBox1.CreateGraphics())
            {
                foreach (var test in tests)
                {
                    var measured = TextRenderer.MeasureText(g, test.Title ?? string.Empty, comboBox1.Font).Width + 40;
                    if (measured > width)
                        width = measured;
                }
            }

            return Math.Min(width, Math.Max(UiTheme.ScalePx(300, uiScale), (int)(ClientSize.Width * 0.42)));
        }

        private void UpdateComboDropDownWidth()
        {
            if (comboBox1 == null)
                return;

            var width = comboBox1.Width;
            using (var g = comboBox1.CreateGraphics())
            {
                foreach (var test in tests)
                {
                    var measured = TextRenderer.MeasureText(g, test.Title ?? string.Empty, comboBox1.Font).Width + 40;
                    if (measured > width)
                        width = measured;
                }
            }

            var screen = Screen.FromControl(this).WorkingArea;
            var comboLeftOnScreen = comboBox1.PointToScreen(Point.Empty).X;
            var availableToRight = screen.Right - comboLeftOnScreen - UiTheme.ScalePx(12, uiScale);
            var max = Math.Max(comboBox1.Width, availableToRight);
            comboBox1.DropDownWidth = Math.Min(Math.Max(comboBox1.Width, width), max);
        }

        private void ConfigureGridBehavior()
        {
            if (dataGridView1.Columns.Contains("Question"))
            {
                var col = dataGridView1.Columns["Question"];
                col.HeaderText = "Вопрос";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = 46;
                col.MinimumWidth = GetHeaderWidth(col.HeaderText) + UiTheme.ScalePx(18, uiScale);
            }

            if (dataGridView1.Columns.Contains("Answers"))
            {
                var col = dataGridView1.Columns["Answers"];
                col.HeaderText = "Ответы";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = 34;
                col.MinimumWidth = GetHeaderWidth(col.HeaderText) + UiTheme.ScalePx(18, uiScale);
            }

            if (dataGridView1.Columns.Contains("TrueAnswer"))
            {
                var col = dataGridView1.Columns["TrueAnswer"];
                col.HeaderText = "Статус ответа";
                col.Width = Math.Max(UiTheme.ScalePx(170, uiScale), GetHeaderWidth(col.HeaderText));
                col.ReadOnly = true;
            }

            if (dataGridView1.Columns.Contains("ShowAnswer"))
            {
                var col = dataGridView1.Columns["ShowAnswer"];
                col.HeaderText = "Глаз";
                col.Width = Math.Max(UiTheme.ScalePx(90, uiScale), GetHeaderWidth(col.HeaderText));
            }

            if (dataGridView1.Columns.Contains("Column1"))
            {
                var col = dataGridView1.Columns["Column1"];
                col.Width = Math.Max(UiTheme.ScalePx(170, uiScale), GetHeaderWidth(col.HeaderText));
                col.ReadOnly = true;
            }

            if (dataGridView1.Columns.Contains("Edit"))
            {
                var col = dataGridView1.Columns["Edit"];
                col.Width = Math.Max(UiTheme.ScalePx(130, uiScale), GetHeaderWidth(col.HeaderText));
            }

            if (dataGridView1.Columns.Contains("Delete"))
            {
                var col = dataGridView1.Columns["Delete"];
                col.Width = Math.Max(UiTheme.ScalePx(120, uiScale), GetHeaderWidth(col.HeaderText));
            }
        }

        private void InitTypeColumnAsText()
        {
            var typeColumn = dataGridView1.Columns["Column1"];
            if (typeColumn == null)
                return;

            typeColumn.HeaderText = "Тип вопроса";
            typeColumn.ReadOnly = true;
        }

        private void EnsureShowAnswerColumn()
        {
            if (dataGridView1.Columns.Contains("ShowAnswer"))
                return;

            var insertIndex = dataGridView1.Columns.Contains("TrueAnswer")
                ? dataGridView1.Columns["TrueAnswer"].Index + 1
                : dataGridView1.Columns.Count;

            var showColumn = new DataGridViewButtonColumn
            {
                Name = "ShowAnswer",
                HeaderText = "Глаз",
                Text = "👁",
                ToolTipText = "Показать правильный ответ",
                UseColumnTextForButtonValue = true,
                Width = UiTheme.ScalePx(84, uiScale)
            };

            dataGridView1.Columns.Insert(insertIndex, showColumn);
        }

        private void LoadTests()
        {
            try
            {
                tests = TestStorage.LoadOrCreateDefaultTests(baseDirectory);
            }
            catch (Exception ex)
            {
                tests = new List<Test>();
                MessageBox.Show("Ошибка загрузки tests_secure.db\n" + ex.Message);
            }

            comboBox1.DataSource = null;
            comboBox1.DataSource = tests;
            UpdateComboDropDownWidth();
            comboToolTip.SetToolTip(comboBox1, comboBox1.Text);

            if (tests.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                selectedTest = null;
                dataGridView1.Rows.Clear();
            }
        }

        private void SaveTests()
        {
            TestStorage.SaveTests(baseDirectory, tests);

            var selectedId = selectedTest != null ? selectedTest.Id : null;
            tests = TestStorage.LoadTests(baseDirectory);
            comboBox1.DataSource = null;
            comboBox1.DataSource = tests;
            UpdateComboDropDownWidth();

            if (!string.IsNullOrWhiteSpace(selectedId))
            {
                comboBox1.SelectedItem = tests.FirstOrDefault(t => string.Equals(t.Id, selectedId, StringComparison.Ordinal));
            }

            if (comboBox1.SelectedItem == null && tests.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTest = comboBox1.SelectedItem as Test;
            comboToolTip.SetToolTip(comboBox1, selectedTest?.Title ?? string.Empty);
            LoadQuestions();
        }

        private void comboBox1_DoubleClick(object sender, EventArgs e)
        {
            if (selectedTest == null)
                return;

            using (var form = new TestEditorForm(selectedTest))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                selectedTest.Title = form.TestTitle;
                selectedTest.Description = form.TestDescription;
                selectedTest.TimeMinutes = form.TimeMinutes;

                var currentId = selectedTest.Id;
                SaveTests();
                LoadTests();
                comboBox1.SelectedItem = tests.FirstOrDefault(t => t.Id == currentId);
            }
        }

        private void LoadQuestions()
        {
            dataGridView1.Rows.Clear();

            if (selectedTest == null || selectedTest.Questions == null)
                return;

            foreach (var q in selectedTest.Questions)
            {
                var rowIndex = dataGridView1.Rows.Add();
                var row = dataGridView1.Rows[rowIndex];
                row.Height = UiTheme.ScalePx(38, uiScale);
                row.Cells["Question"].Value = q.Text;
                row.Cells["Answers"].Value = q.Options != null ? string.Join("; ", q.Options) : string.Empty;
                row.Cells["TrueAnswer"].Value = GetAnswerStateText(q);
                row.Cells["Column1"].Value = GetTypeName(q.Type);
                row.Cells["Edit"].Value = "Изменить";
                row.Cells["Delete"].Value = "Удалить";
            }
        }

        private static string GetAnswerStateText(Question q)
        {
            if (!string.IsNullOrWhiteSpace(q.Answer))
                return "Задан";

            if (!string.IsNullOrWhiteSpace(q.AnswerEncrypted))
                return "Задан (зашифрован)";

            if (!string.IsNullOrWhiteSpace(q.AnswerHash))
                return "Задан (legacy хеш)";

            return "Не задан";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (selectedTest == null || e.RowIndex < 0 || e.RowIndex >= selectedTest.Questions.Count)
                return;

            var columnName = dataGridView1.Columns[e.ColumnIndex].Name;
            var question = selectedTest.Questions[e.RowIndex];

            if (IsFinalAutoQuestion(question) && (columnName == "Edit" || columnName == "Delete"))
            {
                MessageBox.Show(
                    "Этот вопрос автоматически подтягивается из уровней 1-4.\n" +
                    "Измените его в исходном тесте, либо создайте отдельный вопрос в итоговом.",
                    "Автоматический вопрос итогового",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (columnName == "Delete")
            {
                if (MessageBox.Show("Удалить вопрос?", "Подтверждение",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                selectedTest.Questions.RemoveAt(e.RowIndex);
                SaveTests();
                LoadQuestions();
                return;
            }

            if (columnName == "Edit")
            {
                EditQuestion(e.RowIndex);
                return;
            }

            if (columnName == "ShowAnswer")
            {
                ShowAnswer(e.RowIndex);
            }
        }

        private void dataGridView1_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            e.ToolTipText = value == null ? string.Empty : value.ToString();
        }

        private void ShowAnswer(int rowIndex)
        {
            var question = selectedTest.Questions[rowIndex];

            var answer = question.Answer;
            if (string.IsNullOrWhiteSpace(answer))
            {
                answer = SecurityService.TryDecryptAnswerForStorage(question.AnswerEncrypted);
            }

            if (string.IsNullOrWhiteSpace(answer) &&
                !string.IsNullOrWhiteSpace(question.AnswerHash) &&
                TestStorage.TryRecoverEncryptedAnswer(question))
            {
                answer = SecurityService.TryDecryptAnswerForStorage(question.AnswerEncrypted);
                SaveTests();
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                MessageBox.Show(
                    "Показать ответ нельзя: для этого вопроса доступен только хеш.\n" +
                    "Если это старая база, откройте вопрос и заново сохраните правильный ответ.",
                    "Просмотр ответа",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show(
                answer,
                "Правильный ответ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void EditQuestion(int rowIndex)
        {
            var question = selectedTest.Questions[rowIndex];

            using (var form = new QuestionEditorForm(question))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                question.Text = form.QuestionText;
                question.Type = form.QuestionType;
                question.Options = form.Options;

                if (form.KeepExistingStoredAnswer)
                {
                    question.Answer = string.Empty;
                }
                else
                {
                    question.Answer = form.Answer;
                    question.AnswerHash = string.Empty;
                    question.AnswerSalt = string.Empty;
                    question.AnswerEncrypted = string.Empty;
                }

                SaveTests();
                LoadQuestions();
            }
        }

        private static string GetTypeName(QuestionType type)
        {
            switch (type)
            {
                case QuestionType.Single:
                    return "Один вариант";
                case QuestionType.Multiple:
                    return "Несколько вариантов";
                case QuestionType.Text:
                default:
                    return "Текстовый";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedTest == null)
            {
                MessageBox.Show("Тест не выбран");
                return;
            }

            var newQuestion = new Question
            {
                Id = Guid.NewGuid().ToString(),
                Text = string.Empty,
                Type = QuestionType.Single,
                Options = new List<string> { "Вариант 1", "Вариант 2" },
                Answer = string.Empty,
                FinalSourceKey = string.Empty
            };

            using (var form = new QuestionEditorForm(newQuestion))
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                newQuestion.Text = form.QuestionText;
                newQuestion.Type = form.QuestionType;
                newQuestion.Options = form.Options;
                newQuestion.Answer = form.Answer;

                if (selectedTest.Questions == null)
                    selectedTest.Questions = new List<Question>();

                selectedTest.Questions.Add(newQuestion);
                SaveTests();
                LoadQuestions();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new TestAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK && form.CreatedTest != null)
                    {
                        tests.Add(form.CreatedTest);
                        var createdId = form.CreatedTest.Id;
                        SaveTests();
                        LoadTests();
                        comboBox1.SelectedItem = tests.FirstOrDefault(t => t.Id == createdId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении теста:\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedTest == null)
            {
                MessageBox.Show("Тест не выбран");
                return;
            }

            var result = MessageBox.Show(
                $"Удалить тест \"{selectedTest.Title}\"?",
                "Подтверждение удаления",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            tests.Remove(selectedTest);
            SaveTests();
            LoadTests();
            selectedTest = null;
            dataGridView1.Rows.Clear();
        }

        private bool IsFinalAutoQuestion(Question q)
        {
            return selectedTest != null
                && string.Equals(selectedTest.Id, "tm7-final-advanced", StringComparison.Ordinal)
                && !string.IsNullOrWhiteSpace(q.FinalSourceKey);
        }

        private int GetHeaderWidth(string headerText)
        {
            using (var g = dataGridView1.CreateGraphics())
            {
                return TextRenderer.MeasureText(
                    g,
                    headerText ?? string.Empty,
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font
                ).Width + 26;
            }
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(1474, 762));
            UiTheme.ApplyBase(this, uiScale);

            UiTheme.StyleInput(comboBox1, uiScale);
            UiTheme.StylePrimaryButton(button1, uiScale);
            UiTheme.StyleSecondaryButton(button2, uiScale);
            UiTheme.StyleDangerButton(button3, uiScale);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = UiTheme.CreateFont(13f, uiScale, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = UiTheme.CreateFont(12.4f, uiScale, FontStyle.Regular);
            dataGridView1.RowTemplate.Height = UiTheme.ScalePx(38, uiScale);
            dataGridView1.ColumnHeadersHeight = UiTheme.ScalePx(46, uiScale);
            ConfigureGridBehavior();
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
