using Seguradora.Domain.Common;
using Seguradora.Domain.Sinistros;

namespace Seguradora.Domain.Apolices
{
    public sealed class Apolice
    {
        public Guid Id { get; private set; }
        public string Segurado { get; private set; }
        public decimal ValorCobertura { get; private set; }
        public DateTime VigenciaInicio { get; private set; }
        public DateTime VigenciaFim { get; private set; }
        public StatusApolice Status { get; private set; }

        private Apolice(string segurado, decimal valorCobertura, DateTime vigenciaInicio, DateTime vigenciaFim)
        {
            Id = Guid.NewGuid();
            Segurado = segurado;
            ValorCobertura = valorCobertura;
            VigenciaInicio = vigenciaInicio;
            VigenciaFim = vigenciaFim;
            Status = StatusApolice.Ativa;
        }

        public static Apolice Criar(string segurado, decimal valorCobertura, DateTime vigenciaInicio, DateTime vigenciaFim)
        {
            if (string.IsNullOrWhiteSpace(segurado))
                throw new DomainException("O nome do segurado é obrigatório.");
            if (valorCobertura <= 0)
                throw new DomainException("O valor de cobertura deve ser maior que zero.");
            if (vigenciaFim <= vigenciaInicio)
                throw new DomainException("O fim da vigência deve ser posterior ao início.");

            return new Apolice(segurado.Trim(), valorCobertura, vigenciaInicio, vigenciaFim);
        }

        public bool EstaVigente(DateTime agora) =>
            Status == StatusApolice.Ativa && agora >= VigenciaInicio && agora <= VigenciaFim;

        public void Cancelar()
        {
            if (Status == StatusApolice.Cancelada)
                throw new DomainException("A apólice já está cancelada.");

            Status = StatusApolice.Cancelada;
        }

        public Sinistro AbrirSinistro(string descricao, decimal valorEstimado, DateTime dataOcorrencia, DateTime agora)
        {
            if (!EstaVigente(agora))
                throw new DomainException("Não é possível abrir sinistro em apólice vencida ou cancelada.");
            if (dataOcorrencia < VigenciaInicio || dataOcorrencia > VigenciaFim)
                throw new DomainException("A data de ocorrência está fora da vigência da apólice.");
            if (dataOcorrencia > agora)
                throw new DomainException("A data de ocorrência não pode estar no futuro.");
            if (valorEstimado > ValorCobertura)
                throw new DomainException("O valor do sinistro excede a cobertura da apólice.");

            return Sinistro.Abrir(Id, descricao, valorEstimado, dataOcorrencia, agora);
        }
    }
}
