using cinecore.modelos;
using cinecore.enums;

namespace cinecore.utilitarios
{
    public static class GeradorDeLugares
    {
        public static List<Assento> GerarAssentos(int capacidade, Sala sala, int quantidadeCasal = 0, int quantidadePCD = 0)
        {
            List<Assento> assentos = new List<Assento>();

            if (capacidade <= 0 || sala == null)
            {
                return assentos;
            }

            int filas = (int)Math.Ceiling(Math.Sqrt(capacidade));
            int assentosPorFila = (int)Math.Ceiling((double)capacidade / filas);
            char filaAtual = 'A';
            int lugaresGerados = 0;

            for (int i = 0; i < filas; i++)
            {
                int assentoNumero = 1;
                for (int j = 0; j < assentosPorFila; j++)
                {
                    if (lugaresGerados >= capacidade)
                        break;

                    var tipo = TipoAssento.Normal;
                    int lugares = 1;

                    // Prioriza PCD no inicio, e casal quando houver espaco para 2 lugares.
                    if (quantidadePCD > 0)
                    {
                        tipo = TipoAssento.PCD;
                        quantidadePCD--;
                    }
                    else if (quantidadeCasal > 0 && (capacidade - lugaresGerados) >= 2)
                    {
                        tipo = TipoAssento.Casal;
                        lugares = 2;
                        quantidadeCasal--;
                    }

                    bool preferencial = filaAtual == 'A';
                    Assento assento = new Assento(assentos.Count + 1, filaAtual, assentoNumero, sala, null, tipo, lugares, preferencial);
                    assentos.Add(assento);
                    assentoNumero++;
                    lugaresGerados += lugares;
                }
                filaAtual++;
            }

            return assentos;
        }
    }
}
