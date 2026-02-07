using System.Reflection.Metadata;
using System.Text;
using cinema.utilitarios;

namespace cinema.modelos.UsuarioModelo
{
    public abstract class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public DateTime DataCadastro { get; set; }

        public Usuario(int id, string nome, string email, string senha, DateTime dataCadastro)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Senha = senha;
            DataCadastro = dataCadastro;
        }

        //metódos
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Detalhes do Usuário:");
            sb.AppendLine($"ID: {Id}");
            sb.AppendLine($"Nome: {Nome}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Data de Cadastro: {FormatadorData.FormatarData(DataCadastro)}");
            return sb.ToString();
        } 
    }


}




