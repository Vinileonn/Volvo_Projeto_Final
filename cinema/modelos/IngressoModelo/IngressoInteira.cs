using System.Text;
using cinema.modelos;
using cinema.modelos.UsuarioModelo;
using cinema.utilitarios;
namespace cinema.modelos.IngressoModelo
{
    public class IngressoInteira : Ingresso
    {
        public float Preco { get; set; }

        public IngressoInteira(float preco, int id, char fila, int numero, Sessao sessao, 
                               Cliente cliente, Assento assento, DateTime dataCompra)
                               : base(id, fila, numero, sessao, cliente, assento, dataCompra)
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




