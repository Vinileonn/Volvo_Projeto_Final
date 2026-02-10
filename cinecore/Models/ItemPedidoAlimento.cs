using System.ComponentModel.DataAnnotations;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um item de pedido de alimento na WebAPI
    /// </summary>
    public class ItemPedidoAlimento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O preço não pode ser negativo")]
        public decimal Preco { get; set; }

        public ProdutoAlimento? Produto { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public ItemPedidoAlimento() { }

        public ItemPedidoAlimento(int id, int quantidade, decimal preco, ProdutoAlimento? produto = null)
        {
            Id = id;
            Quantidade = quantidade;
            Preco = preco;
            Produto = produto;
            DataCriacao = DateTime.Now;
        }
    }
}
