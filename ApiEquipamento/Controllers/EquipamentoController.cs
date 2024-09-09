namespace ApiEquipamento.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [Route("api/[controller]")]
    [ApiController]
    public class EquipamentoController : ControllerBase
    {
        private const string Arquivo = "Dados Equipamento.txt";

        [HttpGet]
        public IActionResult Get()
        {
            var equipamentos = LerArquivo();
            return Ok(equipamentos);
        }

        private void GravarArquivo(List<Equipamento> equipamentos)
        {
            var linhas = equipamentos.Select(e => $"{e.Nome}|" +
                $"{e.Descricao}|" + $"{e.Codigo}|" + $"{e.Quantidade}|" + $"{e.Valor}|" + $"{e.Fornecedor}");
            System.IO.File.WriteAllLines(Arquivo, linhas);
        }

        private List<Equipamento> LerArquivo()
        {
            var equipamentos = new List<Equipamento>();

            if (!System.IO.File.Exists(Arquivo))
            {
                return equipamentos;
            }

            var linhas = System.IO.File.ReadAllLines(Arquivo);
            foreach (var linha in linhas)
            {
                var dados = linha.Split('|');
                equipamentos.Add(new Equipamento
                {
                    Nome = dados[0],
                    Descricao = dados[1],
                    Codigo = dados[2],
                    Quantidade = dados[3],
                    Valor = dados[4],
                    Fornecedor = dados[5]
                });
            }

            return equipamentos;
        }

        [HttpPost]
        public IActionResult Post([FromBody] EquipamentoDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Dados inválidos.");
            }

            var equipamentos = LerArquivo();

            var equipamento = new Equipamento
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Codigo = dto.Codigo,
                Quantidade = dto.Quantidade,
                Valor = dto.Valor,
                Fornecedor = dto.Fornecedor
            };

            equipamentos.Add(equipamento);
            GravarArquivo(equipamentos);

            // Retorna um status HTTP 201 (Created) com o local do novo recurso
            return CreatedAtAction(nameof(GetByCodigo), new { codigo = equipamento.Codigo }, equipamento);
        }

        [HttpGet("{codigo}")]
        public IActionResult GetByCodigo(string codigo)
        {
            var equipamentos = LerArquivo();
            var equipamento = equipamentos.FirstOrDefault(e => e.Codigo == codigo);

            if (equipamento == null)
            {
                return NotFound();
            }

            return Ok(equipamento);
        }

        [HttpPut("{codigo}")]
        public IActionResult Put(string codigo, [FromBody] EquipamentoDTO dto)
        {
            var equipamentos = LerArquivo();
            var equipamento = equipamentos.FirstOrDefault(e => e.Codigo == codigo);

            if (equipamento == null)
            {
                return NotFound();
            }

            equipamento.Nome = dto.Nome ?? equipamento.Nome;
            equipamento.Descricao = dto.Descricao ?? equipamento.Descricao;
            equipamento.Quantidade = dto.Quantidade ?? equipamento.Quantidade;
            equipamento.Valor = dto.Valor ?? equipamento.Valor;
            equipamento.Fornecedor = dto.Fornecedor ?? equipamento.Fornecedor;

            GravarArquivo(equipamentos);

            return Ok(equipamento);
        }

        [HttpDelete("{codigo}")]
        public IActionResult Delete(string codigo)
        {
            var equipamentos = LerArquivo();
            var equipamento = equipamentos.FirstOrDefault(e => e.Codigo == codigo);

            if (equipamento == null)
            {
                return NotFound();
            }

            equipamentos.Remove(equipamento);
            GravarArquivo(equipamentos);

            return Ok(equipamento);
        }
    }
}