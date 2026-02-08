using System.Text;
using cineflow.enumeracoes;
using cineflow.modelos.IngressoModelo;
namespace cineflow.modelos
{
    public class Assento
    {
        public int Id { get; set; }
        public char Fila { get; set; } // Exemplo: 'A', 'B', etc.
        public int Numero { get; set; } // Exemplo: 1, 2, etc.
        public bool Disponivel { get; set; }

        public TipoAssento Tipo { get; set; }
        public int QuantidadeLugares { get; set; }

        public Sala Sala { get; set; }

        public Ingresso? Ingresso { get; set; }

        // Inclui tipo e quantidade para suportar PCD e casal.
        public Assento(int id, char fila, int numero, Sala sala, Ingresso? ingresso, TipoAssento tipo, int quantidadeLugares)
        {
            Id = id;
            Fila = fila;
            Numero = numero;
            Disponivel = true;
            Sala = sala;
            Ingresso = ingresso;
            Tipo = tipo;
            QuantidadeLugares = quantidadeLugares;
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
            sb.AppendLine($"Tipo: {Tipo}");
            sb.AppendLine($"Lugares: {QuantidadeLugares}");

            return sb.ToString();
        }
    }
}




