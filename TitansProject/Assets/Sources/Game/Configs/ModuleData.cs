namespace Configs {
    public sealed class ModuleDataOld {
        public readonly string Id;
        public readonly int Cost;
        public readonly float BuildTime;

        public ModuleDataOld(string id, int cost, float buildTime) {
            Id = id;
            Cost = cost;
            BuildTime = buildTime;
        }
    }
}
