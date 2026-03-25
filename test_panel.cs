using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TESTS
{
    public partial class test_panel : Form
    {
        private const int QuestionsPerAttempt = 20;
        private const int NavigationButtonBaseSizePx = 44;
        private const int NavigationButtonHorizontalPaddingPx = 16;

        private readonly Test test;
        private readonly bool teacherPreviewMode;
        private List<Question> questions = new List<Question>();
        private readonly Dictionary<int, object> userAnswers = new Dictionary<int, object>();
        private readonly List<Button> questionNavButtons = new List<Button>();

        private int currentIndex;
        private bool resultShown;
        private bool closingFromResult;

        private Timer testTimer;
        private TimeSpan remainingTime;
        private float uiScale = 1f;

        public test_panel(Test test)
            : this(test, false)
        {
        }

        public test_panel(Test test, bool teacherPreviewMode)
        {
            InitializeComponent();
            this.teacherPreviewMode = teacherPreviewMode;
            Text = teacherPreviewMode ? "Тестовый прогон (преподаватель)" : "Прохождение теста";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(920, 620);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
            WindowState = FormWindowState.Maximized;
            Resize += test_panel_Resize;
            Shown += test_panel_Shown;
            ApplyAdaptiveTypography();

            this.test = test ?? throw new ArgumentNullException(nameof(test));

            var rnd = new Random();
            questions = SelectQuestionsForAttempt(
                test.Questions ?? new List<Question>(),
                QuestionsPerAttempt,
                rnd
            );

            if (questions.Count == 0)
            {
                MessageBox.Show("В выбранном тесте нет вопросов.");
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            BuildQuestionNavigation();

            // Экзаменационный режим: 1 вопрос = 1 минута.
            remainingTime = TimeSpan.FromMinutes(Math.Max(1, questions.Count));

            testTimer = new Timer();
            testTimer.Interval = 1000;
            testTimer.Tick += TestTimer_Tick;
            testTimer.Start();

            UpdateTimerLabel();
            UpdateQuestionNumber();
            UpdateNavigationState();
            ApplyResponsiveLayout();
        }

        private void test_panel_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void test_panel_Load(object sender, EventArgs e)
        {
            if (questions.Count > 0)
                ShowQuestion();
        }

        private void ShowQuestion()
        {
            pnlContent.Controls.Clear();

            if (currentIndex >= questions.Count)
            {
                ShowResult();
                return;
            }

            var q = questions[currentIndex];

            UpdateQuestionNumber();
            UpdateNavigationState();

            pnlContent.Controls.Add(new Label
            {
                Text = PrepareQuestionTextForDisplay(q.Text),
                AutoSize = true,
                MaximumSize = new Size(pnlContent.Width - 40, 0),
                Font = UiTheme.CreateFont(15f, uiScale, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 15)
            });

            switch (q.Type)
            {
                case QuestionType.Single:
                    RenderSingle(q);
                    break;
                case QuestionType.Multiple:
                    RenderMultiple(q);
                    break;
                case QuestionType.Text:
                    RenderText();
                    break;
            }

            RestoreAnswer();
            UpdateContentControlWidths();
        }

        private void RenderSingle(Question q)
        {
            foreach (var option in q.Options)
            {
                pnlContent.Controls.Add(new RadioButton
                {
                    Text = option,
                    AutoSize = true,
                    Font = UiTheme.CreateFont(13.5f, uiScale, FontStyle.Regular),
                    Margin = new Padding(10)
                });
            }
        }

        private void RenderMultiple(Question q)
        {
            foreach (var option in q.Options)
            {
                pnlContent.Controls.Add(new CheckBox
                {
                    Text = option,
                    AutoSize = true,
                    Font = UiTheme.CreateFont(13.5f, uiScale, FontStyle.Regular),
                    Margin = new Padding(10)
                });
            }
        }

        private void RenderText()
        {
            pnlContent.Controls.Add(new TextBox
            {
                Width = pnlContent.Width - 40,
                Font = UiTheme.CreateFont(13.5f, uiScale, FontStyle.Regular),
                Margin = new Padding(10)
            });
        }

        private void SaveAnswer()
        {
            if (currentIndex < 0 || currentIndex >= questions.Count)
                return;

            var q = questions[currentIndex];

            switch (q.Type)
            {
                case QuestionType.Single:
                    userAnswers[currentIndex] = pnlContent.Controls
                        .OfType<RadioButton>()
                        .FirstOrDefault(r => r.Checked)?.Text ?? string.Empty;
                    break;

                case QuestionType.Multiple:
                    userAnswers[currentIndex] = pnlContent.Controls
                        .OfType<CheckBox>()
                        .Where(c => c.Checked)
                        .Select(c => c.Text)
                        .ToList();
                    break;

                case QuestionType.Text:
                    userAnswers[currentIndex] = pnlContent.Controls
                        .OfType<TextBox>()
                        .FirstOrDefault()?.Text ?? string.Empty;
                    break;
            }
        }

        private void RestoreAnswer()
        {
            if (!userAnswers.ContainsKey(currentIndex))
                return;

            var q = questions[currentIndex];
            var answer = userAnswers[currentIndex];

            switch (q.Type)
            {
                case QuestionType.Single:
                    foreach (var rb in pnlContent.Controls.OfType<RadioButton>())
                        rb.Checked = rb.Text == (string)answer;
                    break;

                case QuestionType.Multiple:
                    var list = answer as List<string> ?? new List<string>();
                    foreach (var cb in pnlContent.Controls.OfType<CheckBox>())
                        cb.Checked = list.Contains(cb.Text);
                    break;

                case QuestionType.Text:
                    var tb = pnlContent.Controls.OfType<TextBox>().FirstOrDefault();
                    if (tb != null)
                        tb.Text = answer as string ?? string.Empty;
                    break;
            }
        }

        private void btnNext_Click_Click(object sender, EventArgs e)
        {
            SaveAnswer();
            currentIndex++;
            ShowQuestion();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentIndex <= 0)
                return;

            SaveAnswer();
            currentIndex--;
            ShowQuestion();
        }

        private void BuildQuestionNavigation()
        {
            questionNavButtons.Clear();
            panelNavigation.Controls.Clear();
            var navButtonSize = GetNavigationButtonSize();
            var navButtonGap = UiTheme.ScalePx(8, uiScale);

            for (var i = 0; i < questions.Count; i++)
            {
                var index = i;
                var button = new Button
                {
                    Text = (index + 1).ToString(CultureInfo.InvariantCulture),
                    Tag = index,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    UseMnemonic = false,
                    UseCompatibleTextRendering = true,
                    Font = UiTheme.CreateFont(12.5f, uiScale, FontStyle.Regular),
                    Width = navButtonSize.Width,
                    Height = navButtonSize.Height,
                    Margin = new Padding(0, 0, navButtonGap, navButtonGap)
                };
                button.Click += questionNavButton_Click;
                questionNavButtons.Add(button);
                panelNavigation.Controls.Add(button);
            }
        }

        private void questionNavButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Button button) || !(button.Tag is int targetIndex))
                return;

            if (targetIndex < 0 || targetIndex >= questions.Count || targetIndex == currentIndex)
                return;

            SaveAnswer();
            currentIndex = targetIndex;
            ShowQuestion();
        }

        private void UpdateNavigationState()
        {
            btnBack.Enabled = currentIndex > 0;
            btnNext_Click.Text = currentIndex >= questions.Count - 1
                ? "Завершить тест"
                : "Следующий вопрос";
            UpdateQuestionNavigationButtonsState();
        }

        private void UpdateQuestionNavigationButtonsState()
        {
            for (var i = 0; i < questionNavButtons.Count; i++)
            {
                var button = questionNavButtons[i];
                var isCurrent = i == currentIndex;
                var isAnswered = HasAnswerForQuestion(i);
                button.Text = (i + 1).ToString(CultureInfo.InvariantCulture);

                button.BackColor = isCurrent
                    ? Color.FromArgb(0, 120, 215)
                    : isAnswered
                        ? Color.FromArgb(198, 239, 206)
                        : Color.White;
                button.ForeColor = isCurrent ? Color.White : Color.Black;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(58, 68, 78);
                button.FlatAppearance.BorderSize = isCurrent ? 2 : 1;
            }
        }

        private bool HasAnswerForQuestion(int questionIndex)
        {
            if (!userAnswers.TryGetValue(questionIndex, out var value) || value == null)
                return false;

            if (value is string text)
                return !string.IsNullOrWhiteSpace(text);

            if (value is List<string> many)
                return many.Count > 0;

            return true;
        }

        private void TestTimer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (remainingTime <= TimeSpan.Zero)
            {
                remainingTime = TimeSpan.Zero;
                UpdateTimerLabel();
                ShowResult();
                return;
            }

            UpdateTimerLabel();
        }

        private void UpdateTimerLabel()
        {
            labelTimer.Text = string.Format(
                "{0:D2}:{1:D2}",
                remainingTime.Minutes,
                remainingTime.Seconds
            );
        }

        private void UpdateQuestionNumber()
        {
            if (questions.Count == 0)
            {
                labelQNum.Text = "Вопрос 0 из 0";
                return;
            }

            labelQNum.Text = string.Format(
                "Вопрос {0} из {1}",
                currentIndex + 1,
                questions.Count
            );
        }

        private void ShowResult()
        {
            if (resultShown)
                return;

            resultShown = true;
            testTimer.Stop();
            SaveAnswer();

            var review = BuildAttemptReview();

            if (!teacherPreviewMode && review.AnsweredCount < review.TotalCount)
            {
                MessageBox.Show(
                    "Тест завершён!\n\nПройдите весь тест, чтобы узнать результат.",
                    "Результат теста",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                closingFromResult = true;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            if (teacherPreviewMode)
            {
                ShowTeacherReview(review);
                closingFromResult = true;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            var resultText = "Тест завершён!\n\n" +
                             $"Правильных ответов: {review.CorrectCount} из {review.TotalCount}";

            if (review.TotalCount > 0)
            {
                var percent = (double)review.CorrectCount / review.TotalCount * 100;
                resultText += "\n" + $"Результат: {percent:0}%";
            }

            MessageBox.Show(
                resultText,
                "Результат теста",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            closingFromResult = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private AttemptReview BuildAttemptReview()
        {
            var review = new AttemptReview
            {
                TotalCount = questions.Count
            };

            for (var i = 0; i < questions.Count; i++)
            {
                var question = questions[i];
                userAnswers.TryGetValue(i, out var userAnswer);

                var isAnswered = HasAnswerForQuestion(i);
                var isCorrect = isAnswered && SecurityService.VerifyQuestionAnswer(question, userAnswer);

                if (isAnswered)
                    review.AnsweredCount++;
                if (isCorrect)
                    review.CorrectCount++;

                review.Rows.Add(new AttemptReviewRow
                {
                    Number = i + 1,
                    QuestionText = PrepareQuestionTextForDisplay(question.Text),
                    UserAnswer = FormatUserAnswerForDisplay(question, userAnswer, isAnswered),
                    CorrectAnswer = FormatCorrectAnswerForDisplay(question),
                    IsAnswered = isAnswered,
                    IsCorrect = isCorrect
                });
            }

            return review;
        }

        private static string FormatUserAnswerForDisplay(Question question, object userAnswer, bool isAnswered)
        {
            if (!isAnswered || question == null)
                return "—";

            if (question.Type == QuestionType.Multiple)
            {
                var values = userAnswer as List<string> ?? new List<string>();
                return values.Count == 0 ? "—" : string.Join("; ", values);
            }

            var text = userAnswer as string ?? string.Empty;
            return string.IsNullOrWhiteSpace(text) ? "—" : text.Trim();
        }

        private static string FormatCorrectAnswerForDisplay(Question question)
        {
            if (question == null)
                return string.Empty;

            var answer = question.Answer;
            if (string.IsNullOrWhiteSpace(answer))
            {
                answer = SecurityService.TryDecryptAnswerForStorage(question.AnswerEncrypted);
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                return !string.IsNullOrWhiteSpace(question.AnswerHash)
                    ? "Скрыт (доступен только хеш)"
                    : "—";
            }

            return answer.Trim();
        }

        private void ShowTeacherReview(AttemptReview review)
        {
            var percent = review.TotalCount > 0
                ? (double)review.CorrectCount / review.TotalCount * 100
                : 0d;

            using (var form = new Form())
            {
                form.Text = "Тестовый прогон: разбор ответов";
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimumSize = new Size(980, 620);
                form.Size = new Size(
                    Math.Max(980, Width - UiTheme.ScalePx(120, uiScale)),
                    Math.Max(620, Height - UiTheme.ScalePx(120, uiScale))
                );

                var labelSummary = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 86,
                    BorderStyle = BorderStyle.FixedSingle,
                    Padding = new Padding(10, 8, 10, 8),
                    Font = UiTheme.CreateFont(11.8f, uiScale, FontStyle.Regular),
                    Text =
                        "Тестовый прогон завершен.\n" +
                        string.Format("Отвечено: {0} из {1}. Верно: {2}. Результат: {3:0}%.",
                            review.AnsweredCount,
                            review.TotalCount,
                            review.CorrectCount,
                            percent) + "\n" +
                        "Зеленый — верно, красный — ошибка, серый — нет ответа."
                };

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    MultiSelect = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                    AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                    BackgroundColor = Color.White
                };

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Number",
                    HeaderText = "№",
                    Width = 56
                });
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Question",
                    HeaderText = "Вопрос",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 42
                });
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "UserAnswer",
                    HeaderText = "Ваш ответ",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 24
                });
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "CorrectAnswer",
                    HeaderText = "Правильный ответ",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 24
                });
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Status",
                    HeaderText = "Статус",
                    Width = 124
                });

                foreach (var row in review.Rows)
                {
                    var statusText = !row.IsAnswered
                        ? "Нет ответа"
                        : row.IsCorrect ? "Верно" : "Ошибка";

                    var rowIndex = grid.Rows.Add(
                        row.Number,
                        row.QuestionText,
                        row.UserAnswer,
                        row.CorrectAnswer,
                        statusText
                    );

                    var uiRow = grid.Rows[rowIndex];
                    if (!row.IsAnswered)
                    {
                        uiRow.DefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242);
                    }
                    else if (row.IsCorrect)
                    {
                        uiRow.DefaultCellStyle.BackColor = Color.FromArgb(226, 239, 218);
                    }
                    else
                    {
                        uiRow.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    }
                }

                var panelBottom = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 56
                };

                var buttonClose = new Button
                {
                    Text = "Закрыть",
                    Width = 170,
                    Height = 36
                };

                buttonClose.Click += (s, e) => form.Close();
                panelBottom.Controls.Add(buttonClose);
                panelBottom.Resize += (s, e) =>
                {
                    buttonClose.Left = panelBottom.ClientSize.Width - buttonClose.Width - 10;
                    buttonClose.Top = Math.Max(8, (panelBottom.ClientSize.Height - buttonClose.Height) / 2);
                };

                form.Controls.Add(grid);
                form.Controls.Add(panelBottom);
                form.Controls.Add(labelSummary);

                form.Shown += (s, e) =>
                {
                    buttonClose.Left = panelBottom.ClientSize.Width - buttonClose.Width - 10;
                    buttonClose.Top = Math.Max(8, (panelBottom.ClientSize.Height - buttonClose.Height) / 2);
                };

                form.ShowDialog(this);
            }
        }

        private void buttonFinishEarly_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Завершить тест досрочно и показать результат?",
                "Досрочное завершение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ShowResult();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (testTimer != null)
            {
                testTimer.Stop();
                testTimer.Dispose();
            }
            base.OnFormClosed(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!closingFromResult && e.CloseReason == CloseReason.UserClosing && !resultShown)
            {
                var result = MessageBox.Show(
                    "Тест еще не завершен. Завершить досрочно и показать результат?",
                    "Подтверждение выхода",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    e.Cancel = true;
                    BeginInvoke(new Action(() =>
                    {
                        if (!IsDisposed && !Disposing)
                        {
                            ShowResult();
                        }
                    }));
                    return;
                }

                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        private void test_panel_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
            }

            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void ApplyResponsiveLayout()
        {
            var margin = UiTheme.ScalePx(12, uiScale);
            var gap = UiTheme.ScalePx(8, uiScale);

            var timerY = margin + 6;
            labelTimer.Location = new Point(ClientSize.Width - margin - labelTimer.Width, timerY);
            label1.Location = new Point(labelTimer.Left - gap - label1.Width, timerY);
            labelQNum.Location = new Point(margin, timerY);

            var navigationTop = Math.Max(labelQNum.Bottom, labelTimer.Bottom) + UiTheme.ScalePx(10, uiScale);
            var navPadding = UiTheme.ScalePx(10, uiScale);
            var navButtonGap = UiTheme.ScalePx(8, uiScale);
            var navButtonSize = GetNavigationButtonSize();
            var navHeaderHeight = UiTheme.ScalePx(26, uiScale);
            var navWidth = ClientSize.Width - margin * 2;
            var navInnerWidth = Math.Max(100, navWidth - navPadding * 2);
            var buttonsPerRow = Math.Max(1, (navInnerWidth + navButtonGap) / (navButtonSize.Width + navButtonGap));
            var questionCount = questions == null ? 0 : questions.Count;
            var rows = Math.Max(1, (int)Math.Ceiling(questionCount / (double)buttonsPerRow));
            var navInnerHeight = rows * navButtonSize.Height + (rows - 1) * navButtonGap;
            var navHeight = navHeaderHeight + navPadding + navInnerHeight + navPadding;

            groupNavigation.SetBounds(margin, navigationTop, navWidth, navHeight);
            panelNavigation.SetBounds(
                navPadding,
                navHeaderHeight,
                Math.Max(100, groupNavigation.ClientSize.Width - navPadding * 2),
                Math.Max(navButtonSize.Height, navInnerHeight + navButtonGap)
            );

            var contentTop = groupNavigation.Bottom + gap;
            var buttonsHeight = UiTheme.ScalePx(48, uiScale);
            var contentHeight = ClientSize.Height - contentTop - margin - gap - buttonsHeight;
            pnlContent.SetBounds(margin, contentTop, ClientSize.Width - margin * 2, Math.Max(180, contentHeight));

            var buttonTop = pnlContent.Bottom + gap;
            var buttonWidth = (pnlContent.Width - gap * 2) / 3;
            btnBack.SetBounds(margin, buttonTop, buttonWidth, buttonsHeight);
            btnNext_Click.SetBounds(btnBack.Right + gap, buttonTop, buttonWidth, buttonsHeight);
            if (buttonFinishEarly != null)
            {
                buttonFinishEarly.SetBounds(btnNext_Click.Right + gap, buttonTop, buttonWidth, buttonsHeight);
            }

            UpdateContentControlWidths();
        }

        private void UpdateContentControlWidths()
        {
            var maxWidth = Math.Max(200, pnlContent.ClientSize.Width - 42);

            foreach (var control in pnlContent.Controls)
            {
                if (control is Label label)
                {
                    label.MaximumSize = new Size(maxWidth, 0);
                }
                else if (control is TextBox textBox)
                {
                    textBox.Width = maxWidth;
                }
            }
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(1028, 707));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StylePrimaryButton(btnNext_Click, uiScale);
            UiTheme.StyleSecondaryButton(btnBack, uiScale);
            if (buttonFinishEarly != null)
            {
                UiTheme.StyleDangerButton(buttonFinishEarly, uiScale);
            }
            if (groupNavigation != null)
            {
                groupNavigation.Font = UiTheme.CreateFont(15f, uiScale, FontStyle.Bold);
            }

            var navButtonSize = GetNavigationButtonSize();
            var navButtonGap = UiTheme.ScalePx(8, uiScale);
            foreach (var button in questionNavButtons)
            {
                button.Width = navButtonSize.Width;
                button.Height = navButtonSize.Height;
                button.Margin = new Padding(0, 0, navButtonGap, navButtonGap);
                button.Font = UiTheme.CreateFont(12.5f, uiScale, FontStyle.Regular);
            }
            UpdateQuestionNavigationButtonsState();

            labelQNum.Font = UiTheme.CreateFont(15f, uiScale, FontStyle.Bold);
            label1.Font = UiTheme.CreateFont(15f, uiScale, FontStyle.Bold);
            labelTimer.Font = UiTheme.CreateFont(15f, uiScale, FontStyle.Bold);
        }

        private Size GetNavigationButtonSize()
        {
            var minSize = UiTheme.ScalePx(NavigationButtonBaseSizePx, uiScale);
            var maxNumberText = Math.Max(1, questions == null ? 0 : questions.Count)
                .ToString(CultureInfo.InvariantCulture);

            using (var font = UiTheme.CreateFont(12.5f, uiScale, FontStyle.Regular))
            {
                var measured = TextRenderer.MeasureText(
                    maxNumberText,
                    font,
                    new Size(int.MaxValue, int.MaxValue),
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding
                );

                var width = Math.Max(
                    minSize,
                    measured.Width + UiTheme.ScalePx(NavigationButtonHorizontalPaddingPx, uiScale)
                );

                return new Size(width, minSize);
            }
        }

        private static List<Question> SelectQuestionsForAttempt(List<Question> pool, int targetCount, Random rnd)
        {
            var source = (pool ?? new List<Question>())
                .Where(q => q != null && !string.IsNullOrWhiteSpace(q.Text))
                .Where(IsQuestionExamFriendly)
                .OrderBy(_ => rnd.Next())
                .ToList();

            var selected = new List<Question>();
            var usedFingerprint = new HashSet<string>(StringComparer.Ordinal);
            var usedFamily = new HashSet<string>(StringComparer.Ordinal);

            // 1st pass: maximize semantic uniqueness (one family -> one question).
            foreach (var question in source)
            {
                if (selected.Count >= targetCount)
                    break;

                var fingerprint = BuildQuestionFingerprint(question);
                if (!usedFingerprint.Add(fingerprint))
                    continue;

                var family = BuildQuestionFamilyKey(question);
                if (!usedFamily.Add(family))
                {
                    usedFingerprint.Remove(fingerprint);
                    continue;
                }

                selected.Add(question);
            }

            // 2nd pass: if not enough unique families, add remaining unique questions.
            if (selected.Count < targetCount)
            {
                foreach (var question in source)
                {
                    if (selected.Count >= targetCount)
                        break;

                    var fingerprint = BuildQuestionFingerprint(question);
                    if (!usedFingerprint.Add(fingerprint))
                        continue;

                    selected.Add(question);
                }
            }

            return selected;
        }

        private static bool IsQuestionExamFriendly(Question q)
        {
            if (q == null)
                return false;

            // Не даем студентам вопросы на заучивание номеров системных переменных/атрибутов.
            if (ContainsExamNoiseToken(q.Text))
                return false;

            if (ContainsExamNoiseToken(q.Answer))
                return false;

            foreach (var option in q.Options ?? new List<string>())
            {
                if (ContainsExamNoiseToken(option))
                    return false;
            }

            return true;
        }

        private static bool ContainsExamNoiseToken(string value)
        {
            var text = (value ?? string.Empty).Trim();
            if (text.Length == 0)
                return false;

            if (Regex.IsMatch(text, @"(^|\b)[sS]\d{2,6}(\b|$)"))
                return true;

            if (Regex.IsMatch(text, @"^(Атрибут|Атрибуты)\s+.*\d", RegexOptions.IgnoreCase))
                return true;

            return false;
        }

        private static string BuildQuestionFamilyKey(Question question)
        {
            var text = NormalizeForKey(question.Text);
            if (string.IsNullOrWhiteSpace(text))
                return "empty";

            // For theoretical singles: group by term inside «...», so "что означает термин X"
            // will not appear multiple times in one attempt with different wording.
            if (question.Type == QuestionType.Single)
            {
                var quoted = ExtractQuotedTerm(text);
                if (!string.IsNullOrWhiteSpace(quoted))
                    return "single-term::" + quoted;
            }

            return ((int)question.Type).ToString() + "::" + text;
        }

        private static string BuildQuestionFingerprint(Question question)
        {
            var options = (question.Options ?? new List<string>())
                .Select(NormalizeForKey)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .OrderBy(x => x, StringComparer.Ordinal);

            return ((int)question.Type).ToString() + "|" +
                   NormalizeForKey(question.Text) + "|" +
                   string.Join(";", options);
        }

        private static string NormalizeForKey(string value)
        {
            var normalized = (value ?? string.Empty).Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"\s+", " ");
            return normalized;
        }

        private static string PrepareQuestionTextForDisplay(string value)
        {
            var source = (value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            var withoutPrefix = Regex.Replace(source, @"^\s*(\[[^\]]+\]\s*)+", string.Empty);
            return string.IsNullOrWhiteSpace(withoutPrefix)
                ? source
                : withoutPrefix.TrimStart();
        }

        private static string ExtractQuotedTerm(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var start = text.IndexOf('«');
            var end = text.IndexOf('»');
            if (start >= 0 && end > start)
            {
                return NormalizeForKey(text.Substring(start + 1, end - start - 1));
            }

            var quoteMatch = Regex.Match(text, "\"([^\"]+)\"");
            if (quoteMatch.Success && quoteMatch.Groups.Count > 1)
            {
                return NormalizeForKey(quoteMatch.Groups[1].Value);
            }

            return string.Empty;
        }

        private sealed class AttemptReview
        {
            public int TotalCount { get; set; }
            public int AnsweredCount { get; set; }
            public int CorrectCount { get; set; }
            public List<AttemptReviewRow> Rows { get; } = new List<AttemptReviewRow>();
        }

        private sealed class AttemptReviewRow
        {
            public int Number { get; set; }
            public string QuestionText { get; set; }
            public string UserAnswer { get; set; }
            public string CorrectAnswer { get; set; }
            public bool IsAnswered { get; set; }
            public bool IsCorrect { get; set; }
        }
    }
}
