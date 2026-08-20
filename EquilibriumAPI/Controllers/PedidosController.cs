using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Services.Interfaces;

namespace EquilibriumAPI.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidosService _pedidosService;

        public PedidosController(IPedidosService pedidosService) => _pedidosService = pedidosService;

        /// <summary>Lista os pedidos do usuário autenticado</summary>
        [HttpGet]
        public IActionResult Listar()
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            return Ok(_pedidosService.ListarPorUsuario(usuarioId));
        }

        /// <summary>Lista todos os pedidos (somente admin)</summary>
        [HttpGet("todos")]
        [Authorize(Roles = "admin,Admin,ADMIN")]
        public IActionResult ListarTodos()
        {
            return Ok(_pedidosService.ListarTodos());
        }

        /// <summary>Cria um novo pedido (compra de ingressos)</summary>
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPedidoDto dto)
        {
            var usuarioId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
            try
            {
                var pedido = await _pedidosService.CriarAsync(usuarioId, dto);
                return Created($"/api/pedidos/{pedido.Id}", pedido);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
