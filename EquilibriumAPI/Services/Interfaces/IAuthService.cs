using System.Threading.Tasks;
using EquilibriumAPI.DTOs;

namespace EquilibriumAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> CadastrarAsync(CadastroDto dto);
        Task RecuperarSenhaAsync(string email);
    }
}
