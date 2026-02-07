namespace cinema.utilitarios
{
    public static class FormatadorData
    {
        public static string FormatarData(System.DateTime dataHora)
        {
            return dataHora.ToString("dd/MM/yyyy");
        }

        public static string FormatarDataComHora(System.DateTime dataHora)
        {
            return dataHora.ToString("dd/MM/yyyy HH:mm");
        }

        public static string FormatarHora(System.DateTime dataHora)
        {
            return dataHora.ToString("HH:mm");
        }
    }
}





