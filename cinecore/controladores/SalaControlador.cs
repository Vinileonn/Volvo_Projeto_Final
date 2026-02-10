using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.DTOs.Sala;
using cinecore.excecoes;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Salas na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SalaControlador : ControllerBase
    {
        private readonly SalaServico _salaServico;
        private readonly CinemaServico _cinemaServico;
        private readonly IMapper _mapper;

        public SalaControlador(SalaServico salaServico, CinemaServico cinemaServico, IMapper mapper)
        {
            _salaServico = salaServico;
            _cinemaServico = cinemaServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova sala
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<SalaDto> CriarSala([FromBody] CriarSalaDto criarSalaDto)
        {
            try
            {
                var cinema = _cinemaServico.ObterCinema(criarSalaDto.CinemaId);
                var sala = _mapper.Map<Sala>(criarSalaDto);
                sala.Cinema = cinema;
                _salaServico.CriarSala(sala);
                
                // Após criar, mapear para DTO para retornar
                var salaDto = _mapper.Map<SalaDto>(sala);
                return CreatedAtAction(nameof(ObterSala), new { id = salaDto.Id }, salaDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma sala por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SalaDto> ObterSala(int id)
        {
            try
            {
                var sala = _salaServico.ObterSala(id);
                var salaDto = _mapper.Map<SalaDto>(sala);
                return Ok(salaDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todas as salas cadastradas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SalaDto>> ListarSalas()
        {
            var salas = _salaServico.ListarSalas();
            var salasDto = _mapper.Map<List<SalaDto>>(salas);
            return Ok(salasDto);
        }

        /// <summary>
        /// Lista todas as salas de um cinema específico
        /// </summary>
        [HttpGet("cinema/{cinemaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SalaDto>> ListarSalasPorCinema(int cinemaId)
        {
            var salas = _salaServico.ListarSalasPorCinema(cinemaId);
            var salasDto = _mapper.Map<List<SalaDto>>(salas);
            return Ok(salasDto);
        }

        /// <summary>
        /// Atualiza uma sala existente
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<SalaDto> AtualizarSala(int id, [FromBody] AtualizarSalaDto atualizarSalaDto)
        {
            try
            {
                _salaServico.AtualizarSala(
                    id,
                    atualizarSalaDto.Nome,
                    atualizarSalaDto.Capacidade,
                    atualizarSalaDto.QuantidadeAssentosCasal,
                    atualizarSalaDto.QuantidadeAssentosPCD);

                var sala = _salaServico.ObterSala(id);
                var salaDto = _mapper.Map<SalaDto>(sala);
                return Ok(salaDto);
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
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta uma sala
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarSala(int id)
        {
            try
            {
                _salaServico.DeletarSala(id);
                return NoContent();
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Visualiza a sala com os assentos disponíveis e ocupados
        /// </summary>
        [HttpGet("Visualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Dictionary<string, List<string>>> VisualizarSala(int id)
        {
            try
            {
                var visualizacao = _salaServico.VisualizarSala(id);
                return Ok(visualizacao);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

    }
}
