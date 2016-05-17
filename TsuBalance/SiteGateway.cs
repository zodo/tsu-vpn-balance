namespace TsuBalance
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class SiteGateway : ISiteGateway
    {

        private readonly Uri ServerAddress = new Uri("http://vpn.tsu.ru/");

        private string _login;

        private string _pass;

        private async Task<string> GetContent(string login, string pass)
        {
            if (login == null || pass == null)
            {
                throw new SiteGatewayException("Необходима аутентификация");
            }
            using (var client = new HttpClient())
            {
                var content =
                    new FormUrlEncodedContent(
                        new[]
                        {
                            new KeyValuePair<string, string>("login", login),
                            new KeyValuePair<string, string>("password", pass)
                        });
                var response = await client.PostAsync(ServerAddress, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<bool> AuthorizeAsync(string login, string pass)
        {
            try
            {
                var content = await GetContent(login, pass);
                var success = content.IndexOf("Неверно указаны логин или пароль", StringComparison.Ordinal) <= 0;
                if (success)
                {
                    _login = login;
                    _pass = pass;
                }
                return success;
            }
            catch (Exception)
            {
                throw new SiteGatewayException("Что-то пошло не так");
            }
        }

        public async Task<double> GetBalanceAsync()
        {
            try
            {
                var content = await GetContent(_login, _pass);
                var balanceIndex = content.IndexOf("Баланс", StringComparison.Ordinal) + 10;
                content = content.Substring(balanceIndex, content.Length - balanceIndex);
                var regex = new Regex("<td (.*?)>(.*?)</td>");
                var balance = regex.Match(content).Groups[2].Value;
                return double.Parse(balance, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new SiteGatewayException("Что-то пошло не так");
            }
        }
    }
}