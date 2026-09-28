using System;
using System.Collections.Generic;
using SimpleArmyClash.Configuration;
using SimpleArmyClash.Domain;

namespace SimpleArmyClash.Application
{
    public sealed class UnitCatalog : IUnitCatalog
    {
        private readonly UnitDefinition[] _definitions;

        public int Count => _definitions.Length;
        public float MaximumRadius { get; }

        public UnitCatalog(UnitCatalogConfiguration configuration, IUnitStatisticsCalculator calculator)
        {
            ValidateIdentifiers(configuration);
            List<UnitDefinition> definitions = new List<UnitDefinition>();
            float maximumRadius = 0f;

            foreach (UnitShapeConfiguration shape in configuration.Shapes)
            {
                foreach (UnitColorConfiguration color in configuration.Colors)
                {
                    foreach (UnitSizeConfiguration size in configuration.Sizes)
                    {
                        UnitStatistics statistics = calculator.Calculate(configuration.BaseStatistics,
                            shape.StatisticsModifier, color.StatisticsModifier, size.StatisticsModifier);

                        if (statistics.MaximumHealth <= 0)
                        {
                            continue;
                        }

                        ValidateStatistics(statistics);
                        float radius = shape.Radius * size.Scale;

                        if (!IsPositiveFinite(radius) || !IsPositiveFinite(size.Scale))
                        {
                            throw new ArgumentException("Unit radius and scale must be positive finite numbers.");
                        }

                        UnitDefinition definition = new UnitDefinition(shape.Identifier, color.Identifier,
                            size.Identifier, statistics, size.Scale, radius);
                        definitions.Add(definition);
                        maximumRadius = Math.Max(maximumRadius, radius);
                    }
                }
            }

            if (definitions.Count == 0)
            {
                throw new ArgumentException("The unit catalog must contain at least one combination with positive health.");
            }

            _definitions = definitions.ToArray();
            MaximumRadius = maximumRadius;
        }

        public UnitDefinition GetDefinition(int index)
        {
            return _definitions[index];
        }

        private static void ValidateIdentifiers(UnitCatalogConfiguration configuration)
        {
            HashSet<string> identifiers = new HashSet<string>(StringComparer.Ordinal);

            foreach (UnitShapeConfiguration shape in configuration.Shapes)
            {
                ValidateIdentifier(shape.Identifier, identifiers);
            }

            identifiers.Clear();

            foreach (UnitColorConfiguration color in configuration.Colors)
            {
                ValidateIdentifier(color.Identifier, identifiers);
            }

            identifiers.Clear();

            foreach (UnitSizeConfiguration size in configuration.Sizes)
            {
                ValidateIdentifier(size.Identifier, identifiers);
            }
        }

        private static void ValidateIdentifier(string identifier, HashSet<string> identifiers)
        {
            if (string.IsNullOrWhiteSpace(identifier) || !identifiers.Add(identifier))
            {
                throw new ArgumentException("Catalog identifiers must be nonempty and unique within their category.");
            }
        }

        private static void ValidateStatistics(UnitStatistics statistics)
        {
            if (statistics.AttackDamage <= 0 || !IsPositiveFinite(statistics.MovementSpeed)
                || !IsPositiveFinite(statistics.AttackInterval))
            {
                throw new ArgumentException("Living units must have positive damage, movement speed and attack interval.");
            }
        }

        private static bool IsPositiveFinite(float value)
        {
            return value > 0f && !float.IsInfinity(value) && !float.IsNaN(value);
        }
    }
}
