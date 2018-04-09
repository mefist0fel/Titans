using Configs;
using System.Collections.Generic;

public static class Config {

    public static readonly ModuleList Modules = new ModuleList();

    public sealed class ModuleList {
        private readonly List<ModuleData> modules = new List<ModuleData>() {
            new ModuleData("weapon", 5, 2f),
            new ModuleData("rocket", 10, 2f),
            new ModuleData("shield", 5, 2f),
            new ModuleData("anti_air", 5, 2f),
            new ModuleData("titan", 20, 15f),
            new ModuleData("enemy_titan", 20, 15f),
            new ModuleData("titan_upgrade", 10, 10f),
            new ModuleData("add_rocket", 6, 4f)
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
