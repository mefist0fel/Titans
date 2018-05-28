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

        // Components
        public readonly Shield Shield;
        public readonly Laser Laser;

        private readonly Battle battle;

        private TitanMover mover;
        private IView view = new NoView();
        public IView View { get { return view; } }

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
            Shield = new Shield(UpdateShield);
            Laser = new Laser(this, battle);
            // TODO kill
            ResourceUnits = 20;
        }

        public void Hit(Damage damage) {
            view.OnHit(damage.Value);
            Armor -= damage.Value;
            if (Armor < 0) {
                Die();
            }
        }

        public void Die() {
            Armor = 0;
            taskList.Clear();
            view.OnDie();
        }

        public void SetView(IView titanView) {
            if (view == null)
                throw new NullReferenceException("Try set titan view but view is null");
            view = titanView;
        }

        public void Update(float deltaTime) {
            Shield.Update(deltaTime);
            Laser.Update(deltaTime);
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

        public Task GetCurrentTask() {
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

        public void CancelTasks(Task[] tasks) {
            if (!IsAlive)
                return;
            foreach (var task in tasks)
                TaskList.Remove(task);
            view.OnUpdateTaskList();
        }

        public void AddResourceTask(ResourcePoint resourcePoint) {
            taskList.Add(new MoveTask(resourcePoint.Position, mover));
            taskList.Add(new ResourceTask(resourcePoint, ChangeResourceCount));
            view.OnUpdateTaskList();
        }

        public void AddMoveTask(Vector3 targetPosition) {
            taskList.Add(new MoveTask(targetPosition, mover));
            view.OnUpdateTaskList();
        }

        public void ChangeResourceCount(int count) {
            ResourceUnits += count;
            view.OnCollectResource(count);
        }

        public void UpdateModules() {
            view.OnUpdateModules();
        }

        private void UpdateShield() {
            view.OnUpdateShield();
        }

        public interface IView {
            void OnCollectResource(int count);
            void OnUpdateTaskList();
            void OnUpdateModules();
            void OnUpdateShield();
            void OnHit(int damage);
            void OnDie();
        }

        private sealed class NoView : Titan.IView {
            public void OnCollectResource(int count) {
                Debug.Log("Res collected");
            }

            public void OnUpdateTaskList() {
                Debug.Log("On Update task list ");
            }

            public void OnUpdateModules() {
                Debug.Log("On Update modules ");
            }

            public void OnUpdateShield() {
                Debug.Log("On Update shield ");
            }

            public void OnHit(int damage) {
                Debug.Log("On Hit titan " + damage);
            }

            public void OnDie() {
                Debug.Log("On Titan Die ");
            }
        }
    }

}
