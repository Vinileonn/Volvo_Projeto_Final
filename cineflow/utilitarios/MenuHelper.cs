using System;
using System.Globalization;

namespace cineflow.utilitarios
{
    public static class MenuHelper
    {
        public static void LimparConsole()
        {
            if (Console.IsOutputRedirected || Console.IsInputRedirected)
            {
                return;
            }

            Console.Clear();
        }

        public static void MostrarTitulo(string titulo)
        {
            var linha = new string('=', Math.Max(10, titulo.Length + 6));
            Console.WriteLine(linha);
            Console.WriteLine($"  {titulo}");
            Console.WriteLine(linha);
        }

        public static void MostrarCaminho(params string[] caminho)
        {
            if (caminho == null || caminho.Length == 0)
            {
                return;
            }

            Console.WriteLine(string.Join(" > ", caminho));
        }

        public static void MostrarOpcoes(params string[] opcoes)
        {
            for (int i = 0; i < opcoes.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {opcoes[i]}");
            }
            Console.WriteLine("0. Voltar/Sair");
            Console.Write("Escolha uma opcao: ");
        }

        public static int LerOpcaoInteira(int min, int max)
        {
            while (true)
            {
                var entrada = Console.ReadLine();
                if (int.TryParse(entrada, out int opcao) && opcao >= min && opcao <= max)
                {
                    return opcao;
                }
                Console.Write("Opcao invalida. Tente novamente: ");
            }
        }

        public static string LerTextoNaoVazio(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var texto = (Console.ReadLine() ?? string.Empty).Trim();
                if (!string.IsNullOrEmpty(texto))
                {
                    return texto;
                }
                Console.WriteLine("Entrada nao pode ser vazia.");
            }
        }

        public static string? LerTextoOpcional(string prompt)
        {
            Console.Write(prompt);
            var texto = (Console.ReadLine() ?? string.Empty).Trim();
            return string.IsNullOrEmpty(texto) ? null : texto;
        }

        public static int LerInteiro(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                var entrada = Console.ReadLine();
                if (int.TryParse(entrada, out int valor) && valor >= min && valor <= max)
                {
                    return valor;
                }
                Console.WriteLine("Valor invalido.");
            }
        }

        public static decimal LerDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                var entrada = (Console.ReadLine() ?? string.Empty).Trim();
                if (decimal.TryParse(entrada, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out decimal valor)
                    && valor >= min && valor <= max)
                {
                    return valor;
                }
                Console.WriteLine("Valor invalido.");
            }
        }

        public static DateTime LerData(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var entrada = (Console.ReadLine() ?? string.Empty).Trim();
                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out var data))
                {
                    return data;
                }
                Console.WriteLine("Data invalida. Use dd/MM/yyyy.");
            }
        }

        public static DateTime LerDataHora(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var entrada = (Console.ReadLine() ?? string.Empty).Trim();
                if (DateTime.TryParseExact(entrada, "dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out var dataHora))
                {
                    return dataHora;
                }
                Console.WriteLine("Data/hora invalida. Use dd/MM/yyyy HH:mm.");
            }
        }

        public static bool Confirmar(string prompt)
        {
            while (true)
            {
                Console.Write($"{prompt} (s/n): ");
                var entrada = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
                if (entrada == "s" || entrada == "sim")
                {
                    return true;
                }
                if (entrada == "n" || entrada == "nao")
                {
                    return false;
                }
                Console.WriteLine("Resposta invalida.");
            }
        }

        public static void Pausar()
        {
            Console.WriteLine("Pressione ENTER para continuar...");
            Console.ReadLine();
        }

        public static void ExibirMensagem(string mensagem)
        {
            Console.WriteLine(mensagem);
        }

    }
}
