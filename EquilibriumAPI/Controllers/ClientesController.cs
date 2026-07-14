using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.Data;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClientesController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>Lista todos os clientes (somente admin)</summary>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Listar([FromQuery] string? busca = null)
        {
            var query = _db.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var b = busca.ToLower();
                query = query.Where(u =>
                    u.Nome.ToLower().Contains(b) ||
                    u.Email.ToLower().Contains(b) ||
                    (u.Cpf != null && u.Cpf.Contains(b)));
            }

            var result = query
                .OrderByDescending(u => u.CriadoEm)
                .Select(u => AuthService.MapearUsuario(u))
                .ToList();

            return Ok(result);
        }

        /// <summary>Busca cliente por ID (admin ou o próprio usuário)</summary>
        [HttpGet("{id:guid}")]
        public IActionResult BuscarPorId(Guid id)
        {
            var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && usuarioLogadoId != id)
                return Forbid();

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null) return NotFound(new { mensagem = "Cliente não encontrado." });

            return Ok(AuthService.MapearUsuario(usuario));
        }

        /// <summary>Atualiza perfil de um cliente (admin ou o próprio usuário)</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPerfilDto dto)
        {
            var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            var isAdmin = User.IsInRole("admin");

            if (!isAdmin && usuarioLogadoId != id)
                return Forbid();

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null) return NotFound(new { mensagem = "Cliente não encontrado." });

            if (dto.Nome != null) usuario.Nome = dto.Nome;
            if (dto.Celular != null) usuario.Celular = dto.Celular;
            if (dto.DataNascimento.HasValue) usuario.DataNascimento = dto.DataNascimento;

            await _db.SaveChangesAsync();
            return Ok(AuthService.MapearUsuario(usuario));
        }

        /// <summary>Remove (hard delete) um cliente (somente admin)</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null) return NotFound(new { mensagem = "Usuário não encontrado." });

            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>Alterna o role de um usuário entre admin e client (somente admin)</summary>
        [HttpPatch("{id:guid}/role")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AlternarRole(Guid id)
        {
            var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

            if (usuarioLogadoId == id)
                return BadRequest(new { mensagem = "Você não pode alterar o seu próprio role." });

            var usuario = _db.Usuarios.FirstOrDefault(u => u.Id == id);
            if (usuario == null) return NotFound(new { mensagem = "Usuário não encontrado." });

            usuario.Role = usuario.Role == "admin" ? "client" : "admin";
            await _db.SaveChangesAsync();

            return Ok(AuthService.MapearUsuario(usuario));
        }
    }
}
