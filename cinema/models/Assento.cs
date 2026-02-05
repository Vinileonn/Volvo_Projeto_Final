using System.Text;
using cinema.models.IngressoModel;
namespace cinema.models
{
    public class Assento
    {
        public int Id { get; set; }
        public char Fila { get; set; } // Exemplo: 'A', 'B', etc.
        public int Numero { get; set; } // Exemplo: 1, 2, etc.
        public bool Disponivel { get; set; }

        public Sala Sala { get; set; }

        public Ingresso? Ingresso { get; set; }

        // Construtor
        public Assento(int id, char fila, int numero, Sala sala, Ingresso? ingresso)
        {
            Id = id;
            Fila = fila;
            Numero = numero;
            Disponivel = true;
            Sala = sala;
            Ingresso = ingresso;
        }

        // Métodos
        public void Reservar()
        {
            Disponivel = false;
        }

        public void Liberar()
        {
            Disponivel = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Assento:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Assento: {Fila}{Numero}");
            sb.AppendLine($"Sala ID: {Sala.Id}");

            return sb.ToString();
        }
    }
}