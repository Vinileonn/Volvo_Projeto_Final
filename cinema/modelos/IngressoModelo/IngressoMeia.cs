using System.Text;
using cinema.modelos;
using cinema.modelos.UsuarioModelo;
using cinema.utilitarios;
namespace cinema.modelos.IngressoModelo
{
    public class IngressoMeia : Ingresso
    {
        public float Preco { get; set; }

        public string Motivo { get; set; } // Exemplo: "Estudante", "Idoso", etc.

        public IngressoMeia(float preco, int id, char fila, int numero, Sessao sessao,
                            Cliente cliente, Assento assento, DateTime dataCompra, string motivo)
                            : base(id, fila, numero, sessao, cliente, assento, dataCompra)
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"Tipo: {ObterTipo()}");
            sb.AppendLine($"Preço: {FormatadorMoeda.Formatar(CalcularPreco(Preco))}");
            return sb.ToString();
        }
    }
} 




