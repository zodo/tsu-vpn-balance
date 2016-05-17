namespace TsuBalance
{
    using System.Diagnostics.Eventing.Reader;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public interface ISiteGateway
    {
        Task<bool> AuthorizeAsync(string login, string pass);

        Task<double> GetBalanceAsync();
    }
}
