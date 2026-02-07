using System.Text;
using cineflow.modelos.IngressoModelo;
using cineflow.utilitarios;
namespace cineflow.modelos
{
    public class Sessao
    {
        public int Id { get; set; }
        public DateTime DataHorario { get; set; }

        // ANTIGO:
        // public float Preco { get; set; }

        public float PrecoBase { get; set; }
        public float PrecoFinal { get; set; }

        public Filme Filme { get; set; }

        public Sala Sala { get; set; }

        public List<Ingresso> Ingressos { get; set; } 

        // ANTIGO:
        // public Sessao(int id, DateTime dataHorario, float preco, Filme filme, Sala sala)
        // {
        //     Id = id;
        //     DataHorario = dataHorario;
        //     Preco = preco;
        //     Filme = filme;
        //     Sala = sala;
        //     Ingressos = new List<Ingresso>();
        // }

        public Sessao(int id, DateTime dataHorario, float precoBase, Filme filme, Sala sala)
        {
            Id = id;
            DataHorario = dataHorario;
            PrecoBase = precoBase;
            PrecoFinal = precoBase;
            Filme = filme;
            Sala = sala;
            Ingressos = new List<Ingresso>();
        }

        public void RecalcularPreco(float adicionalSala, float adicional3D)
        {
            PrecoFinal = PrecoBase + adicionalSala + adicional3D;
        }

        //métodos
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes da Sessão:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Data e Horário: {FormatadorData.FormatarDataComHora(DataHorario)}");
            sb.AppendLine($"Preço: {FormatadorMoeda.Formatar(PrecoFinal)}");
            sb.AppendLine($"Filme: {Filme.Titulo}");
            sb.AppendLine($"Sala: {Sala.Nome}");
            return sb.ToString();
        }
    }
}




