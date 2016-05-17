namespace TsuBalance
{
    using System;
    using System.Windows.Forms;

    using Microsoft.Win32;

    public class SettingsStorage : ISettingsStorage
    {
        private const string AppName = "TsuVpnBalanceChecker";

        private RegistryKey _rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public void Save(string login, string pass, bool autorun)
        {
            Properties.Settings.Default.Login = login;
            Properties.Settings.Default.Password = pass;
            Properties.Settings.Default.Save();
            if (autorun)
            {
                _rkApp.SetValue(AppName, Application.ExecutablePath);
            }
            else
            {
                _rkApp.DeleteValue(AppName, false);
            }
        }

        public Tuple<string, string, bool> Load()
        {
            var autorun = _rkApp.GetValue(AppName) != null
                          && _rkApp.GetValue(AppName).ToString() == Application.ExecutablePath;
            return new Tuple<string, string, bool>(Properties.Settings.Default.Login, Properties.Settings.Default.Password, autorun);
        }
    }
}