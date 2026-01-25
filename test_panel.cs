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
            this.StartPosition = FormStartPosition.CenterScreen;
            this.test = test;
            totalQuestions = test.Questions.Count;
        }

        private void test_panel_Load(object sender, EventArgs e)
        {
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            pnlContent.Controls.Clear();

            if (currentQuestionIndex >= test.Questions.Count)
            {
                MessageBox.Show("Тест завершён");
                Close();
                return;
            }

            var q = test.Questions[currentQuestionIndex];

            // ВОПРОС
            var lbl = new Label
            {
                Text = q.Text,
                AutoSize = true,
                MaximumSize = new Size(pnlContent.Width - 40, 0),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Margin = new Padding(5, 5, 5, 15)
            };

            pnlContent.Controls.Add(lbl);

            // ОТВЕТЫ
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
                var rb = new RadioButton
                {
                    Text = option,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12),
                    Margin = new Padding(10)
                };
                pnlContent.Controls.Add(rb);
            }
        }


        private void RenderMultipleChoice(Question q)
        {
            foreach (var option in q.Options)
            {
                var cb = new CheckBox
                {
                    Text = option,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12),
                    Margin = new Padding(10)
                };
                pnlContent.Controls.Add(cb);
            }
        }

        private void RenderTextInput()
        {
            var tb = new TextBox
            {
                Width = pnlContent.Width - 40,
                Font = new Font("Segoe UI", 12),
                Margin = new Padding(10)
            };
            pnlContent.Controls.Add(tb);
        }

        private void CheckAnswer()
        {
            var q = test.Questions[currentQuestionIndex];

            switch (q.Type)
            {
                case QuestionType.Single:
                    CheckSingleChoice();
                    break;

                case QuestionType.Multiple:
                    CheckMultipleChoice();
                    break;

                case QuestionType.Text:
                    CheckTextAnswer();
                    break;
            }
        }
        private void CheckSingleChoice()
        {
            var radios = pnlContent.Controls.OfType<RadioButton>().ToList();
            if (radios.Count == 0) return;

            // ВРЕМЕННО: считаем правильным последний вариант
            if (radios.Last().Checked)
                correctAnswers++;
        }
        private void CheckMultipleChoice()
        {
            var checks = pnlContent.Controls.OfType<CheckBox>().ToList();
            if (checks.Count == 0) return;

            // ВРЕМЕННО: если отмечен хотя бы один вариант
            if (checks.Any(c => c.Checked))
                correctAnswers++;
        }
        private void CheckTextAnswer()
        {
            var tb = pnlContent.Controls.OfType<TextBox>().FirstOrDefault();
            if (tb == null) return;

            if (!string.IsNullOrWhiteSpace(tb.Text))
                correctAnswers++;
        }
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

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void btnNext_Click_Click(object sender, EventArgs e)
        {
            // Здесь позже будет проверка ответа
            CheckAnswer();

            currentQuestionIndex++;

            if (currentQuestionIndex >= totalQuestions)
            {
                ShowResult();
                return;
            }

            ShowQuestion();
        }
    }
}




