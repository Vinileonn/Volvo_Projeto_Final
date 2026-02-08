using System.Text;
using cineflow.enumeracoes;
using cineflow.modelos.UsuarioModelo;
using cineflow.utilitarios;

namespace cineflow.modelos
{
    public class AluguelSala
    {
        public int Id { get; set; }
        public Cliente? Cliente { get; set; }
        public string NomeCliente { get; set; }
        public string Contato { get; set; }
        public Sala Sala { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public string Motivo { get; set; }
        public decimal Valor { get; set; }
        public StatusAluguel Status { get; set; }
        public bool PacoteAniversario { get; set; }

        public AluguelSala(int id, Sala sala, DateTime inicio, DateTime fim, string nomeCliente, string contato,
            string motivo, decimal valor, StatusAluguel status = StatusAluguel.Solicitado, Cliente? cliente = null,
            bool pacoteAniversario = false)
        {
            Id = id;
            Sala = sala;
            Inicio = inicio;
            Fim = fim;
            NomeCliente = nomeCliente;
            Contato = contato;
            Motivo = motivo;
            Valor = valor;
            Status = status;
            Cliente = cliente;
            PacoteAniversario = pacoteAniversario;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Aluguel:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Cliente: {NomeCliente}");
            sb.AppendLine($"Contato: {Contato}");
            sb.AppendLine($"Sala: {Sala.Nome}");
            sb.AppendLine($"Periodo: {FormatadorData.FormatarDataComHora(Inicio)} - {FormatadorData.FormatarDataComHora(Fim)}");
            sb.AppendLine($"Motivo: {Motivo}");
            sb.AppendLine($"Valor: {FormatadorMoeda.Formatar((float)Valor)}");
            sb.AppendLine($"Status: {Status}");
            if (PacoteAniversario)
            {
                sb.AppendLine("Pacote Aniversario: Sim");
            }
            return sb.ToString();
        }
    }
}
