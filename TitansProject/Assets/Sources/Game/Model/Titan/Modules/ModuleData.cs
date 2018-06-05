using System.Collections.Generic;

namespace Model {
    public sealed class ModuleData {
        public readonly string Id;
        public readonly int Cost;
        public readonly float BuildTime;
        public readonly string Description;
        public readonly Dictionary<string, int> Params;

        public ModuleData(string id, int cost, float buildTime, Dictionary<string, int> parameters = null, string description = null) {
            Id = id;
            Cost = cost;
            BuildTime = buildTime;
            Description = description ?? string.Empty;
            Params = parameters ?? new Dictionary<string, int>();
        }

        public int this[string id] {
            get {
                if (Params.ContainsKey(id))
                    return Params[id];
                return 0;
            }
        }
    }
}
