using Bogus;

namespace Fc.Codeflix.Core.Catalogo.UnitTests.Common.Entities
{
    public abstract class BaseFixture
    {
        public BaseFixture() => Faker = new Faker("pt_BR");

        public Faker Faker { get; set; }
    }
}
