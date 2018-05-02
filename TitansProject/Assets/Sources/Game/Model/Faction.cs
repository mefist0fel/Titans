using System.Collections.Generic;

namespace Model {
    public sealed class Faction {
        public readonly int ID;
        public readonly List<Titan> Units = new List<Titan>();
        public readonly List<Faction> EnemyFactions = new List<Faction>();

        public Faction(int id) {
            ID = id;
        }

        public void SetEnemy(Faction[] allFactions) {
            foreach (var faction in allFactions) {
                if (faction == this)
                    continue;
                EnemyFactions.Add(faction);
            }
        }
    }
}
