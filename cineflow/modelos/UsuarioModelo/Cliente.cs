using System.Text;
using cineflow.modelos.IngressoModelo;
using cineflow.utilitarios;
namespace cineflow.modelos.UsuarioModelo
{
    public class Cliente : Usuario
    {
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public DateTime dataNascimento { get; set; }

        public List<Ingresso> Ingressos { get; set; }
        public List<PedidoAlimento> Pedidos { get; set; }
        public List<ProdutoAlimento> Cortesias { get; set; }
        public int PontosFidelidade { get; set; }

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
            Cortesias = new List<ProdutoAlimento>();
            PontosFidelidade = 0;
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

        public bool EhAniversario(DateTime data)
        {
            return dataNascimento.Day == data.Day && dataNascimento.Month == data.Month;
        }

        public bool EhMesAniversario(DateTime data)
        {
            return dataNascimento.Month == data.Month;
        }

        public int ObterIdade(DateTime data)
        {
            int idade = data.Year - dataNascimento.Year;
            if (data.Month < dataNascimento.Month ||
                (data.Month == dataNascimento.Month && data.Day < dataNascimento.Day))
            {
                idade--;
            }
            return idade;
        }

        public void AdicionarPontos(int pontos)
        {
            if (pontos > 0)
            {
                PontosFidelidade += pontos;
            }
        }

        public bool TentarUsarPontos(int pontos)
        {
            if (pontos <= 0 || PontosFidelidade < pontos)
            {
                return false;
            }

            PontosFidelidade -= pontos;
            return true;
        }
    }
}




