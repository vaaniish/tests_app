using System;
using System.Drawing;
using System.Windows.Forms;

namespace TESTS
{
    public partial class admin_login : Form
    {
        // Логин и пароль хранятся только в виде солей/хэшей.
        private const string LoginSalt = "VKUrMuLXngwVGhMPW2bnEw==";
        private const string LoginHash = "KQJFq8zCbmPwhpt71FO6WPN+dL2YhugfJryUtsu5C1g=";
        private const string PasswordSalt = "C24x5m5IUGIF5eVt4PhNpw==";
        private const string PasswordHash = "23JOQPDZQzqLnBuZUnE7Sn2W7s9k5W0IAuzyw6E1ZHQ=";

        private readonly Button buttonShowPassword;
        private bool passwordVisible;
        private float uiScale = 1f;

        public admin_login()
        {
            InitializeComponent();
            Text = "Вход преподавателя";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(460, 380);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MinimizeBox = false;
            MaximizeBox = false;
            WindowState = FormWindowState.Maximized;

            tbPassword.UseSystemPasswordChar = true;

            buttonShowPassword = new Button
            {
                Text = "👁",
                TabStop = false
            };
            buttonShowPassword.Click += buttonShowPassword_Click;
            Controls.Add(buttonShowPassword);

            Resize += admin_login_Resize;
            Shown += admin_login_Shown;

            ApplyAdaptiveTypography();
            ApplyResponsiveLayout();
        }

        private void admin_login_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            ApplyResponsiveLayout();
        }

        private void admin_login_Load(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            var login = tbLogin.Text;
            var password = tbPassword.Text;

            var loginOk = SecurityService.VerifySecret(login, LoginSalt, LoginHash, normalize: true);
            var passwordOk = SecurityService.VerifySecret(password, PasswordSalt, PasswordHash, normalize: false);

            if (!loginOk || !passwordOk)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            using (var adminPanel = new admin_panel())
            {
                Hide();
                adminPanel.ShowDialog(this);
                Show();
            }
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
        }

        private void admin_login_Resize(object sender, EventArgs e)
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
            var outerMargin = UiTheme.ScalePx(20, uiScale);
            var cardWidth = Math.Min(UiTheme.ScalePx(920, uiScale), ClientSize.Width - outerMargin * 2);
            cardWidth = Math.Max(UiTheme.ScalePx(360, uiScale), cardWidth);

            var gap = UiTheme.ScalePx(12, uiScale);
            var inputTopGap = UiTheme.ScalePx(6, uiScale);
            var buttonTopGap = UiTheme.ScalePx(20, uiScale);
            var inputHeight = UiTheme.ScalePx(42, uiScale);
            var buttonHeight = UiTheme.ScalePx(52, uiScale);
            var eyeWidth = UiTheme.ScalePx(50, uiScale);
            var eyeGap = UiTheme.ScalePx(8, uiScale);

            var totalHeight =
                label1.Height + inputTopGap + inputHeight +
                gap +
                label2.Height + inputTopGap + inputHeight +
                buttonTopGap + buttonHeight;

            var startX = Math.Max(outerMargin, (ClientSize.Width - cardWidth) / 2);
            var startY = Math.Max(outerMargin, (ClientSize.Height - totalHeight) / 2);

            label1.Location = new Point(startX, startY);
            tbLogin.SetBounds(startX, label1.Bottom + inputTopGap, cardWidth, inputHeight);

            label2.Location = new Point(startX, tbLogin.Bottom + gap);
            var passwordWidth = Math.Max(UiTheme.ScalePx(220, uiScale), cardWidth - eyeWidth - eyeGap);
            tbPassword.SetBounds(startX, label2.Bottom + inputTopGap, passwordWidth, inputHeight);
            buttonShowPassword.SetBounds(tbPassword.Right + eyeGap, tbPassword.Top, eyeWidth, inputHeight);

            btnLogin.SetBounds(startX, tbPassword.Bottom + buttonTopGap, cardWidth, buttonHeight);
        }

        private void buttonShowPassword_Click(object sender, EventArgs e)
        {
            passwordVisible = !passwordVisible;
            tbPassword.UseSystemPasswordChar = !passwordVisible;
            buttonShowPassword.Text = passwordVisible ? "🙈" : "👁";
        }

        private void ApplyAdaptiveTypography()
        {
            uiScale = UiTheme.GetAdaptiveScale(this, new Size(1366, 768));
            UiTheme.ApplyBase(this, uiScale);
            UiTheme.StyleInput(tbLogin, uiScale);
            UiTheme.StyleInput(tbPassword, uiScale);
            UiTheme.StylePrimaryButton(btnLogin, uiScale);
            UiTheme.StyleSecondaryButton(buttonShowPassword, uiScale);
            UiTheme.StyleTitleLabel(label1, uiScale);
            UiTheme.StyleTitleLabel(label2, uiScale);
        }
    }
}
