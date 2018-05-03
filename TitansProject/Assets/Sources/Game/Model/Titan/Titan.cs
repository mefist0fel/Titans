using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [System.Serializable]
    public sealed class Titan {

        public readonly Faction Faction;

        public Vector3 Position { get { return mover.Position; } }
        public Quaternion UpRotation { get { return mover.UpRotation; } }

        public int MaxArmor { get; private set; }
        public int Armor { get; private set; }

        public int ResourceUnits { get; private set; }
        public float Speed { get; private set; }

        public readonly ModuleSlot[] ModuleSlots;

        private readonly Battle battle;

        private TitanMover mover;
        private IView view = new NoView();

        private List<Task> taskList = new List<Task>(20);
        public List<Task> TaskList { get { return taskList; } }

        public bool IsAlive { get { return Armor > 0; } }

        public abstract class Task {
            public abstract bool IsEnded { get; }
            public abstract void MakeTask(float deltaTime);
        }

        public Titan(Faction faction, Battle battleContext, Vector3 position) {
            Faction = faction;
            battle = battleContext;
            Speed = 2;
            Armor = 10;
            mover = new TitanMover(battleContext.Planet, position, Speed);
            ModuleSlots = new ModuleSlot[12];
            for (int i = 0; i < ModuleSlots.Length; i++) {
                ModuleSlots[i] = new ModuleSlot(this);
            }
        }

        public void Hit(int damage) {
            Debug.Log("I'm hit! on " + damage);
        }

        public void Die(int damage) {
        }

        public void SetView(IView titanView) {
            if (view == null)
                throw new NullReferenceException("Try set titan view but view is null");
            view = titanView;
        }

        public void Update(float deltaTime) {
            var currentTask = GetCurrentTask();
            if (currentTask != null) {
                currentTask.MakeTask(deltaTime);
                if (currentTask.IsEnded) {
                    taskList.RemoveAt(0);
                    view.OnUpdateTaskList();
                }
            }
            for (int i = 0; i < ModuleSlots.Length; i++) {
                ModuleSlots[i].Update(deltaTime);
            }
        }

        private Task GetCurrentTask() {
            if (taskList.Count > 0)
                return taskList[0];
            return null;
        }

        public void ClearTasks() {
            taskList.Clear();
        }

        public void TryRemoveLastMoveTask() {
            var count = taskList.Count;
            if (count <= 0)
                return;
            if (taskList[count - 1] is MoveTask) {
                taskList.RemoveAt(count - 1);
            }
        }

        public void AddResourceTask(ResourcePoint resourcePoint) {
            taskList.Add(new MoveTask(resourcePoint.Position, mover));
            taskList.Add(new ResourceTask(resourcePoint, OnCollectResource));
            view.OnUpdateTaskList();
        }

        public void AddMoveTask(Vector3 targetPosition) {
            taskList.Add(new MoveTask(targetPosition, mover));
            view.OnUpdateTaskList();
        }

        private void OnCollectResource(int count) {
            ResourceUnits += count;
            view.OnCollectResource(count);
        }


        public interface IView {
            void OnCollectResource(int count);
            void OnUpdateTaskList();
        }

        private sealed class NoView : Titan.IView {
            public void OnCollectResource(int count) {
                Debug.Log("Res collected");
            }

            public void OnUpdateTaskList() {
                Debug.Log("On Update task list ");
            }
        }
    }

}
