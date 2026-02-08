using System.ComponentModel.DataAnnotations;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um ingresso de entrada inteira na WebAPI
    /// </summary>
    public class IngressoInteira : Ingresso
    {
        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, float.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public float Preco { get; set; }

        public IngressoInteira() : base() { }

        public IngressoInteira(float preco, int id, char fila, int numero, DateTime dataCompra,
            Sessao? sessao = null, Cliente? cliente = null, Assento? assento = null)
            : base(id, fila, numero, dataCompra, sessao, cliente, assento)
        {
            Preco = preco;
        }

        public override float CalcularPreco(float precoBase)
        {
            return Preco;
        }

        public override string ObterTipo()
        {
            return "Inteira";
        }
    }
}
