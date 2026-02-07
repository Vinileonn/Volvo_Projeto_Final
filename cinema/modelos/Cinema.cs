using System.Text;

namespace cinema.modelos
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public List<Sala> Salas { get; set; }
        public List<Funcionario> Funcionarios { get; set; }

        // ANTIGO:
        // public Cinema(int id, string nome, string endereco)
        // {
        //     Id = id;
        //     Nome = nome;
        //     Endereco = endereco;
        //     Salas = new List<Sala>();
        // }

        // Mantem funcionarios no cinema para facilitar consultas por unidade.
        public Cinema(int id, string nome, string endereco)
        {
            Id = id;
            Nome = nome;
            Endereco = endereco;
            Salas = new List<Sala>();
            Funcionarios = new List<Funcionario>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Cinema:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Nome: {Nome}");
            sb.AppendLine($"Endereco: {Endereco}");
            sb.AppendLine($"Salas: {Salas.Count}");
            sb.AppendLine($"Funcionarios: {Funcionarios.Count}");
            return sb.ToString();
        }
    }
}





