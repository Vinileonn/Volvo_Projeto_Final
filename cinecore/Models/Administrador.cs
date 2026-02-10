namespace cinecore.Models
{
    /// <summary>
    /// Modelo de dados para representar um administrador na WebAPI
    /// </summary>
    public class Administrador : Usuario
    {
        public Administrador() : base() { }

        public Administrador(int id, string nome, string email, string senha)
            : base(id, nome, email, senha)
        {
        }

        /// <summary>
        /// Factory method para criar administrador com credenciais personalizadas
        /// </summary>
        public static Administrador Criar(int id, string nome, string email, string senha)
        {
            return new Administrador 
            { 
                Id = id, 
                Nome = nome,
                Email = email,
                Senha = senha,
                DataCriacao = DateTime.Now
            };
        }
    }
}
