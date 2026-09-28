using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public sealed class ArmyGenerator : IArmyGenerator
    {
        private readonly IUnitCatalog _catalog;
        private readonly IRandomSource _random;

        public ArmyGenerator(IUnitCatalog catalog, IRandomSource random)
        {
            _catalog = catalog;
            _random = random;
        }

        public UnitDefinition[] Generate(int count)
        {
            UnitDefinition[] result = new UnitDefinition[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = _catalog.GetDefinition(_random.Next(_catalog.Count));
            }

            return result;
        }
    }
}
