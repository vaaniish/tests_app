using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TESTS
{
    public partial class start : Form
    {
        private List<Test> tests = new List<Test>();
        private string encPath;

        // Ключ ДОЛЖЕН совпадать с admin_panel
        private static readonly byte[] CryptoKey =
            Encoding.UTF8.GetBytes("32_bytes_secret_key_123456789012");

        public start()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;

            encPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tests.enc"
            );

            Load += Start_Load;
        }

        private void Start_Load(object sender, EventArgs e)
        {
            LoadTests();
            BindComboBox();
        }

        // ===== ЗАГРУЗКА ТЕСТОВ (ТОЛЬКО ENC) =====
        private void LoadTests()
        {
            if (!File.Exists(encPath))
            {
                MessageBox.Show("Файл tests.enc не найден");
                return;
            }

            try
            {
                var encrypted = File.ReadAllText(encPath);
                var json = Decrypt(encrypted);

                tests = JsonConvert.DeserializeObject<List<Test>>(json)
                        ?? new List<Test>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки тестов:\n" + ex.Message);
            }
        }

        // ===== AES DECRYPT =====
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

        // ===== UI =====
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
                testForm.ShowDialog(this);
                this.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var adminForm = new admin_login())
            {
                this.Hide();
                adminForm.ShowDialog(this);
                this.Show();
            }
        }
    }
}
