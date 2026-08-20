using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using EquilibriumAPI.Data;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Models;
using System.Collections.Generic;

using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var usuario = _db.Usuarios
                .FirstOrDefault(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("E-mail ou senha inválidos.");

            var token = GerarToken(usuario);
            return new AuthResponseDto(token, MapearUsuario(usuario));
        }

        public async Task<AuthResponseDto> CadastrarAsync(CadastroDto dto)
        {
            if (_db.Usuarios.Any(u => u.Email == dto.Email))
                throw new InvalidOperationException("E-mail já cadastrado.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Role = "client", // Corrigido para corresponder ao valor esperado pelo Postgres
                Cpf = dto.Cpf != null ? new string(dto.Cpf.Where(char.IsDigit).ToArray()) : null,
                Celular = dto.Celular != null ? new string(dto.Celular.Where(char.IsDigit).ToArray()) : null,
                DataNascimento = dto.DataNascimento,
                CriadoEm = DateTime.UtcNow
            };

            _db.Usuarios.Add(usuario);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                // Captura violações de unicidade do MySQL
                var mysqlEx = ex.InnerException as MySqlConnector.MySqlException;
                if (mysqlEx?.Number == 1062) // Duplicate entry
                {
                    var msg = mysqlEx.Message ?? "";
                    if (msg.Contains("cpf"))
                        throw new InvalidOperationException("Este CPF já está cadastrado.");
                    if (msg.Contains("email"))
                        throw new InvalidOperationException("Este e-mail já está cadastrado.");
                    throw new InvalidOperationException("Dados já cadastrados. Verifique e-mail e CPF.");
                }
                throw;
            }

            var token = GerarToken(usuario);
            return new AuthResponseDto(token, MapearUsuario(usuario));
        }

        public async Task RecuperarSenhaAsync(string email)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null)
                throw new KeyNotFoundException("E-mail não encontrado.");

            await Task.CompletedTask;
        }

        private string GerarToken(Usuario usuario)
        {
            var settings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("nome", usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: settings["Issuer"],
                audience: settings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(settings["ExpirationHours"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static UsuarioDto MapearUsuario(Usuario u) => new(
            u.Id, u.Nome, u.Email, u.Role, u.Cpf, u.Celular, u.DataNascimento
        );
    }
}
