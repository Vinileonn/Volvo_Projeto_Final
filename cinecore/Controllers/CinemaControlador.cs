using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.Models;
using cinecore.Services;
using cinecore.DTOs.Cinema;
using cinecore.DTOs.Sala;
using cinecore.DTOs.Funcionario;

namespace cinecore.Controllers
{
    /// <summary>
    /// Controller para gerenciar operações de Cinemas na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CinemaControlador : ControllerBase
    {
        private readonly CinemaServico _cinemaServico;
        private readonly IMapper _mapper;

        public CinemaControlador(CinemaServico cinemaServico, IMapper mapper)
        {
            _cinemaServico = cinemaServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo cinema
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<CinemaDto> CriarCinema([FromBody] CriarCinemaDto criarCinemaDto)
        {
            try
            {
                var cinema = _mapper.Map<Cinema>(criarCinemaDto);
                var novoCinema = _cinemaServico.CriarCinema(cinema);
                var cinemaDto = _mapper.Map<CinemaDto>(novoCinema);
                
                return CreatedAtAction(nameof(ObterCinema), new { id = cinemaDto.Id }, cinemaDto);
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
        /// Obtém um cinema por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CinemaDto> ObterCinema(int id)
        {
            try
            {
                var cinema = _cinemaServico.ObterCinema(id);
                var cinemaDto = _mapper.Map<CinemaDto>(cinema);
                return Ok(cinemaDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os cinemas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CinemaDto>> ListarCinemas()
        {
            var cinemas = _cinemaServico.ListarCinemas();
            var cinemasDto = _mapper.Map<List<CinemaDto>>(cinemas);
            return Ok(cinemasDto);
        }

        /// <summary>
        /// Busca cinemas por nome
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CinemaDto>> BuscarPorNome([FromQuery] string nome)
        {
            var resultado = _cinemaServico.BuscarPorNome(nome);
            var resultadoDto = _mapper.Map<List<CinemaDto>>(resultado);
            return Ok(resultadoDto);
        }

        /// <summary>
        /// Atualiza um cinema existente
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<CinemaDto> AtualizarCinema(int id, [FromBody] AtualizarCinemaDto atualizarCinemaDto)
        {
            try
            {
                var cinemaAtualizado = _mapper.Map<Cinema>(atualizarCinemaDto);
                var cinema = _cinemaServico.AtualizarCinema(id, cinemaAtualizado);
                var cinemaDto = _mapper.Map<CinemaDto>(cinema);
                return Ok(cinemaDto);
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
        /// Deleta um cinema
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarCinema(int id)
        {
            try
            {
                _cinemaServico.DeletarCinema(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém as salas de um cinema
        /// </summary>
        [HttpGet("{id}/salas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<SalaDto>> ObterSalasDoCinema(int id)
        {
            try
            {
                var salas = _cinemaServico.ObterSalasDoCinema(id);
                var salasDto = _mapper.Map<List<SalaDto>>(salas);
                return Ok(salasDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os funcionários de um cinema
        /// </summary>
        [HttpGet("{id}/funcionarios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<FuncionarioDto>> ObterFuncionariosDoCinema(int id)
        {
            try
            {
                var funcionarios = _cinemaServico.ObterFuncionariosDoCinema(id);
                var funcionariosDto = _mapper.Map<List<FuncionarioDto>>(funcionarios);
                return Ok(funcionariosDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
