using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.excecoes;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Escalas de Limpeza na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LimpezaControlador : ControllerBase
    {
        private readonly LimpezaServico _limpezaServico;
        private readonly SalaServico _salaServico;
        private readonly FuncionarioServico _funcionarioServico;

        public LimpezaControlador(LimpezaServico limpezaServico, SalaServico salaServico, FuncionarioServico funcionarioServico)
        {
            _limpezaServico = limpezaServico;
            _salaServico = salaServico;
            _funcionarioServico = funcionarioServico;
        }

        /// <summary>
        /// Cria uma nova escala de limpeza
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("CriarEscala")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<EscalaLimpeza> CriarEscala([FromBody] CriarEscalaRequest request)
        {
            try
            {
                var sala = _salaServico.ObterSala(request.SalaId);
                var funcionario = _funcionarioServico.ObterFuncionario(request.FuncionarioId);

                _limpezaServico.CriarEscala(sala, funcionario, request.Inicio, request.Fim);
                
                return CreatedAtAction(nameof(ListarEscalas), new { mensagem = "Escala de limpeza criada com sucesso." });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todas as escalas de limpeza
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EscalaLimpeza>> ListarEscalas()
        {
            var escalas = _limpezaServico.ListarEscalas();
            return Ok(escalas);
        }

        /// <summary>
        /// Lista escalas de limpeza por sala
        /// </summary>
        [HttpGet("PorSala/{salaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EscalaLimpeza>> ListarPorSala(int salaId)
        {
            var escalas = _limpezaServico.ListarPorSala(salaId);
            return Ok(escalas);
        }

        /// <summary>
        /// Lista escalas de limpeza por funcionário
        /// </summary>
        [HttpGet("PorFuncionario/{funcionarioId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EscalaLimpeza>> ListarPorFuncionario(int funcionarioId)
        {
            var escalas = _limpezaServico.ListarPorFuncionario(funcionarioId);
            return Ok(escalas);
        }

        /// <summary>
        /// Deleta uma escala de limpeza
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarEscala(int id)
        {
            try
            {
                _limpezaServico.DeletarEscala(id);
                return Ok(new { mensagem = "Escala removida com sucesso." });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }

    public class CriarEscalaRequest
    {
        public int SalaId { get; set; }
        public int FuncionarioId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
    }
}
