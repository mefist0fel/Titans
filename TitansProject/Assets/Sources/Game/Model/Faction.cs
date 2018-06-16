using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public sealed class Faction {
        public readonly int ID;
        public readonly List<Titan> Units = new List<Titan>();
        public readonly List<Faction> EnemyFactions = new List<Faction>();

        public interface Listener {
            void OnAddTitan(Titan titan);
            void OnRemoveTitan(Titan titan);
        }

        private Listener listener;

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

        public void AddListener(Listener factionListener) {
            listener = factionListener;
        }

        public void RemoveListener(Listener factionListener) {
            if (listener == factionListener)
                listener = null;
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

        public void RemoveUnit(Titan titan) {
            Units.Add(titan);
            if (listener != null)
                listener.OnAddTitan(titan);
        }

        public void AddUnit(Titan titan) {
            Units.Remove(titan);
            if (listener != null)
                listener.OnRemoveTitan(titan);
        }
    }
}
