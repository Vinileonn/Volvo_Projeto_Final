using System.Text;

namespace cinema.models.UsuarioModel
{
    public class Admin : Usuario
    {
        // credenciais fixas para o admin
        private const string adminEmail = "admin@cinema.com";
        private const string adminPassword = "admin123";

        public Admin(int id, string nome)
            : base(id, nome, adminEmail, adminPassword, DateTime.Now)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("Tipo: Administrador");
            return sb.ToString();
        }
    }
}