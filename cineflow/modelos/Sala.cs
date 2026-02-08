using System.Text;
using cineflow.enumeracoes;

namespace cineflow.modelos
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Capacidade { get; set; }

        public TipoSala Tipo { get; set; }

        public Cinema? Cinema { get; set; }

        public int QuantidadeAssentosCasal { get; set; }
        public int QuantidadeAssentosPCD { get; set; }

        public List<Assento> Assentos { get; set; }

        public Sala(int id, string nome, int capacidade, Cinema? cinema, TipoSala tipo = TipoSala.Normal,
            int quantidadeAssentosCasal = 0, int quantidadeAssentosPCD = 0)
        {
            Id = id;
            Nome = nome;
            Capacidade = capacidade;
            Cinema = cinema;
            Tipo = tipo;
            QuantidadeAssentosCasal = quantidadeAssentosCasal;
            QuantidadeAssentosPCD = quantidadeAssentosPCD;
            Assentos = new List<Assento>();
        }

        //método que consulta se um assento existe e retorna ele se existe na sala ou null se não existir
        public Assento? ConsultarAssento(char fila, int numero)
        {
            foreach (var assento in Assentos)
            {
                if (assento.Fila == fila && assento.Numero == numero)
                {
                    return assento;
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes da Sala:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Nome: {Nome}");
            sb.AppendLine($"Capacidade: {Capacidade}");
            sb.AppendLine($"Tipo: {Tipo}");
            sb.AppendLine($"Cinema: {(Cinema != null ? Cinema.Nome : "Nao informado")}");
            sb.AppendLine($"Assentos Casal: {QuantidadeAssentosCasal}");
            sb.AppendLine($"Assentos PCD: {QuantidadeAssentosPCD}");
            return sb.ToString();
        }

        public string VisualizarDisposicao()
        {
            if (Assentos.Count == 0)
            {
                return "Nenhum assento gerado nesta sala.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"╔═══ DISPOSICAO DA SALA: {Nome} ═══╗");
            sb.AppendLine();

            char filaAtual = 'A';

            foreach (var assento in Assentos)
            {
                if (assento.Fila != filaAtual)
                {
                    sb.AppendLine();
                    filaAtual = assento.Fila;
                }

                if (assento.Disponivel)
                {
                    string marcador = assento.Tipo == TipoAssento.PCD ? "P" :
                        assento.Tipo == TipoAssento.Casal ? "C" : " ";
                    sb.Append($"[{assento.Fila}{assento.Numero:D2}{marcador}] ");
                }
                else
                {
                    sb.Append($"[ X  ] ");
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Legenda: [A01 ] = Disponivel | [ X ] = Reservado | P = PCD | C = Casal");
            sb.AppendLine($"Ocupação: {Assentos.Count(a => !a.Disponivel)} / {Assentos.Count}");

            return sb.ToString();
        }
    }
}
