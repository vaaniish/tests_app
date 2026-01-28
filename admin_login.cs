using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Text;


namespace TESTS
{
    public partial class admin_login : Form
    {
        private const string AdminLogin = "teacher_admin";

        // SHA-256 от пароля: T3st!2026_Admin
        private const string AdminPasswordHash =
            "DE0BD5A9FC7C84A88D2D93D1BF67FD95A3C0CA1E8D5B13FA916C72762F5E18A1";

        public admin_login()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void admin_login_Load(object sender, EventArgs e)
        {
        }

        private string ComputeSha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();

                foreach (var b in bytes)
                    sb.Append(b.ToString("X2"));

                return sb.ToString();
            }
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            var login = tbLogin.Text.Trim();
            var password = tbPassword.Text;

            if (login != AdminLogin)
            {
                MessageBox.Show("Неверный логин");
                return;
            }

            var hash = ComputeSha256(password);
            //MessageBox.Show(hash);

            if (hash != AdminPasswordHash)
            {
                MessageBox.Show("Неверный пароль");
                return;
            }

            // УСПЕХ
            using (var adminPanel = new admin_panel())
            {
                this.Hide();
                adminPanel.ShowDialog(this); // модальное окно
                this.Show();
            }
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {

        }
    }

}