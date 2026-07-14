using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EquilibriumAPI.Data;
using EquilibriumAPI.DTOs;
using EquilibriumAPI.Models;

namespace EquilibriumAPI.Services
{
    public class CuponsService
    {
        private readonly AppDbContext _db;

        public CuponsService(AppDbContext db) => _db = db;

        public List<CupomDto> ListarTodos() =>
            _db.Cupons.OrderByDescending(c => c.CriadoEm).Select(c => MapearCupom(c)).ToList();

        public ValidarCupomResponseDto Validar(ValidarCupomDto dto)
        {
            var cupom = _db.Cupons.FirstOrDefault(c => c.Codigo.ToLower() == dto.Codigo.ToLower());

            if (cupom == null)
                return new(false, "Cupom não encontrado.", null, null);

            if (cupom.ValidoAte.HasValue && cupom.ValidoAte < DateTime.UtcNow)
                return new(false, "Cupom expirado.", null, null);

            if (cupom.LimiteUsos.HasValue && cupom.TotalUsado >= cupom.LimiteUsos)
                return new(false, "Cupom esgotado.", null, null);

            var desconto = cupom.TipoDesconto == "percentual"
                ? dto.ValorPedido * (cupom.ValorDesconto / 100)
                : cupom.ValorDesconto;

            return new(true, "Cupom válido!", desconto, MapearCupom(cupom));
        }

        public async Task<CupomDto> CriarAsync(CriarCupomDto dto)
        {
            if (_db.Cupons.Any(c => c.Codigo.ToLower() == dto.Codigo.ToLower()))
                throw new InvalidOperationException("Já existe um cupom com esse código.");

            var cupom = new Cupom
            {
                Codigo = dto.Codigo.ToUpper(),
                TipoDesconto = dto.TipoDesconto,
                ValorDesconto = dto.ValorDesconto,
                LimiteUsos = dto.LimiteUsos,
                ValidoAte = dto.ValidoAte,
                CriadoEm = DateTime.UtcNow
            };
            _db.Cupons.Add(cupom);
            await _db.SaveChangesAsync();
            return MapearCupom(cupom);
        }

        public async Task<CupomDto?> EditarAsync(Guid id, CriarCupomDto dto)
        {
            var cupom = _db.Cupons.FirstOrDefault(c => c.Id == id);
            if (cupom == null) return null;

            cupom.Codigo = dto.Codigo.ToUpper();
            cupom.TipoDesconto = dto.TipoDesconto;
            cupom.ValorDesconto = dto.ValorDesconto;
            cupom.LimiteUsos = dto.LimiteUsos;
            cupom.ValidoAte = dto.ValidoAte;

            await _db.SaveChangesAsync();
            return MapearCupom(cupom);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var cupom = _db.Cupons.FirstOrDefault(c => c.Id == id);
            if (cupom == null) return false;
            _db.Cupons.Remove(cupom);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task IncrementarUsoAsync(Guid cupomId)
        {
            var cupom = _db.Cupons.FirstOrDefault(c => c.Id == cupomId);
            if (cupom != null)
            {
                cupom.TotalUsado++;
                await _db.SaveChangesAsync();
            }
        }

        private static CupomDto MapearCupom(Cupom c) => new(
            c.Id, c.Codigo, c.TipoDesconto,
            c.ValorDesconto, c.LimiteUsos,
            c.TotalUsado, c.ValidoAte
        );
    }
}
