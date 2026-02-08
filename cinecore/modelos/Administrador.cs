namespace cinecore.modelos
{
    /// <summary>
    /// Modelo de dados para representar um administrador na WebAPI
    /// </summary>
    public class Administrador : Usuario
    {
        private const string EmailAdministrador = "Administrador@cinema.com";
        private const string SenhaAdministrador = "administrador123";

        public Administrador() : base() { }

        public Administrador(int id, string nome)
            : base(id, nome, EmailAdministrador, SenhaAdministrador, DateTime.Now)
        {
        }
    }
}
