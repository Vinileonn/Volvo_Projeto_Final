using System.Text;
using cinema.modelos.IngressoModelo;
using cinema.utilitarios;
namespace cinema.modelos.UsuarioModelo
{
    public class Cliente : Usuario
    {
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public DateTime dataNascimento { get; set; }

        public List<Ingresso> Ingressos { get; set; }
        public List<PedidoAlimento> Pedidos { get; set; }

        public Cliente(int id, string nome, string email, string senha, 
                        string cpf, string telefone, string endereco, DateTime dataNascimento)
                        : base(id, nome, email, senha, DateTime.Now)
        {
            CPF = cpf;
            Telefone = telefone;
            Endereco = endereco;
            this.dataNascimento = dataNascimento;
            Ingressos = new List<Ingresso>();
            Pedidos = new List<PedidoAlimento>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine($"CPF: {CPF}");
            sb.AppendLine($"Telefone: {Telefone}");
            sb.AppendLine($"Endereço: {Endereco}");
            sb.AppendLine($"Data de Nascimento: {FormatadorData.FormatarData(dataNascimento)}");
            return sb.ToString();
        }
    }
}




