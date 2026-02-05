using cinema.models;

namespace cinema.utils
{
	public static class GeradorDeLugares
	{
		public static List<Assento> GerarAssentos(int capacidade, Sala sala)
		{
			List<Assento> assentos = new List<Assento>();

			if (capacidade <= 0 || sala == null)
			{
				return assentos;
			}

			int filas = (int)Math.Ceiling(Math.Sqrt(capacidade));
			int assentosPorFila = (int)Math.Ceiling((double)capacidade / filas);
			char filaAtual = 'A';
			int assentoNumero = 1;

			for (int i = 0; i < filas; i++)
			{
				for (int j = 0; j < assentosPorFila; j++)
				{
					if (assentos.Count >= capacidade)
						break;

					Assento assento = new Assento(assentos.Count + 1, filaAtual, assentoNumero, sala, null);
					assentos.Add(assento);
					assentoNumero++;
				}
				filaAtual++;
			}

			return assentos;
		}
	}
}
