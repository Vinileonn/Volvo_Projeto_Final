using System.ComponentModel.DataAnnotations;
using cinecore.enums;

namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um pedido de alimento na WebAPI
    /// </summary>
    public class PedidoAlimento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A data do pedido é obrigatória")]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O valor total é obrigatório")]
        [Range(0, float.MaxValue, ErrorMessage = "O valor total não pode ser negativo")]
        public float ValorTotal { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "O valor do desconto não pode ser negativo")]
        public float ValorDesconto { get; set; } = 0;

        [StringLength(500, ErrorMessage = "O motivo do desconto deve ter no máximo 500 caracteres")]
        public string? MotivoDesconto { get; set; }

        public FormaPagamento? FormaPagamento { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O valor pago não pode ser negativo")]
        public decimal ValorPago { get; set; } = 0;

        [Range(0, double.MaxValue, ErrorMessage = "O valor do troco não pode ser negativo")]
        public decimal ValorTroco { get; set; } = 0;

        public Dictionary<decimal, int> TrocoDetalhado { get; set; } = new Dictionary<decimal, int>();

        [Range(0, int.MaxValue, ErrorMessage = "Os pontos usados não podem ser negativos")]
        public int PontosUsados { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Os pontos gerados não podem ser negativos")]
        public int PontosGerados { get; set; } = 0;

        [Range(0, float.MaxValue, ErrorMessage = "A taxa de cancelamento não pode ser negativa")]
        public float TaxaCancelamento { get; set; } = 0;

        public Cliente? Cliente { get; set; }

        public List<ItemPedidoAlimento> Itens { get; set; } = new List<ItemPedidoAlimento>();

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public PedidoAlimento() { }

        public PedidoAlimento(int id, float valorTotal)
        {
            Id = id;
            DataPedido = DateTime.Now;
            ValorTotal = valorTotal;
            Itens = new List<ItemPedidoAlimento>();
            DataCriacao = DateTime.Now;
        }

        public void AdicionarItem(ItemPedidoAlimento item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Itens.Add(item);
            ValorTotal += item.Preco * item.Quantidade;
        }
    }
}
