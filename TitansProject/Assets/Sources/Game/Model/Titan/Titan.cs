using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    [System.Serializable]
    public sealed class Titan {
        public interface IComponent {
            void OnAttach(ModuleData module);
            void OnDetach(ModuleData module);
            void Update(float deltaTime);
        }

        public readonly string Name;
        public readonly Faction Faction;

        public Vector3 Position { get { return mover.Position; } }
        public Quaternion UpRotation { get { return mover.UpRotation; } }

        public int MaxArmor { get; private set; }
        public int AFrmor { get; private set; }

        public int ResourceUnits { get; private set; }
        public float Speed { get; private set; }
        
        public readonly ModuleSlot[] ModuleSlots;

        public readonly Armor Armor;
        public readonly Shield Shield;
        public readonly Accuracy Accuracy;
        public readonly Cloaking Cloaking;
        public readonly AntiAirDefence AntiAirDefence; 
        private readonly List<IComponent> Components;

        public readonly Battle Context;

        private TitanMover mover;
        private IView view = new NoView();
        public IView View { get { return view; } }

        private List<Task> taskList = new List<Task>(20);
        public List<Task> TaskList { get { return taskList; } }

        public bool IsAlive { get { return AFrmor > 0; } }

        public abstract class Task {
            public abstract bool IsEnded { get; }
            public abstract void MakeTask(float deltaTime);
        }

        public Titan(Faction faction, Battle battleContext, Vector3 position) {
            Faction = faction;
            Name = Faction.GetTitanName();
            Context = battleContext;
            Speed = 1f;
            AFrmor = 10;
            mover = new TitanMover(battleContext.Planet, position, Speed);
            ModuleSlots = new ModuleSlot[12];
            for (int i = 0; i < ModuleSlots.Length; i++) {
                ModuleSlots[i] = new ModuleSlot(this);
            }
            Shield = new Shield(UpdateLives);
            Armor = new Armor(UpdateLives);
            Accuracy = new Accuracy();
            Cloaking = new Cloaking();
            AntiAirDefence = new AntiAirDefence();
            Components = new List<IComponent>() {
                Armor,
                Shield,
                Accuracy,
                Cloaking,
                AntiAirDefence,
                new RocketLauncher(this, Context),
                new Laser(this, Context, Accuracy)
            };
            AddParams(Config.Base);
            Faction.AddUnit(this);
            // TODO kill
            ResourceUnits = 10;
            // ModuleSlots[0].Build(Config.Modules["anti_air"]);
        }

        public void Hit(Damage damage) {
            var damageValue = damage.Value;
            damageValue = Shield.OnHit(damageValue);
            Armor.OnHit(damageValue);
            view.OnHit(damage);
            if (Armor.Value <= 0) {
                Die();
            }
        }

        public Titan FindEnemyInRange(float radius) {
            return Faction.FindNearestEnemy(Position, radius);
        }

        public void Die() {
            AFrmor = 0;
            taskList.Clear();
            view.OnDie();
        }

        public void SetView(IView titanView) {
            if (view == null)
                throw new NullReferenceException("Try set titan view but view is null");
            view = titanView;
        }

        public void AddParams(ModuleData data) {
            foreach (var component in Components) {
                component.OnAttach(data);
            }
        }

        public void RemoveParams(ModuleData data) {
            foreach (var component in Components) {
                component.OnDetach(data);
            }
        }

        public void Update(float deltaTime) {
            if (!IsAlive)
                return;
            foreach (var component in Components) {
                component.Update(deltaTime);
            }
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
            view.OnUpdateTaskList();
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
            if (!IsAlive) {
                view.OnUpdateTaskList();
                return;
            }
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

        private void UpdateLives() {
            view.OnUpdateLives();
        }

        public interface IView {
            void OnCollectResource(int count);
            void OnUpdateTaskList();
            void OnUpdateModules();
            void OnUpdateLives();
            void OnHit(Damage damage);
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
            }

            public void OnUpdateLives() {}

            public void OnHit(Damage damage) {
                Debug.Log("On Hit titan " + damage.Value);
            }

            public void OnDie() {
                Debug.Log("On Titan Die ");
            }
        }
    }

}
