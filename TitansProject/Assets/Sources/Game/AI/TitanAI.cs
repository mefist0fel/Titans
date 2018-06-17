using Model;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace Model.AI {
    public sealed class TitanAI {
        public readonly Titan Titan;

        public readonly CollectResourcesState CollectResources;
        public readonly DoNothingState DoNothing;
        public readonly KillAllEnemiesState KillAllEnemy;

        private readonly Queue<string> commandsQueue;

        private const float updateTime = 1f;
        private readonly ReloadTimer timer = new ReloadTimer(updateTime);
        private readonly Battle battle;
        private AbstractAIState currentState;
        private List<string> defaultAiCommands;

        public TitanAI(Titan titan, Battle controlBattle, List<ResourcePoint> occupiedResources, IEnumerable<string> queueOfCommands) {
            Titan = titan;
            battle = controlBattle;
            CollectResources = new CollectResourcesState(this, battle.Planet, occupiedResources);
            DoNothing = new DoNothingState(this);
            KillAllEnemy = new KillAllEnemiesState(this, battle.Planet);
            commandsQueue = new Queue<string>(queueOfCommands);
            SetState(new WaitingState(this, CollectResources, 5));
        }

        public void SetState(AbstractAIState state) {
            if (currentState != null)
                currentState.OnStateExit();
            currentState = state;
            currentState.OnStateEnter();
        }

        public void Update(float deltaTime) {
            currentState.Update(deltaTime);
            timer.Update(deltaTime);
            if (timer.IsReady) {
                timer.Reload();
                UpdateBuildList();
            }
        }

        private void UpdateBuildList() {
            if (commandsQueue.Count == 0) {
                SetState(KillAllEnemy);
                return;
            }
            var moduleName = commandsQueue.Peek();
            var module = Config.Modules[moduleName];
            if (module == null) {
                Debug.Log("Error module " + moduleName + " - to next step");
                commandsQueue.Dequeue();
                return;
            }
            if (Titan.ResourceUnits >= module.Cost) {
                TryBuildModule(module);
                commandsQueue.Dequeue();
            }
        }

        private void TryBuildModule(ModuleData module) {
            Debug.Log("Building " + module.Id.ToString());
            var slot = FindEmptySlot(Titan);
            if (slot != null) {
                slot.Build(module);
            }
        }

        private ModuleSlot FindEmptySlot(Titan titan) {
            return Titan.ModuleSlots.FirstOrDefault(slot => slot.Module == null);
        }
    }
}