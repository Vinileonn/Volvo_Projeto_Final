using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using cinecore.modelos;
using cinecore.servicos;
using cinecore.enums;
using cinecore.excecoes;
using cinecore.DTOs.Ingresso;

namespace cinecore.controladores
{
    /// <summary>
    /// Controller para gerenciar operações de Ingressos na WebAPI
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IngressoControlador : ControllerBase
    {
        private readonly IngressoServico _ingressoServico;
        private readonly SessaoServico _sessaoServico;
        private readonly UsuarioServico _usuarioServico;
        private readonly IMapper _mapper;

        public IngressoControlador(IngressoServico ingressoServico, SessaoServico sessaoServico, UsuarioServico usuarioServico, IMapper mapper)
        {
            _ingressoServico = ingressoServico;
            _sessaoServico = sessaoServico;
            _usuarioServico = usuarioServico;
            _mapper = mapper;
        }

        /// <summary>
        /// Vende um ingresso inteiro
        /// </summary>
        [HttpPost("VenderInteira")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngressoDto> VenderInteira([FromBody] VenderIngressoInteiraRequest request)
        {
            try
            {
                var sessao = _sessaoServico.ObterSessao(request.SessaoId);
                var cliente = _usuarioServico.ObterUsuario(request.ClienteId) as Cliente;

                if (cliente == null)
                {
                    return BadRequest(new { mensagem = "Usuário informado não é um cliente." });
                }

                var ingresso = _ingressoServico.VenderInteira(
                    sessao,
                    cliente,
                    request.Fila,
                    request.Numero,
                    request.FormaPagamento,
                    request.ValorPago,
                    request.CupomParceiro,
                    request.ReservaAntecipada,
                    request.PontosUsados
                );

                var ingressoDto = _mapper.Map<IngressoDto>(ingresso);
                return CreatedAtAction(nameof(ObterIngresso), new { id = ingresso.Id }, ingressoDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = $"Recurso não encontrado: {ex.Message}" });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = $"Operação não permitida: {ex.Message}" });
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return StatusCode(500, new { mensagem = $"Erro crítico: {ex.Message}" });
            }
        }

        /// <summary>
        /// Vende uma meia entrada
        /// </summary>
        [HttpPost("VenderMeia")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngressoDto> VenderMeia([FromBody] VenderIngressoMeiaRequest request)
        {
            try
            {
                var sessao = _sessaoServico.ObterSessao(request.SessaoId);
                var cliente = _usuarioServico.ObterUsuario(request.ClienteId) as Cliente;

                if (cliente == null)
                {
                    return BadRequest(new { mensagem = "Usuário informado não é um cliente." });
                }

                var ingresso = _ingressoServico.VenderMeia(
                    sessao,
                    cliente,
                    request.Fila,
                    request.Numero,
                    request.Motivo,
                    request.FormaPagamento,
                    request.ValorPago,
                    request.CupomParceiro,
                    request.ReservaAntecipada,
                    request.PontosUsados
                );

                var ingressoDto = _mapper.Map<IngressoDto>(ingresso);
                return CreatedAtAction(nameof(ObterIngresso), new { id = ingresso.Id }, ingressoDto);
            }
            catch (DadosInvalidosExcecao ex)
            {
                return BadRequest(new { mensagem = $"Dados inválidos: {ex.Message}" });
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = $"Recurso não encontrado: {ex.Message}" });
            }
            catch (OperacaoNaoPermitidaExcecao ex)
            {
                return BadRequest(new { mensagem = $"Operação não permitida: {ex.Message}" });
            }
            catch (ErroOperacaoCriticaExcecao ex)
            {
                return StatusCode(500, new { mensagem = $"Erro crítico: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém um ingresso por ID
        /// </summary>
        [HttpGet("Obter/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IngressoDto> ObterIngresso(int id)
        {
            try
            {
                var ingresso = _ingressoServico.ObterIngresso(id);
                var ingressoDto = _mapper.Map<IngressoDto>(ingresso);
                return Ok(ingressoDto);
            }
            catch (RecursoNaoEncontradoExcecao ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Lista todos os ingressos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<IngressoDto>> ListarIngressos()
        {
            var ingressos = _ingressoServico.ListarIngressos();
            var ingressosDto = _mapper.Map<List<IngressoDto>>(ingressos);
            return Ok(ingressosDto);
        }

        /// <summary>
        /// Cancela um ingresso
        /// </summary>
        [HttpDelete("Cancelar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult CancelarIngresso(int id)
        {
            try
            {
                _ingressoServico.CancelarIngresso(id);
                return Ok(new { mensagem = "Ingresso cancelado com sucesso." });
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
        /// Realiza check-in de um ingresso
        /// </summary>
        [HttpPost("CheckIn/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult RealizarCheckIn(int id)
        {
            try
            {
                _ingressoServico.RealizarCheckIn(id);
                return Ok(new { mensagem = "Check-in realizado com sucesso." });
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
    }

    public class VenderIngressoInteiraRequest
    {
        public int SessaoId { get; set; }
        public int ClienteId { get; set; }
        public char Fila { get; set; }
        public int Numero { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorPago { get; set; } = 0m;
        public string? CupomParceiro { get; set; }
        public bool ReservaAntecipada { get; set; } = false;
        public int PontosUsados { get; set; } = 0;
    }

    public class VenderIngressoMeiaRequest
    {
        public int SessaoId { get; set; }
        public int ClienteId { get; set; }
        public char Fila { get; set; }
        public int Numero { get; set; }
        public required string Motivo { get; set; }
        public FormaPagamento FormaPagamento { get; set; }
        public decimal ValorPago { get; set; } = 0m;
        public string? CupomParceiro { get; set; }
        public bool ReservaAntecipada { get; set; } = false;
        public int PontosUsados { get; set; } = 0;
    }
}
