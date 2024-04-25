using Fc.Codeflix.Core.Catalogo.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fc.Codeflix.Core.Catalogo.Domain.Entities
{
    public class Categoria
    {
        public Categoria(string nome, string descricao, bool ativo = true)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao;
            Ativo = ativo;
            DataCriacao = DateTime.Now;

            Validate();
        }

        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCriacao { get; private set; }

        public void Ativar()
        {
            this.Ativo = true;
            Validate();
        }

        public void Inativar()
        {
            this.Ativo = false;
            Validate();
        }

        public void Update(string nome, string? descricao = null)
        {
            this.Nome = nome;
            this.Descricao = descricao ?? Descricao;
            Validate();
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Nome))
                throw new EntityValidationException($"{nameof(this.Nome)} não pode ser vazio ou nulo.");

            if (this.Nome.Length <= 3)
                throw new EntityValidationException($"{nameof(this.Nome)} não pode ser menor que 3 caracteres.");

            if (this.Nome.Length > 255)
                throw new EntityValidationException($"{nameof(this.Nome)} não pode ser maior que 255 caracteres.");

            if (Descricao == null)
                throw new EntityValidationException($"{nameof(this.Descricao)} não pode ser nulo.");

            if (this.Descricao.Length > 10000)
                throw new EntityValidationException($"{nameof(this.Descricao)} não conter mais que 10000 caracteres.");
        }
    }
}
