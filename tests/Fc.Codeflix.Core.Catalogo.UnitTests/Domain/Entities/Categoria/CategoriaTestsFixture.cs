using Xunit;
using DomainEntity = Fc.Codeflix.Core.Catalogo.Domain.Entities;

namespace Fc.Codeflix.Core.Catalogo.UnitTests.Domain.Entities.Categoria
{
    public class CategoriaTestsFixture
    {
        public DomainEntity.Categoria GetValidCategory()
        {
            return new DomainEntity.Categoria("Nome válido", "Descrição Válido");
        }
    }

    [CollectionDefinition(nameof(CategoriaTestsFixture))]
    public class CategoriaTestsFixtureCollection : ICollectionFixture<CategoriaTestsFixture> { }
}
