using System.Text;
using cinema.models.IngressoModel;
using cinema.utils;
namespace cinema.models
{
    public class Sessao
    {
        public int Id { get; set; }
        public DateTime DataHorario { get; set; }
        
        public float Preco { get; set; }

        public Filme Filme { get; set; }

        public Sala Sala { get; set; }

        public List<Ingresso> Ingressos { get; set; } 

        public Sessao(int id, DateTime dataHorario, float preco, Filme filme, Sala sala)
        {
            Id = id;
            DataHorario = dataHorario;
            Preco = preco;
            Filme = filme;
            Sala = sala;
            Ingressos = new List<Ingresso>();
        }

        //métodos
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes da Sessão:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Data e Horário: {FormatadorData.FormatarDataComHora(DataHorario)}");
            sb.AppendLine($"Preço: {FormatadorMoeda.Formatar(Preco)}");
            sb.AppendLine($"Filme: {Filme.Titulo}");
            sb.AppendLine($"Sala: {Sala.Nome}");
            return sb.ToString();
        }
    }
}