using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TESTS
{
    public partial class TestAdd : Form
    {
        public TestModel CreatedTest { get; private set; }
        public TestAdd()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void TestAdd_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var title = richTextBox1.Text.Trim();
            var description = richTextBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название теста");
                return;
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Введите описание теста");
                return;
            }

            CreatedTest = new TestModel
            {
                Id = "test-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                Title = title,
                Description = description, // ← ВАЖНО
                TimeMinutes = 10,
                Questions = new List<QuestionModel>()
            };

            DialogResult = DialogResult.OK;
            Close();


        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
