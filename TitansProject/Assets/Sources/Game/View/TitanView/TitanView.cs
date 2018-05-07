using System;
using Model;
using UnityEngine;

namespace View {
    public sealed class TitanView : MonoBehaviour, Titan.IView {
        [SerializeField]
        private Transform hitTransfom;
        [SerializeField]
        private Material deathMaterial;

        public Titan Titan { get; private set; }
        public Action UpdateModules;
        public Action UpdateTaskList;
        public Action<int> UpdateResources;

        public void Init(Titan controlTitan) {
            Titan = controlTitan;
            Titan.SetView(this);
            transform.position = Titan.Position;
            transform.rotation = Titan.UpRotation;
        }

        public void OnCollectResource(int count) {
            if (UpdateResources != null)
                UpdateResources(count);
        }

        public void OnUpdateTaskList() {
            if (UpdateTaskList != null)
                UpdateTaskList();
        }

        public void OnUpdateModules() {
            if (UpdateModules != null)
                UpdateModules();
        }

        private void Update() {
            transform.position = Titan.Position;
            transform.rotation = Titan.UpRotation;
        }

        public void OnHit(int damage) {
            Debug.Log("I'm hit! on " + damage);
            StatusTextView.Create(damage.ToString(), Color.red, hitTransfom.position);
        }

        public void OnDie() {
            Debug.Log("I'm Die!");
            var renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers) {
                renderer.material = deathMaterial;
            }
        }
    }
}