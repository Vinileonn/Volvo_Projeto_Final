using System.Text;

namespace cinema.models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Capacidade { get; set; }

        public List<Assento> Assentos { get; set; }

        public Sala(int id, string nome, int capacidade)
        {
            Id = id;
            Nome = nome;
            Capacidade = capacidade;
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
            return sb.ToString();
        }

        public string VisualizarLayout()
        {
            if (Assentos.Count == 0)
            {
                return "Nenhum assento gerado nesta sala.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"╔═══ LAYOUT DA SALA: {Nome} ═══╗");
            sb.AppendLine();

            char filaAtual = 'A';

            foreach (var assento in Assentos)
            {
                // Quebra de linha quando muda a fila
                if (assento.Fila != filaAtual)
                {
                    sb.AppendLine();
                    filaAtual = assento.Fila;
                }
                // Exibe assento disponível ou reservado
                if (assento.Disponivel)
                {
                    sb.Append($"[{assento.Fila}{assento.Numero:D2}] ");
                }
                else
                {
                    sb.Append($"[ X  ] ");
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Legenda: [A01] = Disponível | [ X ] = Reservado");
            sb.AppendLine($"Ocupação: {Assentos.Count(a => !a.Disponivel)} / {Assentos.Count}");

            return sb.ToString();
        } 
    }
}