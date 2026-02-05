using cinema.models;
using cinema.models.IngressoModel;
using cinema.models.UsuarioModel;
using cinema.exceptions;

namespace cinema.services
{
    public class IngressoService
    {
        private readonly List<Ingresso> ingressos;

        public IngressoService()
        {
            ingressos = new List<Ingresso>();
        }

        // VENDA - inteira
        public Ingresso VenderInteira(Sessao sessao, Cliente cliente, char fila, int numero)
        {
            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var ingresso = new IngressoInteira(sessao.Preco, ProximoId(), fila, numero, sessao, cliente, assento, DateTime.Now);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            return ingresso;
        }

        // VENDA - meia
        public Ingresso VenderMeia(Sessao sessao, Cliente cliente, char fila, int numero, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new DadosInvalidosException("O motivo para meia entrada é obrigatório.");
            }

            var assento = ValidarVenda(sessao, cliente, fila, numero);

            var ingresso = new IngressoMeia(sessao.Preco, ProximoId(), fila, numero, sessao, cliente, assento, DateTime.Now, motivo);
            RegistrarVenda(ingresso, assento, sessao, cliente);
            return ingresso;
        }

        // READ - obter ingresso por id
        public Ingresso ObterIngresso(int id)
        {
            var ingresso = ingressos.FirstOrDefault(i => i.Id == id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoException($"Ingresso com ID {id} não encontrado.");
            }
            return ingresso;
        }

        // READ - listar todos os ingressos
        public List<Ingresso> ListarIngressos()
        {
            return new List<Ingresso>(ingressos);
        }

        // DELETE - cancelar ingresso e liberar assento
        public void CancelarIngresso(int id)
        {
            var ingresso = ObterIngresso(id);
            if (ingresso == null)
            {
                throw new RecursoNaoEncontradoException($"Ingresso com ID {id} não encontrado.");
            }

            ingresso.Assento.Liberar();
            ingresso.Assento.Ingresso = null;
            ingresso.Sessao.Ingressos.Remove(ingresso);
            ingresso.Cliente.Ingressos.Remove(ingresso);

            ingressos.Remove(ingresso);
        }

        private Assento ValidarVenda(Sessao sessao, Cliente cliente, char fila, int numero)
        {
            if (sessao == null)
            {
                throw new DadosInvalidosException("Sessão nula.");
            }
            // Validações básicas agrupadas
            var erros = new List<string>();
            
            if (sessao == null)
                erros.Add("sessão nula");
            
            if (cliente == null)
                erros.Add("cliente nulo");
            
            if (erros.Count > 0)
            {
                throw new DadosInvalidosException($"Dados inválidos: {string.Join(", ", erros)}.");
            }

            // Validações críticas da sessão
            if (sessao?.Sala == null)
            {
                throw new ErroOperacaoCriticaException("Sessão sem sala associada.");
            }

            if (sessao.Preco < 0)
            {
                throw new ErroOperacaoCriticaException("Preço inválido.");
            }

            // Validações de localização do assento (fila e número)
            var errosAssento = new List<string>();
            if (!char.IsLetter(fila))
                errosAssento.Add("fila inválida");
            
            if (numero <= 0)
                errosAssento.Add("número inválido");
            
            if (errosAssento.Count > 0)
            {
                throw new DadosInvalidosException($"Localização do assento inválida: {string.Join(", ", errosAssento)}.");
            }

            var assentoSala = sessao.Sala.ConsultarAssento(fila, numero);
            if (assentoSala == null)
            {
                throw new RecursoNaoEncontradoException($"Assento {fila}{numero} não existe.");
            }

            if (!assentoSala.Disponivel)
            {
                throw new OperacaoNaoPermitidaException($"Assento {fila}{numero} indisponível.");
            }

            return assentoSala;
        }

        private void RegistrarVenda(Ingresso ingresso, Assento assento, Sessao sessao, Cliente cliente)
        {
            assento.Reservar();
            assento.Ingresso = ingresso;

            if (!sessao.Ingressos.Contains(ingresso))
            {
                sessao.Ingressos.Add(ingresso);
            }

            if (!cliente.Ingressos.Contains(ingresso))
            {
                cliente.Ingressos.Add(ingresso);
            }

            ingressos.Add(ingresso);
        }

        private int ProximoId()
        {
            return ingressos.Count > 0 ? ingressos.Max(i => i.Id) + 1 : 1;
        }
    }
}