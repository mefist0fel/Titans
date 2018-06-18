using System;
using System.Collections.Generic;
using UnityEngine;


namespace Model {
    /// <summary>
    /// Main class for control battle (units, resources) on single planet
    /// </summary>
    public sealed class Battle {
        public readonly Planet Planet;
        public readonly Faction[] Factions;
        public List<Titan> Units { get; private set; }
        private readonly List<AbstractInteraction> activeInteractions = new List<AbstractInteraction>();
        public readonly List<AbstractInteraction> interactionsAddQueue = new List<AbstractInteraction>();
        private readonly IBattleController controller;

        public interface IFactionController : Faction.IController {
            void Init(Battle battle);
        }

        public Battle(IBattleController battleController, IFactionController[] factionControllers) {
            controller = battleController;
            Planet = new Planet();
            Units = new List<Titan>();
            Factions = new Faction[factionControllers.Length];
            for (int i = 0; i < factionControllers.Length; i++) {
                factionControllers[i].Init(this);
                Factions[i] = new Faction(i, factionControllers[i]);
            }
            foreach (var faction in Factions)
                faction.SetEnemy(Factions);
            foreach (var faction in Factions)
                AddTitan(faction, Quaternion.Euler(faction.ID * 90, 0, 0) * new Vector3(0, 0, Planet.Radius)); // Planet.GetRandomPosition());

            // AddTitan(0, new Vector3(0, 0, Planet.Radius));
            // AddTitan(0, Quaternion.Euler(10, 0, 0) * new Vector3(0, 0, Planet.Radius));
            // AddTitan(1, Quaternion.Euler(0, 10, 0) * new Vector3(0, 0, Planet.Radius));// Planet.GetRandomPosition());
        }

        private void AddTitan(Faction faction, Vector3 position) {
            var titan = new Titan(faction, this, position);
            Units.Add(titan);
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
            titan.Faction.RemoveUnit(titan);
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