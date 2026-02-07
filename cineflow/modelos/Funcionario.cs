using System.Text;
using cineflow.enumeracoes;

namespace cineflow.modelos
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public CargoFuncionario Cargo { get; set; }
        public Cinema? Cinema { get; set; }

        public Funcionario(int id, string nome, CargoFuncionario cargo, Cinema? cinema)
        {
            Id = id;
            Nome = nome;
            Cargo = cargo;
            Cinema = cinema;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Funcionario:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Nome: {Nome}");
            sb.AppendLine($"Cargo: {Cargo}");
            sb.AppendLine($"Cinema: {(Cinema != null ? Cinema.Nome : "Nao informado")}");
            return sb.ToString();
        }
    }
}





