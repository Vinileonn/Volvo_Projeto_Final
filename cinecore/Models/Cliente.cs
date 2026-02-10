using System.ComponentModel.DataAnnotations;

namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um cliente na WebAPI
    /// </summary>
    public class Cliente : Usuario
    {
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "O CPF deve ter entre 11 e 14 caracteres")]
        public required string CPF { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "O telefone deve ter entre 8 e 20 caracteres")]
        public required string Telefone { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "O endereço deve ter entre 1 e 500 caracteres")]
        public required string Endereco { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Os pontos de fidelidade não podem ser negativos")]
        public int PontosFidelidade { get; set; } = 0;

        public List<Ingresso> Ingressos { get; set; } = new List<Ingresso>();

        public List<PedidoAlimento> Pedidos { get; set; } = new List<PedidoAlimento>();

        public List<ProdutoAlimento> Cortesias { get; set; } = new List<ProdutoAlimento>();

        public Cliente() : base() { }

        public Cliente(int id, string nome, string email, string senha, string cpf, 
            string telefone, string endereco, DateTime dataNascimento)
            : base(id, nome, email, senha)
        {
            CPF = cpf;
            Telefone = telefone;
            Endereco = endereco;
            DataNascimento = dataNascimento;
            PontosFidelidade = 0;
            Ingressos = new List<Ingresso>();
            Pedidos = new List<PedidoAlimento>();
            Cortesias = new List<ProdutoAlimento>();
        }

        public bool EhAniversario(DateTime data)
        {
            return DataNascimento.Day == data.Day && DataNascimento.Month == data.Month;
        }

        public bool EhMesAniversario(DateTime data)
        {
            return DataNascimento.Month == data.Month;
        }

        public int ObterIdade(DateTime data)
        {
            int idade = data.Year - DataNascimento.Year;
            if (data.Month < DataNascimento.Month ||
                (data.Month == DataNascimento.Month && data.Day < DataNascimento.Day))
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
