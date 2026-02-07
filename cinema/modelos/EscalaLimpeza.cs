using System.Text;

namespace cinema.modelos
{
    public class EscalaLimpeza
    {
        public int Id { get; set; }
        public Sala Sala { get; set; }
        public Funcionario Funcionario { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }

        public EscalaLimpeza(int id, Sala sala, Funcionario funcionario, DateTime inicio, DateTime fim)
        {
            Id = id;
            Sala = sala;
            Funcionario = funcionario;
            Inicio = inicio;
            Fim = fim;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes da Escala de Limpeza:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Sala: {Sala.Nome}");
            sb.AppendLine($"Funcionario: {Funcionario.Nome}");
            sb.AppendLine($"Inicio: {Inicio:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Fim: {Fim:dd/MM/yyyy HH:mm}");
            return sb.ToString();
        }
    }
}





