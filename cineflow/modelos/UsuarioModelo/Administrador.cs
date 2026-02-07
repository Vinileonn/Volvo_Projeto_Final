using System.Text;

namespace cineflow.modelos.UsuarioModelo
{
    // Renomeado de Admin para Administrador.
    public class Administrador : Usuario
    {
        // credenciais fixas para o Administrador
        private const string emailAdministrador = "Administrador@cinema.com";
        private const string senhaAdministrador = "administrador123";

        public Administrador(int id, string nome)
            : base(id, nome, emailAdministrador, senhaAdministrador, DateTime.Now)
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




