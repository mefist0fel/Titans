using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public sealed class Faction {
        public readonly int ID;
        public readonly List<Titan> Units = new List<Titan>();
        public readonly List<Faction> EnemyFactions = new List<Faction>();
        private int number = 0;

        public interface IController {
            void Init(Faction faction);
            void OnAddTitan(Titan titan);
            void OnRemoveTitan(Titan titan);
        }

        private IController controller;

        public Faction(int id, IController factionController) {
            ID = id;
            controller = factionController;
            controller.Init(this);
        }

        public void SetEnemy(Faction[] allFactions) {
            foreach (var faction in allFactions) {
                if (faction == this)
                    continue;
                EnemyFactions.Add(faction);
            }
        }
        
        public void RemoveListener(IController factionListener) {
            if (controller == factionListener)
                controller = null;
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

        public string GetTitanName() {
            number += 1;
            switch (ID) {
                case 0:
                    return "blue_titan_" + number;
                case 1:
                    return "red_titan_" + number;
                case 2:
                    return "green_titan_" + number;
                case 3:
                    return "yellow_titan_" + number;
                default:
                    return "gray_titan_" + number;
            }
        }

        public void AddUnit(Titan titan) {
            Units.Add(titan);
            if (controller != null)
                controller.OnAddTitan(titan);
        }

        public void RemoveUnit(Titan titan) {
            Units.Remove(titan);
            if (controller != null)
                controller.OnRemoveTitan(titan);
        }
    }
}
