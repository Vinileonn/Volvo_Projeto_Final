using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.Models;
using cinecore.Enums;
using cinecore.Services;
using cinecore.DTOs.Funcionario;
using cinecore.Exceptions;

namespace cinecore.Controllers
{
    /// <summary>
    /// Controller para gerenciar operacoes de Funcionarios na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioControlador : ControllerBase
    {
        private readonly FuncionarioServico _funcionarioServico;
        private readonly CinemaServico _cinemaServico;
        private readonly IMapper _mapper;

        public FuncionarioControlador(FuncionarioServico funcionarioServico, CinemaServico cinemaServico, IMapper mapper)
        {
            _funcionarioServico = funcionarioServico;
            _cinemaServico = cinemaServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria um novo funcionario
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPost("Criar")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioDto> CriarFuncionario([FromBody] CriarFuncionarioDto criarFuncionarioDto)
        {
            try
            {
                var cinema = _cinemaServico.ObterCinema(criarFuncionarioDto.CinemaId);
                var funcionario = _mapper.Map<Funcionario>(criarFuncionarioDto);
                funcionario.Cinema = cinema;

                _funcionarioServico.CriarFuncionario(funcionario);
                var funcionarioDto = _mapper.Map<FuncionarioDto>(funcionario);

                return CreatedAtAction(nameof(ObterFuncionario), new { id = funcionarioDto.Id }, funcionarioDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Obtem um funcionario por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioDto> ObterFuncionario(int id)
        {
            try
            {
                var funcionario = _funcionarioServico.ObterFuncionario(id);
                var funcionarioDto = _mapper.Map<FuncionarioDto>(funcionario);
                return Ok(funcionarioDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os funcionarios
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FuncionarioDto>> ListarFuncionarios()
        {
            var funcionarios = _funcionarioServico.ListarFuncionarios();
            var funcionariosDto = _mapper.Map<List<FuncionarioDto>>(funcionarios);
            return Ok(funcionariosDto);
        }

        /// <summary>
        /// Lista funcionarios por cargo
        /// </summary>
        [HttpGet("cargo/{cargo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FuncionarioDto>> ListarPorCargo(CargoFuncionario cargo)
        {
            var funcionarios = _funcionarioServico.ListarPorCargo(cargo);
            var funcionariosDto = _mapper.Map<List<FuncionarioDto>>(funcionarios);
            return Ok(funcionariosDto);
        }

        /// <summary>
        /// Lista funcionarios por cinema
        /// </summary>
        [HttpGet("cinema/{cinemaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FuncionarioDto>> ListarPorCinema(int cinemaId)
        {
            var funcionarios = _funcionarioServico.ListarPorCinema(cinemaId);
            var funcionariosDto = _mapper.Map<List<FuncionarioDto>>(funcionarios);
            return Ok(funcionariosDto);
        }

        /// <summary>
        /// Atualiza um funcionario existente
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<FuncionarioDto> AtualizarFuncionario(int id, [FromBody] AtualizarFuncionarioDto atualizarFuncionarioDto)
        {
            try
            {
                Cinema? cinema = null;
                if (atualizarFuncionarioDto.CinemaId.HasValue)
                {
                    cinema = _cinemaServico.ObterCinema(atualizarFuncionarioDto.CinemaId.Value);
                }

                _funcionarioServico.AtualizarFuncionario(
                    id,
                    atualizarFuncionarioDto.Nome,
                    atualizarFuncionarioDto.Cargo,
                    cinema);

                var funcionario = _funcionarioServico.ObterFuncionario(id);
                var funcionarioDto = _mapper.Map<FuncionarioDto>(funcionario);
                return Ok(funcionarioDto);
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
        }

        /// <summary>
        /// Deleta um funcionario
        /// </summary>
        [Authorize(Policy = "AdministradorOnly")]
        [HttpDelete("Deletar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeletarFuncionario(int id)
        {
            try
            {
                _funcionarioServico.DeletarFuncionario(id);
                return NoContent();
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
