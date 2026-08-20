using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>Realiza login e retorna o token JWT</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
        }

        /// <summary>Cadastra um novo cliente</summary>
        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] CadastroDto dto)
        {
            try
            {
                var result = await _authService.CadastrarAsync(dto);
                return Created("", result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>Solicita recuperação de senha por e-mail</summary>
        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] string email)
        {
            try
            {
                await _authService.RecuperarSenhaAsync(email);
                return Ok(new { mensagem = "Se o e-mail estiver cadastrado, você receberá as instruções." });
            }
            catch (KeyNotFoundException)
            {
                // Retornamos a mesma mensagem por segurança (não vazar se e-mail existe)
                return Ok(new { mensagem = "Se o e-mail estiver cadastrado, você receberá as instruções." });
            }
        }
    }
}
