using Model;
using System.Collections.Generic;

public static class Config {

    public static readonly ModuleList Modules = new ModuleList();
    public static readonly ModuleData Base = new ModuleData("base", 0, 0,
                new Dictionary<string, int>() {
                    { "armor", 10},
                    { "armor_restore", 1},
                    { "accuracy", 100},
                    { "cloaking", 100}
                });

    public sealed class ModuleList {
        public readonly List<ModuleData> Modules = new List<ModuleData>() {
            new ModuleData("laser", 10, 3f,
                new Dictionary<string, int>() {
                    { "armor", 5},
                    { "damage", 2}
                },
                "Лазерное оружие"),
            new ModuleData("shield", 10, 2f,
                new Dictionary<string, int>() {
                    { "shield", 10},
                    { "shield_restore", 3}
                },
                "Энергетические щиты"),
            new ModuleData("rocket", 10, 2f,
                new Dictionary<string, int>() {
                    { "armor", 5},
                    { "rocket_salvo", 3}
                },
                "Ракетная установка"),
            new ModuleData("anti_air", 10, 2f,
                new Dictionary<string, int>() {
                    { "armor", 5},
                    { "anti_air", 6}
                },
                "Установка ПВО"),
            new ModuleData("radar", 10, 3f,
                new Dictionary<string, int>() {
                    { "armor", 5},
                    { "accuracy", 50}
                },
                "Радар - увеличивает точность прицеливания обычным оружием"),
            new ModuleData("cloak", 10, 3f,
                new Dictionary<string, int>() {
                    { "armor", 5},
                    { "cloak", 50}
                },
                "Система маскировки - снижает точность прицеливания противника"),
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
