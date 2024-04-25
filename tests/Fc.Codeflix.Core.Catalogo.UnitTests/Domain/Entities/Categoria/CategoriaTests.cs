using Fc.Codeflix.Core.Catalogo.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var dadosValidos = new
            {
                Nome = "Categoria Nome Válido",
                Descricao = "Categoria Descricao Válida"
            };

            //Action
            DateTime dataAnterior = DateTime.Now;
            var categoria = new DomainEntity.Categoria(dadosValidos.Nome, dadosValidos.Descricao);
            DateTime dataPosterior = DateTime.Now;

            //Assertions
            Assert.NotNull(categoria);
            Assert.Equal(dadosValidos.Nome, categoria.Nome);
            Assert.Equal(dadosValidos.Descricao, categoria.Descricao);
            Assert.NotEqual(default(Guid), categoria.Id);
            Assert.NotEqual(default(DateTime), categoria.DataCriacao);
            Assert.True(categoria.Ativo);
            Assert.True(categoria.DataCriacao > dataAnterior);
            Assert.True(categoria.DataCriacao < dataPosterior);
        }

        [Theory(DisplayName = nameof(InstanciarStatusFalse))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData(true)]
        [InlineData(false)]
        public void InstanciarStatusFalse(bool ativo)
        {
            //Arrange
            var dadosValidos = new
            {
                Nome = "Categoria Nome Válido",
                Descricao = "Categoria Descrição Válido"
            };

            //Actions
            DateTime dataAnterior = DateTime.Now;
            
            var categoria = new DomainEntity.Categoria(dadosValidos.Nome, dadosValidos.Descricao, ativo);
            
            DateTime dataPosterior = DateTime.Now;

            //Asserts
            Assert.NotNull(categoria);
            Assert.NotEqual(default(Guid), categoria.Id);
            Assert.NotEqual(default(DateTime), categoria.DataCriacao);
            Assert.Equal(dadosValidos.Nome, categoria.Nome);
            Assert.Equal(dadosValidos.Descricao, categoria.Descricao);
            Assert.Equal(ativo, categoria.Ativo);
            Assert.True(categoria.DataCriacao > dataAnterior);
            Assert.True(categoria.DataCriacao < dataPosterior);
        }

        [Theory(DisplayName = nameof(InstanciarNomeInvalidoError))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void InstanciarNomeInvalidoError(string? nome)
        {
            void action() => new DomainEntity.Categoria(nome!, "Categoria Descrição Válida");

            var exception = Assert.Throws<EntityValidationException>(() => action());

            Assert.Equal(exception.Message, $"Nome não pode ser vazio ou nulo.");
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoInvalidoError))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoInvalidoError()
        {
            //Arrange
            void action() => new DomainEntity.Categoria("Categoria Nome Válido", null!);

            //Actions
            var exception = Assert.Throws<EntityValidationException>(() => action());

            //Asserts
            Assert.Equal("Descricao não pode ser nulo.", exception.Message);
        }

        [Theory(DisplayName = nameof(InstanciarNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        public void InstanciarNomeMenor3CaracteresException(string nome)
        {
            void action() => new DomainEntity.Categoria(nome, "Categoria Descrição Válida");

            var exception = Assert.Throws<EntityValidationException>(() => action());

            //Asserts
            Assert.Equal("Nome não pode ser menor que 3 caracteres.", exception.Message);
        }

        [Fact(DisplayName = nameof(InstanciarNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarNomeMaior255CaracteresException()
        {
            var nome = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

            void action() => new DomainEntity.Categoria(nome, "Categoria Descrição Válida");

            var exception = Assert.Throws<EntityValidationException>(action);

            //Asserts
            Assert.Equal("Nome não pode ser maior que 255 caracteres.", exception.Message);
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoMaior10_000Exception()
        {
            var descricao = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

            void action() => new DomainEntity.Categoria("Categoria Nome Válido", descricao);

            var exception = Assert.Throws<EntityValidationException>(action);

            //Asserts
            Assert.Equal("Descricao não conter mais que 10000 caracteres.", exception.Message);
        }

        [Fact(DisplayName = nameof(AtivarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void AtivarCategoria()
        {
            var categoria = new DomainEntity.Categoria("Categoria Nome Válido", "Categoria Descrição Válida", false);

            //Actions
            categoria.Ativar();

            //Asserts
            Assert.True(categoria.Ativo);
        }

        [Fact(DisplayName = nameof(InativarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InativarCategoria()
        {
            var categoria = new DomainEntity.Categoria("Categoria Nome Válido", "Categoria Descrição Válida");

            categoria.Inativar();

            Assert.False(categoria.Ativo);
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void Update()
        {
            var categoria = new DomainEntity.Categoria("Nome 1", "Descrição 1");
            var categoriaDto = new { Nome = "Nome novo", Descricao = "Descrição nova" };

            categoria.Update(categoriaDto.Nome, categoriaDto.Descricao);

            Assert.Equal(categoriaDto.Nome, categoria.Nome);
            Assert.Equal(categoriaDto.Descricao, categoria.Descricao);
        }

        [Fact(DisplayName = nameof(UpdateNome))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNome()
        {
            var categoria = new DomainEntity.Categoria("Nome 1", "Descrição 1");
            var categoriaDto = new { Nome = "Nome novo" };
            var descricao = categoria.Descricao;

            categoria.Update(categoriaDto.Nome);

            Assert.Equal(categoriaDto.Nome, categoria.Nome);
            Assert.Equal(descricao, categoria.Descricao);
        }

        [Theory(DisplayName = nameof(UpdateNomeInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void UpdateNomeInvalidoException(string? nome)
        {
            var categoria = new DomainEntity.Categoria("Nome Válido", "Descrição Válido");

            void action() => categoria.Update(nome!);

            var exception = Assert.Throws<EntityValidationException>(() => action());

            Assert.Equal(exception.Message, $"Nome não pode ser vazio ou nulo.");
        }

        [Theory(DisplayName = nameof(UpdateNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("abc")]
        public void UpdateNomeMenor3CaracteresException(string nome)
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            
            void action() => categoria.Update(nome);

            var exception = Assert.Throws<EntityValidationException>(() => action());

            //Asserts
            Assert.Equal("Nome não pode ser menor que 3 caracteres.", exception.Message);
        }

        [Fact(DisplayName = nameof(UpdateNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNomeMaior255CaracteresException()
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            var nomeInvalido = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

            void action() => categoria.Update(nomeInvalido);

            var exception = Assert.Throws<EntityValidationException>(action);

            //Asserts
            Assert.Equal("Nome não pode ser maior que 255 caracteres.", exception.Message);
        }

        [Fact(DisplayName = nameof(UpdateDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateDescricaoMaior10_000Exception()
        {
            var categoria = new DomainEntity.Categoria("Nome válido", "Descrição válido");
            var descricaoInvalida = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

            void action() => categoria.Update("Categoria Nome Válido", descricaoInvalida);

            var exception = Assert.Throws<EntityValidationException>(action);

            //Asserts
            Assert.Equal("Descricao não conter mais que 10000 caracteres.", exception.Message);
        }
    }
}
