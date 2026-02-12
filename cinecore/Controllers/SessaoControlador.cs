using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.Models;
using cinecore.Services;
using cinecore.DTOs.Sessao;
using cinecore.Exceptions;

namespace cinecore.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações de Sessões na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SessaoControlador : ControllerBase
    {
        private readonly SessaoServico _sessaoServico;
        private readonly FilmeServico _filmeServico;
        private readonly SalaServico _salaServico;
        private readonly IMapper _mapper;

        public SessaoControlador(SessaoServico sessaoServico, FilmeServico filmeServico, SalaServico salaServico, IMapper mapper)
        {
            _sessaoServico = sessaoServico;
            _filmeServico = filmeServico;
            _salaServico = salaServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova sessão
        /// </summary>

        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<SessaoDto> CriarSessao([FromBody] CriarSessaoDto criarSessaoDto)
        {
            try
            {
                var filme = _filmeServico.ObterFilme(criarSessaoDto.FilmeId);
                var sala = _salaServico.ObterSala(criarSessaoDto.SalaId);
                var sessao = _mapper.Map<Sessao>(criarSessaoDto);
                sessao.Filme = filme;
                sessao.Sala = sala;

                _sessaoServico.CriarSessao(sessao);
                var sessaoDto = _mapper.Map<SessaoDto>(sessao);

                return CreatedAtAction(nameof(ObterSessao), new { id = sessaoDto.Id }, sessaoDto);
            }
            catch (KeyNotFoundException ex)
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
        /// Obtém uma sessão por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SessaoDto> ObterSessao(int id)
        {
            try
            {
                var sessao = _sessaoServico.ObterSessao(id);
                var sessaoDto = _mapper.Map<SessaoDto>(sessao);
                return Ok(sessaoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todas as sessões
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SessaoDto>> ListarSessoes()
        {
            var sessoes = _sessaoServico.ListarSessoes();
            var sessoesDto = _mapper.Map<List<SessaoDto>>(sessoes);
            return Ok(sessoesDto);
        }

        /// <summary>
        /// Lista sessões de um filme específico
        /// </summary>
        [HttpGet("filme/{filmeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SessaoDto>> ListarSessoesPorFilme(int filmeId)
        {
            var sessoes = _sessaoServico.ListarSessoesPorFilme(filmeId);
            var sessoesDto = _mapper.Map<List<SessaoDto>>(sessoes);
            return Ok(sessoesDto);
        }

        /// <summary>
        /// Lista sessões de uma sala específica
        /// </summary>
        [HttpGet("sala/{salaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<SessaoDto>> ListarSessoesPorSala(int salaId)
        {
            var sessoes = _sessaoServico.ListarSessoesPorSala(salaId);
            var sessoesDto = _mapper.Map<List<SessaoDto>>(sessoes);
            return Ok(sessoesDto);
        }

        /// <summary>
        /// Atualiza uma sessão existente
        /// </summary>

        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<SessaoDto> AtualizarSessao(int id, [FromBody] AtualizarSessaoDto atualizarSessaoDto)
        {
            try
            {
                var sessao = _sessaoServico.ObterSessao(id);
                
                var filme = atualizarSessaoDto.FilmeId.HasValue ? _filmeServico.ObterFilme(atualizarSessaoDto.FilmeId.Value) : sessao.Filme;
                var sala = atualizarSessaoDto.SalaId.HasValue ? _salaServico.ObterSala(atualizarSessaoDto.SalaId.Value) : sessao.Sala;

                _sessaoServico.AtualizarSessao(
                    id,
                    atualizarSessaoDto.DataHorario,
                    atualizarSessaoDto.PrecoBase,
                    filme,
                    sala,
                    atualizarSessaoDto.Tipo,
                    atualizarSessaoDto.NomeEvento,
                    atualizarSessaoDto.Parceiro,
                    atualizarSessaoDto.Idioma
                );

                var sessaoAtualizada = _sessaoServico.ObterSessao(id);
                var sessaoDto = _mapper.Map<SessaoDto>(sessaoAtualizada);
                return Ok(sessaoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (KeyNotFoundException ex)
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
        /// Deleta uma sessão
        /// </summary>

        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarSessao(int id)
        {
            try
            {
                _sessaoServico.DeletarSessao(id);
                return NoContent();
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
