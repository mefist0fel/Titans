using System;
using System.Collections.Generic;
using UnityEngine;


namespace Model {
    /// <summary>
    /// Main class for control battle (units, resources) on single planet
    /// </summary>
    public sealed class Battle {
        public Planet Planet { get; private set; }

        public readonly Faction[] Factions;
        public List<Titan> Units { get; private set; }
        private List<AbstractInteraction> activeInteractions;
        public List<AbstractInteraction> interactionsAddQueue;
        private readonly IBattleController controller;

        public Battle(IBattleController battleController) {
            controller = battleController;
            Planet = new Planet();
            Units = new List<Titan>();
            activeInteractions = new List<AbstractInteraction>();
            interactionsAddQueue = new List<AbstractInteraction>();
            Factions = new Faction[] {
                new Faction(0),
                new Faction(1)
            };
            foreach (var faction in Factions)
                faction.SetEnemy(Factions);

            AddTitan(0, new Vector3(0, 0, Planet.Radius));
            AddTitan(0, Quaternion.Euler(10, 0, 0) * new Vector3(0, 0, Planet.Radius));
            AddTitan(1, Quaternion.Euler(0, 10, 0) * new Vector3(0, 0, Planet.Radius));// Planet.GetRandomPosition());
            CameraController.SetViewToTitan(new Vector3(0, 0, Planet.Radius));
        }

        private void AddTitan(int factionId, Vector3 position) {
            if (factionId < 0 || factionId >= Factions.Length) {
                throw new InvalidOperationException("Try add titan but no such faction id " + factionId);
            }
            var faction = Factions[factionId];
            var titan = new Titan(faction, this, position);
            Units.Add(titan);
            faction.Units.Add(titan);
            controller.OnCreateTitan(titan);
        }

        public void AddInteraction(AbstractInteraction interaction) {
            interactionsAddQueue.Add(interaction);
            controller.OnAddInteraction(interaction);
        }

        public void Update(float deltaTime) {
            foreach (var titan in Units) {
                if (titan.IsAlive)
                    titan.Update(deltaTime);
            }
            foreach (var interaction in interactionsAddQueue) {
                activeInteractions.Add(interaction);
            }
            interactionsAddQueue.Clear();
            foreach (var interaction in activeInteractions) {
                interaction.Update(deltaTime);
            }
            RemoveDeadTitans();
            RemoveEndedInteractions();
        }

        private void RemoveDeadTitans() {
            foreach (var titan in Units) {
                if (!titan.IsAlive) {
                    RemoveTitan(titan);
                    return;
                }
            }
        }

        private void RemoveTitan(Titan titan) {
            controller.OnRemoveTitan(titan);

            Units.Remove(titan);
            titan.Faction.Units.Remove(titan);
        }

        private void RemoveEndedInteractions() {
            for (int i = 0; i < activeInteractions.Count; i++) {
                if (activeInteractions[i].IsEnded) {
                    activeInteractions.RemoveAt(i);
                    i -= 1;
                }
            }
        }
    }

    public interface IBattleController {
        void OnCreateTitan(Titan titan);
        void OnRemoveTitan(Titan titan);
        void OnBattleEnd(Faction winner);
        void OnAddInteraction(AbstractInteraction interaction);
    }
}