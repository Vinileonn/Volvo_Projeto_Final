using System.Text;
using cineflow.modelos.IngressoModelo;
using cineflow.enumeracoes;
using cineflow.utilitarios;
namespace cineflow.modelos
{
    public class Sessao
    {
        public int Id { get; set; }
        public DateTime DataHorario { get; set; }

        public float PrecoBase { get; set; }
        public float PrecoFinal { get; set; }

        public Filme Filme { get; set; }

        public Sala Sala { get; set; }

        public List<Ingresso> Ingressos { get; set; } 

        public TipoSessao Tipo { get; set; }
        public string? NomeEvento { get; set; }
        public string? Parceiro { get; set; }
        public IdiomaSessao Idioma { get; set; }

        public Sessao(int id, DateTime dataHorario, float precoBase, Filme filme, Sala sala,
            TipoSessao tipo = TipoSessao.Regular, string? nomeEvento = null, string? parceiro = null,
            IdiomaSessao idioma = IdiomaSessao.Dublado)
        {
            Id = id;
            DataHorario = dataHorario;
            PrecoBase = precoBase;
            PrecoFinal = precoBase;
            Filme = filme;
            Sala = sala;
            Ingressos = new List<Ingresso>();
            Tipo = tipo;
            NomeEvento = nomeEvento;
            Parceiro = parceiro;
            Idioma = idioma;
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
            sb.AppendLine($"Tipo: {Tipo}");
            sb.AppendLine($"Idioma: {Idioma}");
            if (!string.IsNullOrWhiteSpace(NomeEvento))
            {
                sb.AppendLine($"Evento: {NomeEvento}");
            }
            if (!string.IsNullOrWhiteSpace(Parceiro))
            {
                sb.AppendLine($"Parceiro: {Parceiro}");
            }
            return sb.ToString();
        }
    }
}




