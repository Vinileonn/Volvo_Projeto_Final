using System;
using System.Collections.Generic;

namespace cineflow.utilitarios
{
    public static class CalculadoraTroco
    {
        // Calcula troco detalhado em notas e moedas.
        public static Dictionary<decimal, int> CalcularTroco(decimal total, decimal pago, out decimal valorTroco)
        {
            if (pago < total)
            {
                throw new ArgumentException("Valor pago menor que o total.");
            }

            valorTroco = Math.Round(pago - total, 2);
            var trocoDetalhado = new Dictionary<decimal, int>();

            var denominacoes = new decimal[]
            {
                100m, 50m, 20m, 10m, 5m, 2m, 1m,
                0.50m, 0.25m, 0.10m, 0.05m, 0.01m
            };

            var restante = valorTroco;
            foreach (var d in denominacoes)
            {
                if (restante <= 0)
                {
                    break;
                }

                int quantidade = (int)(restante / d);
                if (quantidade > 0)
                {
                    trocoDetalhado[d] = quantidade;
                    restante = Math.Round(restante - (quantidade * d), 2);
                }
            }

            return trocoDetalhado;
        }
    }
}





