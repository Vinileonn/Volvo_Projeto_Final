using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.modelos;
using cinecore.enums;
using cinecore.servicos;
using cinecore.DTOs.AluguelSala;
using cinecore.excecoes;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operacoes de Aluguel de Salas na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AluguelSalaControlador : ControllerBase
    {
        private readonly AluguelSalaServico _aluguelSalaServico;
        private readonly SalaServico _salaServico;
        private readonly IMapper _mapper;

        public AluguelSalaControlador(AluguelSalaServico aluguelSalaServico, SalaServico salaServico, IMapper mapper)
        {
            _aluguelSalaServico = aluguelSalaServico;
            _salaServico = salaServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Solicita um novo aluguel de sala
        /// </summary>
        [HttpPost("Solicitar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<AluguelSalaDto> SolicitarAluguel([FromBody] CriarAluguelSalaDto criarAluguelDto)
        {
            try
            {
                var sala = _salaServico.ObterSala(criarAluguelDto.SalaId);
                var aluguel = _mapper.Map<AluguelSala>(criarAluguelDto);
                aluguel.Sala = sala;

                _aluguelSalaServico.SolicitarAluguel(aluguel);
                var aluguelDto = _mapper.Map<AluguelSalaDto>(aluguel);

                return CreatedAtAction(nameof(ObterAluguel), new { id = aluguelDto.Id }, aluguelDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtem um aluguel por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AluguelSalaDto> ObterAluguel(int id)
        {
            try
            {
                var aluguel = _aluguelSalaServico.ObterAluguel(id);
                var aluguelDto = _mapper.Map<AluguelSalaDto>(aluguel);
                return Ok(aluguelDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os alugueis
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AluguelSalaDto>> ListarAlugueis()
        {
            var alugueis = _aluguelSalaServico.ListarAlugueis();
            var alugueisDto = _mapper.Map<List<AluguelSalaDto>>(alugueis);
            return Ok(alugueisDto);
        }

        /// <summary>
        /// Lista alugueis por status
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AluguelSalaDto>> ListarPorStatus(StatusAluguel status)
        {
            var alugueis = _aluguelSalaServico.ListarPorStatus(status);
            var alugueisDto = _mapper.Map<List<AluguelSalaDto>>(alugueis);
            return Ok(alugueisDto);
        }

        /// <summary>
        /// Lista alugueis de uma sala especifica
        /// </summary>
        [HttpGet("sala/{salaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AluguelSalaDto>> ListarPorSala(int salaId)
        {
            var alugueis = _aluguelSalaServico.ListarPorSala(salaId);
            var alugueisDto = _mapper.Map<List<AluguelSalaDto>>(alugueis);
            return Ok(alugueisDto);
        }

        /// <summary>
        /// Aprova um aluguel e opcionalmente ajusta o valor
        /// </summary>
        [HttpPut("Aprovar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<AluguelSalaDto> AprovarAluguel(int id, [FromQuery] decimal? valor = null)
        {
            try
            {
                _aluguelSalaServico.AprovarAluguel(id, valor);
                var aluguel = _aluguelSalaServico.ObterAluguel(id);
                var aluguelDto = _mapper.Map<AluguelSalaDto>(aluguel);
                return Ok(aluguelDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Cancela um aluguel (apenas com 24h de antecedencia)
        /// </summary>
        [HttpPut("Cancelar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<AluguelSalaDto> CancelarAluguel(int id)
        {
            try
            {
                _aluguelSalaServico.CancelarAluguel(id);
                var aluguel = _aluguelSalaServico.ObterAluguel(id);
                var aluguelDto = _mapper.Map<AluguelSalaDto>(aluguel);
                return Ok(aluguelDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um aluguel existente
        /// </summary>
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<AluguelSalaDto> AtualizarAluguel(int id, [FromBody] AtualizarAluguelSalaDto atualizarAluguelDto)
        {
            try
            {
                _aluguelSalaServico.AtualizarAluguel(
                    id,
                    atualizarAluguelDto.NomeCliente,
                    atualizarAluguelDto.Contato,
                    atualizarAluguelDto.Motivo,
                    atualizarAluguelDto.Inicio,
                    atualizarAluguelDto.Fim,
                    atualizarAluguelDto.PacoteAniversario);

                var aluguel = _aluguelSalaServico.ObterAluguel(id);
                var aluguelDto = _mapper.Map<AluguelSalaDto>(aluguel);
                return Ok(aluguelDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um aluguel
        /// </summary>
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarAluguel(int id)
        {
            try
            {
                _aluguelSalaServico.DeletarAluguel(id);
                return NoContent();
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
