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
    [Collection(nameof(CategoriaTestsFixture))]
    public class CategoriaTests
    {
        private readonly CategoriaTestsFixture _fixture;

        public CategoriaTests(CategoriaTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = nameof(InstanciarStatusTrue))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarStatusTrue()
        {
            //Arrange
            var categoriaDTO = _fixture.GetValidCategory();

            //Action
            DateTime dataAnterior = DateTime.Now;
            var categoria = new DomainEntity.Categoria(categoriaDTO.Nome, categoriaDTO.Descricao);
            DateTime dataPosterior = DateTime.Now.AddSeconds(1);

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
            var categoriaDTO = _fixture.GetValidCategory();

            //Actions
            DateTime dataAnterior = DateTime.Now;
            
            var categoria = new DomainEntity.Categoria(categoriaDTO.Nome, categoriaDTO.Descricao, ativo);
            
            DateTime dataPosterior = DateTime.Now.AddSeconds(1);

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
            var categoria = _fixture.GetValidCategory();
            Action action = () => new DomainEntity.Categoria(nome!, categoria.Descricao);

            action.Should().Throw<EntityValidationException>().WithMessage($"Nome não pode ser vazio ou nulo.");
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoInvalidoException()
        {
            //Arrange
            var categoria = _fixture.GetValidCategory();

            Action action = () => new DomainEntity.Categoria(categoria.Nome, null!);

            //Asserts
            action.Should().Throw<EntityValidationException>("Descricao não pode ser nulo.");
        }

        [Theory(DisplayName = nameof(InstanciarNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [MemberData(nameof(GetNamesWithLessThan3Characteres), parameters: 10)]
        public void InstanciarNomeMenor3CaracteresException(string nome)
        {
            var categoria = _fixture.GetValidCategory();

            Action action = () => new DomainEntity.Categoria(nome, categoria.Descricao);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser menor que 3 caracteres.");
        }

        [Fact(DisplayName = nameof(InstanciarNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarNomeMaior255CaracteresException()
        {
            var categoria = _fixture.GetValidCategory();
            var nomeInvalido = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Categoria(nomeInvalido, categoria.Descricao);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser maior que 255 caracteres.");
        }

        [Fact(DisplayName = nameof(InstanciarDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InstanciarDescricaoMaior10_000Exception()
        {
            var categoria = _fixture.GetValidCategory();
            var descricaoInvalida = String.Join(null, Enumerable.Range(0, 10001).Select(_ => "a").ToArray());

            Action action = () => new DomainEntity.Categoria(categoria.Nome, descricaoInvalida);

            action.Should().Throw<EntityValidationException>().WithMessage("Descricao não conter mais que 10000 caracteres.");
        }

        [Fact(DisplayName = nameof(AtivarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void AtivarCategoria()
        {
            var data = _fixture.GetValidCategory();
            var categoria = new DomainEntity.Categoria(data.Nome, data.Descricao, false);

            //Actions
            categoria.Ativar();

            //Asserts
            categoria.Ativo.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(InativarCategoria))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void InativarCategoria()
        {
            var categoria = _fixture.GetValidCategory();

            categoria.Inativar();

            categoria.Ativo.Should().BeFalse();
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void Update()
        {
            var categoria = _fixture.GetValidCategory();
            var categoriaInputDto = _fixture.GetValidCategory();

            categoria.Update(categoriaInputDto.Nome, categoriaInputDto.Descricao);

            categoria.Nome.Should().Be(categoriaInputDto.Nome);
            categoria.Descricao.Should().Be(categoriaInputDto.Descricao);
        }

        [Fact(DisplayName = nameof(UpdateNome))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNome()
        {
            var categoria = _fixture.GetValidCategory();
            var novoNome = _fixture.GetValidName();
            var descricao = categoria.Descricao;

            categoria.Update(novoNome);

            categoria.Nome.Should().Be(novoNome);
            categoria.Descricao.Should().Be(descricao);
        }

        [Theory(DisplayName = nameof(UpdateNomeInvalidoException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void UpdateNomeInvalidoException(string? nome)
        {
            var categoria = _fixture.GetValidCategory();

            Action action = () => categoria.Update(nome!);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser vazio ou nulo.");
        }

        [Theory(DisplayName = nameof(UpdateNomeMenor3CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        [MemberData(nameof(GetNamesWithLessThan3Characteres), parameters: 10)]
        public void UpdateNomeMenor3CaracteresException(string nome)
        {
            var categoria = _fixture.GetValidCategory();

            Action action = () => categoria.Update(nome);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser menor que 3 caracteres.");
        }

        public static IEnumerable<object[]> GetNamesWithLessThan3Characteres(int count)
        {
            var fixture = new CategoriaTestsFixture();

            for (int i = 0; i <= count; i++)
            {
                var isOdd = i % 2 == 1;
                yield return new object[]
                {
                    fixture.GetValidName()[..(isOdd ? 1 : 2)]
                };
            }
        }

        [Fact(DisplayName = nameof(UpdateNomeMaior255CaracteresException))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateNomeMaior255CaracteresException()
        {
            var categoria = _fixture.GetValidCategory();
            var nomeInvalido = _fixture.Faker.Lorem.Letter(256);

            Action action = () => categoria.Update(nomeInvalido);

            action.Should().Throw<EntityValidationException>().WithMessage("Nome não pode ser maior que 255 caracteres.");
        }

        [Fact(DisplayName = nameof(UpdateDescricaoMaior10_000Exception))]
        [Trait("Domain", "Categoria - Aggregates")]
        public void UpdateDescricaoMaior10_000Exception()
        {
            var categoria = _fixture.GetValidCategory();
            var descricaoInvalida = _fixture.Faker.Commerce.ProductDescription();

            while (descricaoInvalida.Length < 10000)
            {
                descricaoInvalida = $"${descricaoInvalida} {_fixture.Faker.Commerce.ProductDescription}";
            }

            Action action = () => categoria.Update("Categoria Nome Válido", descricaoInvalida);

            action.Should().Throw<EntityValidationException>().WithMessage("Descricao não conter mais que 10000 caracteres.");
        }
    }
}
