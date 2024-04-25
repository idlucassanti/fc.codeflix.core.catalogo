using Fc.Codeflix.Core.Catalogo.UnitTests.Common.Entities;
using Xunit;
using DomainEntity = Fc.Codeflix.Core.Catalogo.Domain.Entities;

namespace Fc.Codeflix.Core.Catalogo.UnitTests.Domain.Entities.Categoria
{
    public class CategoriaTestsFixture : BaseFixture
    {
        public CategoriaTestsFixture()
            :base() { }

        public DomainEntity.Categoria GetValidCategory()
        {
            return new DomainEntity.Categoria(GetValidName(), GetValidDescription());
        }

        public string GetValidName()
        {
            var name = string.Empty;
            
            while(name.Length < 3)
            {
                name = Faker.Commerce.Categories(1)[0];
            }

            if (name.Length > 255) 
            {
                name = name[..255];
            }

            return name;
        }

        public string GetValidDescription()
        {
            string description = Faker.Commerce.ProductDescription();

            if (description.Length > 10000)
            {
                description = description[..10000];
            }

            return description;
        }
    }

    [CollectionDefinition(nameof(CategoriaTestsFixture))]
    public class CategoriaTestsFixtureCollection : ICollectionFixture<CategoriaTestsFixture> { }
}
