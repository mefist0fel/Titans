using Configs;
using UnityEngine;

public class BuildTitanModule : MonoBehaviour, ITitanModule {
    public float FullTime { get; private set; }
    public float NormalizedTime { get { return timer / FullTime; } }

    private float timer;
    private TitanView controlledTitan;
    private ModuleData titanData;

    public static BuildTitanModule Create(ModuleData titanModule) {
        GameObject buildObject = new GameObject();
        var module = buildObject.AddComponent<BuildTitanModule>();
        module.FullTime = titanModule.BuildTime;
        module.timer = titanModule.BuildTime;
        module.titanData = titanModule;
        return module;
    }

    public void OnAttach(TitanView titan) {
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
        if (titanData.Id == "titan") {
            Game.Instance.Factions[0].AddUnit(Game.Instance.CreateTitan("Prefabs/titan"), Quaternion.Euler(3f, 0, 0) * controlledTitan.Position);
        } else {
            Game.Instance.Factions[0].AddUnit(Game.Instance.CreateTitan("Prefabs/titan_enemy"), Quaternion.Euler(20f, 0, 0) * controlledTitan.Position);
        }
        controlledTitan.Attach(null, 12);
    }
}
