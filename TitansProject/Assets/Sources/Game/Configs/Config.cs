using Model;
using System.Collections.Generic;

public static class Config {

    public static readonly ModuleList Modules = new ModuleList();

    public sealed class ModuleList {
        public readonly List<ModuleData> Modules = new List<ModuleData>() {
            new ModuleData("laser", 10, 3f, "Лазерное оружие"),
            new ModuleData("thermal_protection", 10, 3f, "Термозащита"),
            new ModuleData("rocket", 10, 2f, "Ракетная установка"),
            new ModuleData("shield", 10, 2f, "Энергетические щиты"),
            new ModuleData("anti_air", 10, 2f, "Установка ПВО"),
           // new ModuleData("titan", 20, 15f),
           // new ModuleData("enemy_titan", 20, 15f),
           // new ModuleData("titan_upgrade", 10, 10f),
           // new ModuleData("add_rocket", 6, 4f)
        };

        public ModuleData this[string id] {
            get {
                foreach (var module in Modules) {
                    if (id == module.Id)
                        return module;
                }
                return null;
            }
        }
    }
}
