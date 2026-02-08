using cineflow.modelos;
using cineflow.enumeracoes;

namespace cineflow.utilitarios
{
	public static class GeradorDeLugares
	{
		// ANTIGO:
		// public static List<Assento> GerarAssentos(int capacidade, Sala sala)
		// {
		// 	List<Assento> assentos = new List<Assento>();
		//
		// 	if (capacidade <= 0 || sala == null)
		// 	{
		// 		return assentos;
		// 	}
		//
		// 	int filas = (int)Math.Ceiling(Math.Sqrt(capacidade));
		// 	int assentosPorFila = (int)Math.Ceiling((double)capacidade / filas);
		// 	char filaAtual = 'A';
		// 	int assentoNumero = 1;
		//
		// 	for (int i = 0; i < filas; i++)
		// 	{
		// 		for (int j = 0; j < assentosPorFila; j++)
		// 		{
		// 			if (assentos.Count >= capacidade)
		// 				break;
		//
		// 			Assento assento = new Assento(assentos.Count + 1, filaAtual, assentoNumero, sala, null);
		// 			assentos.Add(assento);
		// 			assentoNumero++;
		// 		}
		// 		filaAtual++;
		// 	}
		//
		// 	return assentos;
		// }

		// Gera assentos especiais (PCD e Casal) mantendo a capacidade total de lugares.
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

					Assento assento = new Assento(assentos.Count + 1, filaAtual, assentoNumero, sala, null, tipo, lugares);
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





