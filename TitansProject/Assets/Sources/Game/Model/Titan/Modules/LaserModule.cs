using UnityEngine;

namespace Model {
    public sealed class LaserModule : IModule {
        private Titan titan;
      //  private int damage = 3;
      //  private float fireRadius = 2f;
      //  private float accuracy = 0.5f;
      //  private float reloadTime = 1.5f;
      //  private float reloadTimeRandomShift = 0.2f;
      //  private float timer = 0f;
      //  private Titan target = null;
        public string Id { get; private set; }

    //    public bool IsReady { get { return timer <= 0 && hostTitan != null && hostTitan.IsAlive; } }

        public LaserModule(ModuleData data) {
            Id = data.Id;
        }

        public void OnAttach(Titan parentTitan) {
            titan = parentTitan;
            titan.Laser.SetDamate(3);
        }

        public void OnDetach() {
            titan.Laser.SetDamate(-3);
            titan = null;
        }

        public void Update(float deltaTime) {
        }
    }
}
