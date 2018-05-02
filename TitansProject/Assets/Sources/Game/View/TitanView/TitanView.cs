using System;
using Model;
using UnityEngine;

namespace View {
    public sealed class TitanView : MonoBehaviour, Titan.IView {
        public Titan Titan { get; private set; }
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

        private void Update() {
            transform.position = Titan.Position;
            transform.rotation = Titan.UpRotation;
        }
    }
}