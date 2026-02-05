using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TESTS
{
    public partial class admin_panel : Form
    {
        private List<TestModel> tests;
        private TestModel selectedTest;
        private string encPath;

        // 32-byte key for AES-256
        private static readonly byte[] CryptoKey =
            Encoding.UTF8.GetBytes("32_bytes_secret_key_123456789012");

        public admin_panel()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;

            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;

            encPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tests.enc"
            );
            //MigrateJsonToEnc();
            DebugDecryptEncToJson();
            InitTypeColumn();
            LoadTests();
        }

        // ================== INIT TYPE COLUMN ==================
        private void InitTypeColumn()
        {
            // Если колонка "Тип вопроса" уже существует в дизайнере — заменим её на ComboBoxColumn
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

        // ================== LOAD ==================
        private void LoadTests()
        {
            if (!File.Exists(encPath))
            {
                tests = new List<TestModel>();
                comboBox1.DisplayMember = "Title";
                comboBox1.DataSource = tests;
                return;
            }

            try
            {
                var encrypted = File.ReadAllText(encPath);
                var json = Decrypt(encrypted);
                tests = JsonConvert.DeserializeObject<List<TestModel>>(json) ?? new List<TestModel>();

                comboBox1.DisplayMember = "Title";
                comboBox1.DataSource = tests;

                if (tests.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки tests.enc\n" + ex.Message);
                tests = new List<TestModel>();
                comboBox1.DisplayMember = "Title";
                comboBox1.DataSource = tests;
            }
        }

        // ================== OPTIONAL MIGRATION / DEBUG ==================
        // Если нужно сконвертировать tests.json -> tests.enc (однократно)
        private void MigrateJsonToEnc()
        {
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tests.json");

            if (!File.Exists(jsonPath))
            {
                MessageBox.Show("tests.json не найден");
                return;
            }

            if (File.Exists(encPath))
            {
                MessageBox.Show("tests.enc уже существует");
                return;
            }

            var json = File.ReadAllText(jsonPath);
            var encrypted = Encrypt(json);
            File.WriteAllText(encPath, encrypted);
            MessageBox.Show("Миграция завершена. tests.enc создан.");
        }

        // Для отладки: создаёт tests_debug.json расшифрованный из tests.enc
        private void DebugDecryptEncToJson()
        {
            if (!File.Exists(encPath))
            {
                MessageBox.Show("tests.enc не найден");
                return;
            }

            var encrypted = File.ReadAllText(encPath);
            var json = Decrypt(encrypted);

            var debugPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tests_debug.json");
            File.WriteAllText(debugPath, json, Encoding.UTF8);

            MessageBox.Show("tests_debug.json создан для проверки");
        }

        // ================== COMBO ==================
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTest = comboBox1.SelectedItem as TestModel;
            LoadQuestions();
        }

        // ================== GRID ==================
        private void LoadQuestions()
        {
            dataGridView1.Rows.Clear();

            if (selectedTest?.Questions == null)
                return;

            foreach (var q in selectedTest.Questions)
            {
                dataGridView1.Rows.Add(
                    q.Text,
                    q.Options != null ? string.Join("; ", q.Options) : "",
                    q.Answer,
                    GetTypeName(q.Type),
                    "Изменить",
                    "Удалить"
                );
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (selectedTest == null)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.RowIndex >= selectedTest.Questions.Count)
                return;

            var columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;

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
                var row = dataGridView1.Rows[e.RowIndex];
                var question = selectedTest.Questions[e.RowIndex];

                question.Text = row.Cells[0].Value?.ToString() ?? "";

                question.Options = row.Cells[1].Value != null
                    ? row.Cells[1].Value.ToString()
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList()
                    : new List<string>();

                question.Answer = row.Cells[2].Value?.ToString() ?? "";

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


        // ================== ADD QUESTION ==================
        // button1 = добавить вопрос в выбранный тест
        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedTest == null)
            {
                MessageBox.Show("Тест не выбран");
                return;
            }

            if (selectedTest.Questions == null)
                selectedTest.Questions = new List<QuestionModel>();

            selectedTest.Questions.Add(new QuestionModel
            {
                Id = Guid.NewGuid().ToString(),
                Text = "Новый вопрос",
                Type = 2,
                Options = new List<string>(),
                Answer = ""
            });

            SaveJson();
            LoadQuestions();
        }

        // ================== ADD TEST ==================
        // button2 = добавить новый тест (предполагается форма TestAdd, аналог в твоём проекте)
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new TestAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK && form.CreatedTest != null)
                    {
                        if (tests == null) tests = new List<TestModel>();
                        tests.Add(form.CreatedTest);
                        SaveJson();
                        LoadTests();
                        comboBox1.SelectedItem = form.CreatedTest;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении теста:\n" + ex.Message);
            }
        }

        // ================== DELETE TEST ==================
        // button3 = удалить выбранный тест
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
            SaveJson();
            LoadTests();
            selectedTest = null;
            dataGridView1.Rows.Clear();
        }

        // ================== SAVE ==================
        private void SaveJson()
        {
            var json = JsonConvert.SerializeObject(tests, Formatting.Indented);
            var encrypted = Encrypt(json);
            File.WriteAllText(encPath, encrypted);
        }

        // ================== TYPE ==================
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

        // ================== AES ==================
        private static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = CryptoKey;
                aes.GenerateIV();

                using (var encryptor = aes.CreateEncryptor())
                {
                    var bytes = Encoding.UTF8.GetBytes(plainText);
                    var cipher = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

                    var result = new byte[aes.IV.Length + cipher.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipher, 0, result, aes.IV.Length, cipher.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }

        private static string Decrypt(string encryptedText)
        {
            var full = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.Key = CryptoKey;

                var iv = new byte[16];
                var cipher = new byte[full.Length - 16];

                Buffer.BlockCopy(full, 0, iv, 0, 16);
                Buffer.BlockCopy(full, 16, cipher, 0, cipher.Length);

                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                {
                    var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                    return Encoding.UTF8.GetString(plain);
                }
            }
        }
    }

    // ================== MODELS ==================
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
        public string Answer { get; set; }
    }
}
