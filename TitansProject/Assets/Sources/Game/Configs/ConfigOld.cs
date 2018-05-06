using Configs;
using System.Collections.Generic;

public static class ConfigOld {

    public static readonly ModuleList Modules = new ModuleList();

    public sealed class ModuleList {
        private readonly List<ModuleDataOld> modules = new List<ModuleDataOld>() {
            new ModuleDataOld("weapon", 5, 2f),
            new ModuleDataOld("rocket", 10, 2f),
            new ModuleDataOld("shield", 5, 2f),
            new ModuleDataOld("anti_air", 5, 2f),
            new ModuleDataOld("titan", 20, 15f),
            new ModuleDataOld("enemy_titan", 20, 15f),
            new ModuleDataOld("titan_upgrade", 10, 10f),
            new ModuleDataOld("add_rocket", 6, 4f)
        };

        public ModuleDataOld this[string id] {
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
