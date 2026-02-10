using System.ComponentModel.DataAnnotations;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um ingresso de entrada inteira na WebAPI
    /// </summary>
    public class IngressoInteira : Ingresso
    {
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public decimal Preco { get; set; }

        public IngressoInteira() : base() { }

        public IngressoInteira(decimal preco, int id, char fila, int numero, DateTime dataCompra,
            Sessao? sessao = null, Cliente? cliente = null, Assento? assento = null)
            : base(id, fila, numero, dataCompra, sessao, cliente, assento)
        {
            Preco = preco;
        }

        public override decimal CalcularPreco(decimal precoBase)
        {
            return Preco;
        }

        public override string ObterTipo()
        {
            return "Inteira";
        }
    }
}
