using System.ComponentModel.DataAnnotations;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um ingresso de meia-entrada na WebAPI
    /// </summary>
    public class IngressoMeia : Ingresso
    {
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, float.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public float Preco { get; set; }

        [Required(ErrorMessage = "O motivo é obrigatório")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O motivo deve ter entre 1 e 200 caracteres")]
        public required string Motivo { get; set; }

        public IngressoMeia() : base() { }

        public IngressoMeia(float preco, int id, char fila, int numero, DateTime dataCompra, string motivo,
            Sessao? sessao = null, Cliente? cliente = null, Assento? assento = null)
            : base(id, fila, numero, dataCompra, sessao, cliente, assento)
        {
            Preco = preco;
            Motivo = motivo;
        }

        public override float CalcularPreco(float precoBase)
        {
            return Preco / 2;
        }

        public override string ObterTipo()
        {
            return $"Meia Entrada ({Motivo})";
        }
    }
}
