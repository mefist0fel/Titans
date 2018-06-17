using System;
using Model;
using UnityEngine;

namespace View {
    public sealed class TitanView : MonoBehaviour, Titan.IView {
        [SerializeField]
        private Transform hitTransfom;
        [SerializeField]
        private Material deathMaterial;
        [SerializeField]
        private ShieldView shieldView;
        [SerializeField]
        private LivesView livesView;
        [SerializeField]
        private ParticleSystem resourceCollectionParticles;

        public Titan Titan { get; private set; }
        public Action UpdateModules;
        public Action UpdateTaskList;
        public Action<int> UpdateResources;

        public void Init(Titan controlTitan) {
            Titan = controlTitan;
            Titan.SetView(this);
            name = Titan.Name;
            transform.position = Titan.Position;
            transform.rotation = Titan.UpRotation;
            UpdateResourceCollectionEffect();
            OnUpdateLives();
        }

        public void OnCollectResource(int count) {
            if (UpdateResources != null)
                UpdateResources(count);
            UpdateResourceCollectionEffect();
        }

        public void OnUpdateTaskList() {
            if (UpdateTaskList != null)
                UpdateTaskList();
            UpdateResourceCollectionEffect();
        }

        public void OnUpdateModules() {
            if (UpdateModules != null)
                UpdateModules();
        }

        public void OnUpdateLives() {
            if (shieldView != null)
                shieldView.UpdateState(Titan.Shield.Capacity);
            if (livesView != null) {
                var armor = Titan.Armor;
                var shield = Titan.Shield;
                livesView.UpdateState(armor.Value, armor.MaxValue, shield.Capacity, shield.MaxValue);
            }
        }

        private void Update() {
            transform.position = Titan.Position;
            transform.rotation = Titan.UpRotation;
        }

        public void OnHit(Damage damage) {
            if (!Titan.IsAlive)
                return;
            if (damage.Value == 0) {
                StatusTextView.Create("Miss", Color.white, hitTransfom.position);
                return;
            }
            OnUpdateLives();
            StatusTextView.Create(damage.Value.ToString(), damage.Critical ? Color.red : Color.yellow , hitTransfom.position);
        }

        public Vector3 GetHitPoint() {
            return hitTransfom.transform.position;
        }

        public void OnDie() {
            Debug.Log("I'm Die!");
            livesView.UpdateState(0, 0, 0, 0);
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers) {
                renderer.material = deathMaterial;
            }
        }

        private void UpdateResourceCollectionEffect() {
            if (resourceCollectionParticles != null) {
                var needShowEffect = Titan.GetCurrentTask() is ResourceTask;
                if (needShowEffect) {
                    resourceCollectionParticles.Play();
                } else {
                    resourceCollectionParticles.Stop();
                }
            }
        }
    }
}