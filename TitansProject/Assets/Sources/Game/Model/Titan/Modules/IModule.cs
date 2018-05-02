using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model {
    public interface IModule {
        void OnAttach(Titan titan);
        void OnDetach();
        void Update(float deltaTime);
    }
}
