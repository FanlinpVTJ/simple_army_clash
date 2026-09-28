namespace SimpleArmyClash.Domain
{
    public sealed class UnitDefinition
    {
        public string ShapeIdentifier { get; }
        public string ColorIdentifier { get; }
        public string SizeIdentifier { get; }
        public UnitStatistics Statistics { get; }
        public float Scale { get; }
        public float Radius { get; }

        public UnitDefinition(string shapeIdentifier, string colorIdentifier, string sizeIdentifier,
            UnitStatistics statistics, float scale, float radius)
        {
            ShapeIdentifier = shapeIdentifier;
            ColorIdentifier = colorIdentifier;
            SizeIdentifier = sizeIdentifier;
            Statistics = statistics;
            Scale = scale;
            Radius = radius;
        }
    }
}
