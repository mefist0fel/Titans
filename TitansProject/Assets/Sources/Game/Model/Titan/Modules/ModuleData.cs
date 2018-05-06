namespace Model {
    public sealed class ModuleData {
        public readonly string Id;
        public readonly int Cost;
        public readonly float BuildTime;
        public readonly string Description;

        public ModuleData(string id, int cost, float buildTime, string description = null) {
            Id = id;
            Cost = cost;
            BuildTime = buildTime;
            Description = description ?? string.Empty;
        }
    }
}
