using Configs;
using UnityEngine;

public class BuildTitanModule : MonoBehaviour, ITitanModule {
    public float FullTime { get; private set; }
    public float NormalizedTime { get { return timer / FullTime; } }

    private float timer;
    private TitanViewOld controlledTitan;
    private ModuleData titanData;

    public static BuildTitanModule Create(ModuleData titanModule) {
        GameObject buildObject = new GameObject();
        var module = buildObject.AddComponent<BuildTitanModule>();
        module.FullTime = titanModule.BuildTime;
        module.timer = titanModule.BuildTime;
        module.titanData = titanModule;
        return module;
    }

    public void OnAttach(TitanViewOld titan) {
        controlledTitan = titan;
    }

    public void OnDetach() {
        controlledTitan = null;
        Destroy(gameObject);
    }

    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            BuildTitan();
        }
    }

    private void BuildTitan() {
        var pos = controlledTitan.Position;
        var newPosition = (controlledTitan.Position + Quaternion.LookRotation(controlledTitan.Position) * Vector3.up).normalized * Game.Instance.Planet.Radius;
        if (titanData.Id == "titan") {
            Game.Instance.Factions[0].AddUnit(Game.Instance.CreateTitan("Prefabs/titan"), newPosition);
        } else {
            Game.Instance.Factions[1].AddUnit(Game.Instance.CreateTitan("Prefabs/titan_enemy"), newPosition);
        }
        controlledTitan.Attach(null, 12);
    }
}

