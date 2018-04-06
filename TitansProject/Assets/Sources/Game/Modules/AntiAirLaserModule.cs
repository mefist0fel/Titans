using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class AntiAirLaserModule : MonoBehaviour, ITitanModule {
    [SerializeField]
    private float waitTime = 0.15f; // hit accuracy
    [SerializeField]
    private float accuracy = 0.5f; // hit accuracy
    [SerializeField]
    private int fireCount = 1;
    [SerializeField]
    private int rocketsCount = 10;
    [SerializeField]
    private float hitRadius = 2f;

    private TitanView titan;

    private List<RocketView> aimRockets = new List<RocketView>();

    public void OnAttach(TitanView parentTitan) {
        titan = parentTitan;
    }

    public void OnDetach() {
        titan = null;
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }

    public void Start () {
		
	}

    public void Update () {
        if (titan == null)
            return;
        if (!titan.IsAlive)
            return;
        if (rocketsCount == 0)
            return;
        int factionId = titan.FactionId;
        Vector3 position = titan.Position;
        foreach (var rocket in RocketView.RocketsList) {
            if (factionId == rocket.FactionId || aimRockets.Contains(rocket))
                continue;
            if (Vector3.Distance(rocket.Position, position) < hitRadius) {
                TryInterceptRocket(rocket);
            }
        }
	}

    private void TryInterceptRocket(RocketView rocket) {
        aimRockets.RemoveAll((emptyRocket) => { return (emptyRocket == null); });
        aimRockets.Add(rocket);
        var count = fireCount;
        if (rocketsCount < count)
            count = rocketsCount;
        rocketsCount -= count;
        for (int i = 0; i < count; i++) {
            float time = i * waitTime + 0.001f;
            Timer.Add(time, () => {
                if (rocket == null)
                    return;
                if (Random.Range(0f, 1f) < accuracy) {
                    rocket.Intercept();
                    LaserBeamController.ShowHit(titan.GetFirePosition(), rocket.Position);
                } else {
                    LaserBeamController.ShowMiss(titan.GetFirePosition(), rocket.Position);
                }
            });
        }
    }
}
