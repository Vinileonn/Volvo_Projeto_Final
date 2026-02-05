using System.Reflection.Metadata;
using System.Text;
using cinema.models;
using cinema.models.UsuarioModel;
using cinema.utils;
namespace cinema.models.IngressoModel
{
    public abstract class Ingresso
    {
        public int Id { get; set; }
        
        public char Fila { get; set; } // Exemplo: 'A', 'B', etc.
        public int Numero { get; set; } // Exemplo: 10, 5, etc.

        public Sessao Sessao { get; set; }
        public Cliente Cliente { get; set; }
        public Assento Assento { get; set; }

        public DateTime DataCompra { get; set; }

        public Ingresso(int id, char fila, int numero, Sessao sessao, Cliente cliente, Assento assento, DateTime dataCompra)
        {
            Id = id;
            Fila = fila;
            Numero = numero;
            Sessao = sessao;
            Cliente = cliente;
            Assento = assento;
            DataCompra = dataCompra;
        }

        //métodos
        public abstract float CalcularPreco(float precoBase);
        public abstract string ObterTipo();
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Ingresso:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Sessão ID: {Sessao.Id}");
            sb.AppendLine($"Data da Compra: {FormatadorData.FormatarDataComHora(DataCompra)}");
            sb.AppendLine($"Número do Lugar: {Fila}{Numero}");
            return sb.ToString();
        }
    }
}