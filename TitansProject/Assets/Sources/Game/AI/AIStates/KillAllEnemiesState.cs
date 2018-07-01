using System;
using UnityEngine;

namespace Model.AI {
    public sealed class KillAllEnemiesState : AbstractAIState {
        private readonly Planet planet = null;
        private readonly Titan titan = null;
        private Titan enemy = null;
        private const float pursuitDistance = 2.3f;

        public KillAllEnemiesState(TitanAI ai, Planet controlPlanet) : base(ai) {
            titan = ai.Titan;
            planet = controlPlanet;
        }

        public override void Update(float deltaTime) {
            if (!titanAI.Titan.IsAlive) {
                titanAI.SetState(titanAI.DoNothing);
                return;
            }
            if (enemy == null || !enemy.IsAlive) {
                enemy = FindNearestEnemy();
            }
            MoveTo(enemy);
        }

        public override void OnStateEnter() {
            enemy = FindNearestEnemy();
        }

        private void MoveTo(Titan enemy) {
            if (enemy == null)
                return;
            var delta = (titan.Position - enemy.Position).normalized * pursuitDistance;
            var needPosition = (enemy.Position + delta).normalized * planet.Radius;
            titan.ClearTasks();
            titan.AddMoveTask(needPosition);
        }

        private Titan FindNearestEnemy() {
            Titan nearestEnemy = null;
            float nearestDistance = 0;
            foreach (var enemyFaction in titan.Faction.EnemyFactions) {
                foreach (var enemyTitan in enemyFaction.Units) {
                    if (enemyTitan == null || !enemyTitan.IsAlive)
                        continue;
                    var distance = Vector3.Distance(titan.Position, enemyTitan.Position);
                    if (nearestEnemy == null || distance < nearestDistance) {
                        nearestEnemy = enemyTitan;
                        nearestDistance = distance;
                    }
                }
            }
            return nearestEnemy;
        }
    }
}