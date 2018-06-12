using System;
using System.Collections.Generic;
using UnityEngine;

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

        public Titan FindNearestEnemy(Vector3 position, float radius) {
            Titan nearestEnemy = null;
            float nearestDistance = 0;
            foreach (var enemyFaction in EnemyFactions) {
                foreach (var enemyTitan in enemyFaction.Units) {
                    if (enemyTitan == null || !enemyTitan.IsAlive)
                        continue;
                    var distance = Vector3.Distance(enemyTitan.Position, position);
                    if (distance < radius && (distance < nearestDistance || nearestEnemy == null)) {
                        nearestDistance = distance;
                        nearestEnemy = enemyTitan;
                    }
                }
            }
            return nearestEnemy;
        }
    }
}
