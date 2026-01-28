using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TESTS
{
    public partial class admin_panel : Form
    {
        private List<TestModel> tests;
        private TestModel selectedTest;
        private string jsonPath;

        public admin_panel()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;

            // 🔑 КЛЮЧЕВОЙ ДЛЯ COMBOBOX В GRID
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;

            jsonPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tests.json"
            );

            InitTypeColumn();
            LoadTests();
        }

        // ================== COMBOBOX КОЛОНКА ==================
        private void InitTypeColumn()
        {
            int index = -1;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.HeaderText == "Тип вопроса")
                {
                    index = col.Index;
                    break;
                }
            }

            if (index == -1)
                return;

            var comboCol = new DataGridViewComboBoxColumn
            {
                Name = "Тип вопроса",
                HeaderText = "Тип вопроса",
                FlatStyle = FlatStyle.Flat,

                // ✅ ФИКСИРОВАННАЯ ШИРИНА
                Width = 140,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,

                DataSource = new[]
                {
                    "Один вариант",
                    "Несколько вариантов",
                    "Текстовый"
                }
            };

            dataGridView1.Columns.RemoveAt(index);
            dataGridView1.Columns.Insert(index, comboCol);
        }

        // ================== ЗАГРУЗКА JSON ==================
        private void LoadTests()
        {
            if (!File.Exists(jsonPath))
            {
                MessageBox.Show("Файл tests.json не найден");
                return;
            }

            try
            {
                var json = File.ReadAllText(jsonPath);
                tests = JsonConvert.DeserializeObject<List<TestModel>>(json);

                comboBox1.DisplayMember = "Title";
                comboBox1.DataSource = tests;

                if (tests.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки tests.json\n" + ex.Message);
            }
        }

        // ================== ВЫБОР ТЕСТА ==================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTest = comboBox1.SelectedItem as TestModel;
            LoadQuestions();
        }

        // ================== ЗАПОЛНЕНИЕ GRID ==================
        private void LoadQuestions()
        {
            dataGridView1.Rows.Clear();

            if (selectedTest == null || selectedTest.Questions == null)
                return;

            foreach (var q in selectedTest.Questions)
            {
                dataGridView1.Rows.Add(
                    q.Text,
                    q.Options != null ? string.Join("; ", q.Options) : "",
                    q.AnswerHash,
                    GetTypeName(q.Type),
                    "Изменить",
                    "Удалить"
                );
            }
        }

        // ================== КЛИКИ В GRID ==================
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (selectedTest == null)
                return;

            if (e.RowIndex < 0 || e.RowIndex >= selectedTest.Questions.Count)
                return;

            var columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            var row = dataGridView1.Rows[e.RowIndex];
            var question = selectedTest.Questions[e.RowIndex];

            // ===== УДАЛИТЬ =====
            if (columnName == "Удалить")
            {
                if (MessageBox.Show("Удалить вопрос?", "Подтверждение",
                    MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;

                selectedTest.Questions.RemoveAt(e.RowIndex);
                SaveJson();
                LoadQuestions();
                return;
            }

            // ===== ИЗМЕНИТЬ =====
            if (columnName == "Изменить")
            {
                question.Text = row.Cells[0].Value?.ToString() ?? "";

                question.Options = row.Cells[1].Value != null
                    ? new List<string>(
                        row.Cells[1].Value.ToString()
                            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    )
                    : new List<string>();

                question.AnswerHash = row.Cells[2].Value?.ToString() ?? "";

                switch (row.Cells[3].Value?.ToString())
                {
                    case "Один вариант": question.Type = 0; break;
                    case "Несколько вариантов": question.Type = 1; break;
                    case "Текстовый": question.Type = 2; break;
                    default:
                        MessageBox.Show("Выберите тип вопроса");
                        return;
                }

                SaveJson();
                MessageBox.Show("Изменения сохранены");
            }
        }

        // ================== СОХРАНЕНИЕ JSON ==================
        private void SaveJson()
        {
            var json = JsonConvert.SerializeObject(tests, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
        }

        // ================== ДОБАВИТЬ ВОПРОС ==================
        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedTest == null)
                return;

            selectedTest.Questions.Add(new QuestionModel
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Новый вопрос",
                Type = 2,
                Options = new List<string>(),
                AnswerHash = "",
                Salt = ""
            });

            SaveJson();
            LoadQuestions();
        }

        // ================== TYPE → TEXT ==================
        private string GetTypeName(int type)
        {
            switch (type)
            {
                case 0: return "Один вариант";
                case 1: return "Несколько вариантов";
                case 2: return "Текстовый";
                default: return "Текстовый";
            }
        }
    }

    // ================== МОДЕЛИ ==================
    public class TestModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeMinutes { get; set; }
        public List<QuestionModel> Questions { get; set; }
    }

    public class QuestionModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public int Type { get; set; }
        public List<string> Options { get; set; }
        public string AnswerHash { get; set; }
        public string Salt { get; set; }
    }
}
