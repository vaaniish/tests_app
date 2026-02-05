using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TESTS
{
    public partial class test_panel : Form
    {
        private Test test;
        private int currentQuestionIndex = 0;
        private int correctAnswers = 0;
        private int totalQuestions;

        public test_panel(Test test)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            this.test = test;
            totalQuestions = test.Questions.Count;
        }

        private void test_panel_Load(object sender, EventArgs e)
        {
            ShowQuestion();
        }

        // ================== UI ==================
        private void ShowQuestion()
        {
            pnlContent.Controls.Clear();

            if (currentQuestionIndex >= test.Questions.Count)
            {
                ShowResult();
                return;
            }

            var q = test.Questions[currentQuestionIndex];

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
                    RenderSingleChoice(q);
                    break;

                case QuestionType.Multiple:
                    RenderMultipleChoice(q);
                    break;

                case QuestionType.Text:
                    RenderTextInput();
                    break;
            }
        }

        private void RenderSingleChoice(Question q)
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

        private void RenderMultipleChoice(Question q)
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

        private void RenderTextInput()
        {
            pnlContent.Controls.Add(new TextBox
            {
                Width = pnlContent.Width - 40,
                Font = new Font("Segoe UI", 12),
                Margin = new Padding(10)
            });
        }

        // ================== CHECK ==================
        private void CheckAnswer()
        {
            var q = test.Questions[currentQuestionIndex];

            switch (q.Type)
            {
                case QuestionType.Single:
                    CheckSingleChoice(q);
                    break;

                case QuestionType.Multiple:
                    CheckMultipleChoice(q);
                    break;

                case QuestionType.Text:
                    CheckTextAnswer(q);
                    break;
            }
        }

        private void CheckSingleChoice(Question q)
        {
            var selected = pnlContent.Controls
                .OfType<RadioButton>()
                .FirstOrDefault(r => r.Checked);

            if (selected == null)
                return;

            if (Normalize(selected.Text) == Normalize(q.Answer))
                correctAnswers++;
        }

        private void CheckMultipleChoice(Question q)
        {
            var selected = pnlContent.Controls
                .OfType<CheckBox>()
                .Where(c => c.Checked)
                .Select(c => Normalize(c.Text))
                .OrderBy(x => x)
                .ToList();

            if (selected.Count == 0)
                return;

            var correct = q.Answer
                .Split(';')
                .Select(x => Normalize(x))
                .OrderBy(x => x)
                .ToList();

            if (selected.SequenceEqual(correct))
                correctAnswers++;
        }

        private void CheckTextAnswer(Question q)
        {
            var tb = pnlContent.Controls
                .OfType<TextBox>()
                .FirstOrDefault();

            if (tb == null)
                return;

            if (Normalize(tb.Text) == Normalize(q.Answer))
                correctAnswers++;
        }

        // ================== RESULT ==================
        private void ShowResult()
        {
            double percent = (double)correctAnswers / totalQuestions * 100;

            MessageBox.Show(
                $"Тест завершён!\n\n" +
                $"Правильных ответов: {correctAnswers} из {totalQuestions}\n" +
                $"Результат: {percent:0}%",
                "Результат теста",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnNext_Click_Click(object sender, EventArgs e)
        {
            CheckAnswer();
            currentQuestionIndex++;
            ShowQuestion();
        }

        // ================== UTILS ==================
        private string Normalize(string s)
        {
            return s?.Trim().ToLowerInvariant();
        }
    }
}
