using System.Text;

namespace cineflow.modelos
{
    public class Filme
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Duracao { get; set; }
        public string Genero { get; set; }
        public DateTime AnoLancamento { get; set; }
        public bool Eh3D { get; set; }

        public List<Sessao> Sessoes { get; set; }

        // ANTIGO:
        // public Filme(int id, string titulo, int duracao, string genero, DateTime anoLancamento)
        // {
        //     Id = id;
        //     Titulo = titulo;
        //     Duracao = duracao;
        //     Genero = genero;
        //     AnoLancamento = anoLancamento;
        //     Sessoes = new List<Sessao>();
        // }

        public Filme(int id, string titulo, int duracao, string genero, DateTime anoLancamento, bool eh3D = false)
        {
            Id = id;
            Titulo = titulo;
            Duracao = duracao;
            Genero = genero;
            AnoLancamento = anoLancamento;
            Eh3D = eh3D;
            Sessoes = new List<Sessao>();
        }

        //métodos
        public void AtualizarDetalhes(string? titulo = null, int? duracao = null, string? genero = null, DateTime? anoLancamento = null)
        {
            if (!string.IsNullOrWhiteSpace(titulo))
            {
                Titulo = titulo;
            }

            if (duracao.HasValue)
            {
                Duracao = duracao.Value;
            }

            if (!string.IsNullOrWhiteSpace(genero))
            {
                Genero = genero;
            }
            if (anoLancamento.HasValue)
            {
                AnoLancamento = anoLancamento.Value;
            }
        }

        private (int horas, int minutos) ObterDuracaoFormatada()
        {
            int horas = Duracao / 60;
            int minutos = Duracao % 60;
            return (horas, minutos);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var (horas, minutos) = ObterDuracaoFormatada();
             sb.AppendLine("Detalhes do Filme:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Título: {Titulo}");
            sb.AppendLine($"Duração: {horas}h {minutos}m");
            sb.AppendLine($"Gênero: {Genero}");
            sb.AppendLine($"Ano de Lançamento: {AnoLancamento.Year}");
            sb.AppendLine($"3D: {(Eh3D ? "Sim" : "Nao")}");
            return sb.ToString();
        }
    }
}




