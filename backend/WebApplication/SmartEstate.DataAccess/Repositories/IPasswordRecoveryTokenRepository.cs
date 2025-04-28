using DatabaseModel;
using System.Threading.Tasks;
using DatabaseModels.RecoveryPassword;

namespace SmartEstate.DataAccess.Repositories
{
    public interface IPasswordRecoveryTokenRepository
    {
        Task<PasswordRecoveryToken?> GetValidTokenAsync(string token);
        Task CreateTokenAsync(PasswordRecoveryToken token);
        Task InvalidateUserTokensAsync(Guid userId);
    }
}