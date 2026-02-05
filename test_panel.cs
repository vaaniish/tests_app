using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TESTS
{
    public partial class test_panel : Form
    {
        private readonly Test test;
        private readonly List<Question> questions;
        private readonly Dictionary<int, object> userAnswers = new Dictionary<int, object>();

        private int currentIndex = 0;

        // ===== TIMER =====
        private Timer testTimer;
        private TimeSpan remainingTime;

        public test_panel(Test test)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            this.test = test ?? throw new ArgumentNullException(nameof(test));

            var rnd = new Random();

            questions = (test.Questions ?? new List<Question>())
                .OrderBy(q => rnd.Next())
                .Take(10)
                .ToList();

            // ===== INIT TIMER =====
            remainingTime = TimeSpan.FromMinutes(10);

            testTimer = new Timer();
            testTimer.Interval = 1000;
            testTimer.Tick += TestTimer_Tick;
            testTimer.Start();

            UpdateTimerLabel();
            UpdateQuestionNumber();
        }

        private void test_panel_Load(object sender, EventArgs e)
        {
            ShowQuestion();
        }

        // ================= UI =================

        private void ShowQuestion()
        {
            pnlContent.Controls.Clear();

            if (currentIndex >= questions.Count)
            {
                ShowResult();
                return;
            }

            UpdateQuestionNumber();

            var q = questions[currentIndex];

            pnlContent.Controls.Add(new Label
            {
                Text = q.Text,
                AutoSize = true,
                MaximumSize = new Size(pnlContent.Width - 40, 0),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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
        }

        private void RenderSingle(Question q)
        {
            foreach (var option in q.Options)
            {
                pnlContent.Controls.Add(new RadioButton
                {
                    Text = option,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12),
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
                    Font = new Font("Segoe UI", 12),
                    Margin = new Padding(10)
                });
            }
        }

        private void RenderText()
        {
            pnlContent.Controls.Add(new TextBox
            {
                Width = pnlContent.Width - 40,
                Font = new Font("Segoe UI", 12),
                Margin = new Padding(10)
            });
        }

        // ================= ANSWERS =================

        private void SaveAnswer()
        {
            var q = questions[currentIndex];

            switch (q.Type)
            {
                case QuestionType.Single:
                    userAnswers[currentIndex] = pnlContent.Controls
                        .OfType<RadioButton>()
                        .FirstOrDefault(r => r.Checked)?.Text;
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
                        .FirstOrDefault()?.Text;
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

        // ================= NAVIGATION =================

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

        // ================= TIMER =================

        private void TestTimer_Tick(object sender, EventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (remainingTime <= TimeSpan.Zero)
            {
                remainingTime = TimeSpan.Zero;
                UpdateTimerLabel();
                testTimer.Stop();
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

        // ================= QUESTION NUMBER =================

        private void UpdateQuestionNumber()
        {
            labelQNum.Text = string.Format(
                "Вопрос {0} из {1}",
                currentIndex + 1,
                questions.Count
            );
        }

        private void labelTimer_Click(object sender, EventArgs e)
        {
            // ничего не нужно
        }

        // ================= RESULT =================

        private void ShowResult()
        {
            testTimer.Stop();

            int correct = 0;

            for (int i = 0; i < questions.Count; i++)
            {
                if (!userAnswers.ContainsKey(i))
                    continue;

                var q = questions[i];
                var ua = userAnswers[i];

                switch (q.Type)
                {
                    case QuestionType.Single:
                        if (Normalize((string)ua) == Normalize(q.Answer))
                            correct++;
                        break;

                    case QuestionType.Multiple:
                        var user = ((List<string>)ua).Select(Normalize).OrderBy(x => x);
                        var right = q.Answer.Split(';').Select(Normalize).OrderBy(x => x);
                        if (user.SequenceEqual(right))
                            correct++;
                        break;

                    case QuestionType.Text:
                        if (Normalize((string)ua) == Normalize(q.Answer))
                            correct++;
                        break;
                }
            }

            double percent = questions.Count == 0
                ? 0
                : (double)correct / questions.Count * 100;

            MessageBox.Show(
                $"Тест завершён!\n\n" +
                $"Правильных ответов: {correct} из {questions.Count}\n" +
                $"Результат: {percent:0}%",
                "Результат теста",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            DialogResult = DialogResult.OK;
            Close();
        }

        // ================= UTILS =================

        private string Normalize(string s)
        {
            return s?.Trim().ToLowerInvariant() ?? string.Empty;
        }
    }
}
