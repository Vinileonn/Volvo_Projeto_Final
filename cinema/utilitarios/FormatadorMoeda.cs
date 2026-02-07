using System.Globalization;

namespace cinema.utilitarios
{
    public static class FormatadorMoeda
    {
        public static string Formatar(float valor)
        {
            return ((decimal)valor).ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        public static string Formatar(decimal valor)
        {
            return valor.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
        }
    }
}





