using tmg.equinox.applicationservices.viewmodels.Account;

namespace tmg.equinox.applicationservices.interfaces
{
    public interface IAccountService
    {
        LoginViewModel FindUser(int tenantId, string username, string password);
        LoginViewModel FindUser(int tenantId, string username);
    }    
}
