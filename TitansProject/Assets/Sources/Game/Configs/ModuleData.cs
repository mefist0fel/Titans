namespace Configs {
    public sealed class ModuleData {
        public readonly string Id;
        public readonly int Cost;
        public readonly float BuildTime;

        public ModuleData(string id, int cost, float buildTime) {
            Id = id;
            Cost = cost;
            BuildTime = buildTime;
        }
    }
}
