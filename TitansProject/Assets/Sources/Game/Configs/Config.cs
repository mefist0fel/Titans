using Configs;
using System.Collections.Generic;

public static class Config {

    public static readonly ModuleList Modules = new ModuleList();

    public sealed class ModuleList {
        private readonly List<ModuleData> modules = new List<ModuleData>() {
            new ModuleData("weapon", 1, 2f),
            new ModuleData("rocket", 2, 2f),
            new ModuleData("shield", 3, 2f),
            new ModuleData("anti_air", 4, 2f),
            new ModuleData("titan", 10, 2f),
            new ModuleData("level_up", 10, 2f)
        };

        public ModuleData this[string id] {
            get {
                foreach (var module in modules) {
                    if (id == module.Id)
                        return module;
                }
                return null;
            }
        }
    }
}
