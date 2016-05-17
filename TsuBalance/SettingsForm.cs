using System;
using System.Windows.Forms;

namespace TsuBalance
{
    using System.Globalization;
    using System.Threading.Tasks;

    public partial class SettingsForm : Form
    {
        private readonly ISiteGateway _gateway = new SiteGateway();

        private readonly ISettingsStorage _settingsStorage = new SettingsStorage();

        public SettingsForm()
        {
            InitializeComponent();
        }

        private async void SettingsForm_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(Hide));
            var data = _settingsStorage.Load();
            txtLogin.Text = data.Item1;
            txtPassword.Text = data.Item2;
            chkAutoRun.Checked = data.Item3;
            await TryLogin();
        }

        private async void btnEnter_Click(object sender, EventArgs e)
        {
            _settingsStorage.Save(txtLogin.Text, txtPassword.Text, chkAutoRun.Checked);
            await TryLogin();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;
                BeginInvoke(new MethodInvoker(Hide));
            }
        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var balance = await _gateway.GetBalanceAsync();
                trayIcon.ShowBalloonTip(500, "Баланс ТГУшного интернета", 
                    $"{balance.ToString(CultureInfo.InvariantCulture)} руб.", ToolTipIcon.Info);
            }
            catch (SiteGatewayException)
            {
                trayIcon.ShowBalloonTip(500, "Баланс ТГУшного интернета", "Ошибка получения баланса", ToolTipIcon.Error);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(Show));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://vpn.tsu.ru");
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            await CheckBalanceAndWarning();
        }

        private async Task CheckBalanceAndWarning()
        {
            try
            {
                var balance = await _gateway.GetBalanceAsync();
                if (balance < 20)
                {
                    trayIcon.ShowBalloonTip(500, "Баланс ТГУшного интернета",
                            $"Баланс составляет {balance.ToString(CultureInfo.InvariantCulture)} руб. Пополните счет", ToolTipIcon.Warning);
                }
               
            }
            catch (SiteGatewayException)
            {}
        }

        private async Task TryLogin()
        {
            var tupl = _settingsStorage.Load();
            if (string.IsNullOrWhiteSpace(tupl.Item1) || string.IsNullOrWhiteSpace(tupl.Item2))
            {
                BeginInvoke(new MethodInvoker(Show));
            }
            else
            {
                try
                {
                    if (await _gateway.AuthorizeAsync(tupl.Item1, tupl.Item2))
                    {
                        await CheckBalanceAndWarning();
                        timer.Enabled = true;
                    }
                    else
                    {
                        BeginInvoke(new MethodInvoker(Show));
                        MessageBox.Show("Неправильный логин или пароль");
                    }
                }
                catch (SiteGatewayException e)
                {
                    MessageBox.Show($"Ошибка. {e.Message}");
                }
            }
        }
    }
}
