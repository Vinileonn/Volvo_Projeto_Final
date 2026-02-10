using System.ComponentModel.DataAnnotations;
using cinecore.Enums;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar uma sessão de cinema na WebAPI
    /// </summary>
    public class Sessao
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data e horário são obrigatórios")]
        public DateTime DataHorario { get; set; }

        [Required(ErrorMessage = "O preço base é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço base não pode ser negativo")]
        public decimal PrecoBase { get; set; }

        [Required(ErrorMessage = "O preço final é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço final não pode ser negativo")]
        public decimal PrecoFinal { get; set; }

        [Required(ErrorMessage = "O tipo de sessão é obrigatório")]
        public TipoSessao Tipo { get; set; } = TipoSessao.Regular;

        [StringLength(200, ErrorMessage = "O nome do evento deve ter no máximo 200 caracteres")]
        public string? NomeEvento { get; set; }

        [StringLength(200, ErrorMessage = "O nome do parceiro deve ter no máximo 200 caracteres")]
        public string? Parceiro { get; set; }

        [Required(ErrorMessage = "O idioma da sessão é obrigatório")]
        public IdiomaSessao Idioma { get; set; } = IdiomaSessao.Dublado;

        public Filme? Filme { get; set; }

        public Sala? Sala { get; set; }

        public List<Ingresso> Ingressos { get; set; } = new List<Ingresso>();

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public Sessao() { }

        public Sessao(int id, DateTime dataHorario, decimal precoBase, Filme? filme = null, Sala? sala = null,
            TipoSessao tipo = TipoSessao.Regular, string? nomeEvento = null, string? parceiro = null,
            IdiomaSessao idioma = IdiomaSessao.Dublado)
        {
            Id = id;
            DataHorario = dataHorario;
            PrecoBase = precoBase;
            PrecoFinal = precoBase;
            Filme = filme;
            Sala = sala;
            Ingressos = new List<Ingresso>();
            Tipo = tipo;
            NomeEvento = nomeEvento;
            Parceiro = parceiro;
            Idioma = idioma;
            DataCriacao = DateTime.Now;
        }

        public void RecalcularPreco(decimal adicionalSala, decimal adicional3D)
        {
            PrecoFinal = PrecoBase + adicionalSala + adicional3D;
        }
    }
}
