using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.DTOs.Filme;
using cinecore.DTOs.Sessao;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Filmes na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FilmeControlador : ControllerBase
    {
        private readonly FilmeServico _filmeServico;
        private readonly IMapper _mapper;

        public FilmeControlador(FilmeServico filmeServico, IMapper mapper)
        {
            _filmeServico = filmeServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo filme
        /// </summary>
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<FilmeDto> CriarFilme([FromBody] CriarFilmeDto criarFilmeDto)
        {
            try
            {
                var filme = _mapper.Map<Filme>(criarFilmeDto);
                var novoFilme = _filmeServico.CriarFilme(filme);
                var filmeDto = _mapper.Map<FilmeDto>(novoFilme);
                
                return CreatedAtAction(nameof(ObterFilme), new { id = filmeDto.Id }, filmeDto);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um filme por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FilmeDto> ObterFilme(int id)
        {
            try
            {
                var filme = _filmeServico.ObterFilme(id);
                var filmeDto = _mapper.Map<FilmeDto>(filme);
                return Ok(filmeDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os filmes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FilmeDto>> ListarFilmes()
        {
            var filmes = _filmeServico.ListarFilmes();
            var filmesDto = _mapper.Map<List<FilmeDto>>(filmes);
            return Ok(filmesDto);
        }

        /// <summary>
        /// Busca filmes por título
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FilmeDto>> BuscarPorTitulo([FromQuery] string titulo)
        {
            var resultado = _filmeServico.BuscarPorTitulo(titulo);
            var resultadoDto = _mapper.Map<List<FilmeDto>>(resultado);
            return Ok(resultadoDto);
        }

        /// <summary>
        /// Atualiza um filme existente
        /// </summary>
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<FilmeDto> AtualizarFilme(int id, [FromBody] AtualizarFilmeDto atualizarFilmeDto)
        {
            try
            {
                var filmeAtualizado = _mapper.Map<Filme>(atualizarFilmeDto);
                var filme = _filmeServico.AtualizarFilme(id, filmeAtualizado);
                var filmeDto = _mapper.Map<FilmeDto>(filme);
                return Ok(filmeDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um filme
        /// </summary>
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarFilme(int id)
        {
            try
            {
                _filmeServico.DeletarFilme(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém as sessões de um filme
        /// </summary>
        [HttpGet("{id}/sessoes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<object> ObterSessoesDoFilme(int id)
        {
            try
            {
                var sessoes = _filmeServico.ObterSessoesDoFilme(id);
                var sessoesDto = _mapper.Map<List<SessaoDto>>(sessoes);
                return Ok(new { quantidade = sessoesDto.Count, sessoes = sessoesDto });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
