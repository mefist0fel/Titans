﻿using System;
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
        private readonly IBattleController controller;

        public Battle(IBattleController battleController) {
            controller = battleController;
            Planet = new Planet();
            Units = new List<Titan>();
            Factions = new Faction[] {
                new Faction(0),
                new Faction(1)
            };
            foreach (var faction in Factions)
                faction.SetEnemy(Factions);

            AddTitan(0, new Vector3(0, Planet.Radius, 0));
            AddTitan(1, Planet.GetRandomPosition());
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

        private void RemoveTitan(Titan titan) {
            controller.OnRemoveTitan(titan);

            Units.Remove(titan);
            titan.Faction.Units.Remove(titan);
        }

        public void Update(float deltaTime) {
            foreach (var titan in Units) {
                if (titan.IsAlive)
                    titan.Update(deltaTime);
            }
            RemoveDeadTitans();
        }

        private void RemoveDeadTitans() {
            foreach (var titan in Units) {
                if (!titan.IsAlive) {
                    RemoveTitan(titan);
                    return;
                }
            }
        }
    }

    public interface IBattleController {
        void OnCreateTitan(Titan titan);
        void OnRemoveTitan(Titan titan);
        void OnBattleEnd(Faction winner);
    }
}