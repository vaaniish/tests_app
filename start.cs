using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TESTS
{
    public partial class start : Form
    {
        private List<Test> tests = new List<Test>();
        private string testsPath;

        public start()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            testsPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tests.json"
            );

            Load += Start_Load;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            LoadTests();
            BindComboBox();
        }

        private void LoadTests()
        {
            if (!File.Exists(testsPath))
            {
                MessageBox.Show("Файл tests.json не найден");
                return;
            }

            try
            {
                var json = File.ReadAllText(testsPath);
                tests = JsonConvert.DeserializeObject<List<Test>>(json)
                        ?? new List<Test>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения tests.json:\n" + ex.Message);
            }
        }

        private void BindComboBox()
        {
            comboBoxTests.DataSource = null;
            comboBoxTests.DisplayMember = "Title";
            comboBoxTests.DataSource = tests;

            if (tests.Count > 0)
                comboBoxTests.SelectedIndex = 0;
        }

        private void ComboBoxTests_SelectedIndexChanged(object sender, EventArgs e)
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
                "Вопросов: " + selectedTest.Questions.Count;
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
                this.Hide();
                testForm.ShowDialog(this); // ВАЖНО: owner
                this.Show();
            }
        }
    }
}
