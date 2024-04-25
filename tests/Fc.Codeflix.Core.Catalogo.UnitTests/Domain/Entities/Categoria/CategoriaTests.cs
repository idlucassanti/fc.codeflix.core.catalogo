using Fc.Codeflix.Core.Catalogo.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using DomainEntity = Fc.Codeflix.Core.Catalogo.Domain.Entities;

namespace Fc.Codeflix.Core.Catalogo.UnitTests.Domain.Entities.Categoria
{
    public class CategoriaTests
    {
        [Fact(DisplayName = nameof(InstanciarStatusTrue))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarStatusTrue()
        {
            //Arrange
            var categoriaDTO = new
            {
                Nome = "Categoria Nome Válido",
                Descricao = "Categoria Descricao Válida"
            };

            //Action
            DateTime dataAnterior = DateTime.Now;
            var categoria = new DomainEntity.Categoria(categoriaDTO.Nome, categoriaDTO.Descricao);
            DateTime dataPosterior = DateTime.Now;

            //Assertions
            categoria.Should().NotBeNull();
            categoria.Nome.Should().Be(categoriaDTO.Nome);
            categoria.Descricao.Should().Be(categoriaDTO.Descricao);
            categoria.Id.Should().NotBe(default(Guid));
            categoria.DataCriacao.Should().NotBeSameDateAs(default(DateTime));
            categoria.Ativo.Should().BeTrue();
            (categoria.DataCriacao > dataAnterior).Should().BeTrue();
            (categoria.DataCriacao < dataPosterior).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstanciarStatusFalse))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstanciarStatusFalse(bool ativo)
        {
            //Arrange
            var categoriaDTO = new
            {
                Nome = "Categoria Nome Válido",
                Descricao = "Categoria Descrição Válido"
            };

            //Actions
            DateTime dataAnterior = DateTime.Now;
            
            var categoria = new DomainEntity.Categoria(categoriaDTO.Nome, categoriaDTO.Descricao, ativo);
            
            DateTime dataPosterior = DateTime.Now;

            //Asserts
            categoria.Should().NotBeNull();
            categoria.Should().NotBe(default(Guid));
            categoria.Should().NotBeSameAs(default(DateTime));
            categoria.Nome.Should().Be(categoriaDTO.Nome);
            categoria.Descricao.Should().Be(categoriaDTO.Descricao);
            categoria.Ativo.Should().Be(ativo);
            (categoria.DataCriacao > dataAnterior).Should().BeTrue();
            (categoria.DataCriacao < dataPosterior).Should().BeTrue();
        }

        [Theory(DisplayName = nameof(InstanciarNomeInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void InstanciarNomeInvalidoException(string? nome)
        {
            Action action = () => new DomainEntity.Categoria(nome!, "Categoria Descrição Válida");

            action.Should().Throw<EntityValidationException>().WithMessage($"Nome não pode ser vazio ou nulo.");
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoInvalidoException()
        {
            //Arrange
            Action action = () => new DomainEntity.Categoria("Categoria Nome Válido", null!);

            //Asserts
            action.Should().Throw<EntityValidationException>("Descricao não pode ser nulo.");
        }

        [Theory(DisplayName = nameof(InstanciarNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        public void InstanciarNomeMenor3CaracteresException(string nome)
        {
            Action action = () => new DomainEntity.Categoria(nome, "Categoria Descrição Válida");

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser menor que 3 caracteres.");
        }

        [Fact(DisplayName = nameof(InstanciarNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarNomeMaior255CaracteresException()
        {
            var nome = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Categoria(nome, "Categoria Descrição Válida");

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser maior que 255 caracteres.");
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoMaior10_000Exception()
        {
            var descricao = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Categoria("Categoria Nome Válido", descricao);

            action.Should().Throw<EntityValidationException>().WithMessage("Descricao não conter mais que 10000 caracteres.");
        }

        [Fact(DisplayName = nameof(AtivarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void AtivarCategoria()
        {
            var categoria = new DomainEntity.Categoria("Categoria Nome Válido", "Categoria Descrição Válida", false);

            //Actions
            categoria.Ativar();

            //Asserts
            categoria.Ativo.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(InativarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InativarCategoria()
        {
            var categoria = new DomainEntity.Categoria("Categoria Nome Válido", "Categoria Descrição Válida");

            categoria.Inativar();

            categoria.Ativo.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void Update()
        {
            var categoria = new DomainEntity.Categoria("Nome 1", "Descrição 1");
            var categoriaDto = new { Nome = "Nome novo", Descricao = "Descrição nova" };

            categoria.Update(categoriaDto.Nome, categoriaDto.Descricao);

            categoria.Nome.Should().Be(categoriaDto.Nome);
            categoria.Descricao.Should().Be(categoriaDto.Descricao);
        }

        [Fact(DisplayName = nameof(UpdateNome))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNome()
        {
            var categoria = new DomainEntity.Categoria("Nome 1", "Descrição 1");
            var categoriaDto = new { Nome = "Nome novo" };
            var descricao = categoria.Descricao;

            categoria.Update(categoriaDto.Nome);

            categoria.Nome.Should().Be(categoriaDto.Nome);
            categoria.Descricao.Should().Be(descricao);
        }

        [Theory(DisplayName = nameof(UpdateNomeInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void UpdateNomeInvalidoException(string? nome)
        {
            var categoria = new DomainEntity.Categoria("Nome Válido", "Descrição Válido");

            Action action = () => categoria.Update(nome!);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser vazio ou nulo.");
        }

        [Theory(DisplayName = nameof(UpdateNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        public void UpdateNomeMenor3CaracteresException(string nome)
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            
            Action action = () => categoria.Update(nome);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser menor que 3 caracteres.");
        }

        [Fact(DisplayName = nameof(UpdateNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNomeMaior255CaracteresException()
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            var nomeInvalido = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

            Action action = () => categoria.Update(nomeInvalido);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser maior que 255 caracteres.");
        }

        [Fact(DisplayName = nameof(UpdateDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateDescricaoMaior10_000Exception()
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            var descricaoInvalida = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

            Action action = () => categoria.Update("Categoria Nome Válido", descricaoInvalida);

            action.Should().Throw<EntityValidationException>().WithMessage("Descricao não conter mais que 10000 caracteres.");
        }
    }
}
