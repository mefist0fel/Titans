using UnityEngine;

public sealed class LaserBeamPool : MonoBehaviour {
    private static LaserBeamPool instance;

    private ObjectPool<LineRenderer> cache;

    [SerializeField]
    public LineRenderer laserLinePrototype; // Set from editor
    [SerializeField]
    public float laserWeight = 0.1f; // Set from editor
    [SerializeField]
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.2f, 1f), new Keyframe(1f, 0f) });

    private void Awake() {
        instance = this;
    }

    private void Start() {
        cache = new ObjectPool<LineRenderer>(laserLinePrototype);
    }

    public static void ShowHit(Vector3 from, Vector3 to, float time = 0.3f) {
        if (instance != null) {
            instance.ShowLaser(from, to, new Color(0.8f, 0.8f, 1), new Color(0, 0, 1, 0.66f), time);
        }
    }

    public static void ShowMiss(Vector3 from, Vector3 to, float time = 0.3f) {
        if (instance != null) {
            to += UnityEngine.Random.insideUnitSphere * 0.1f;
            instance.ShowLaser(from, to + (to - from) * 3f, new Color(0.8f, 0.8f, 1), new Color(0, 0, 1, 0), time);
        }
    }

    private void ShowLaser(Vector3 from, Vector3 to, Color start, Color end, float showTime) {
        var laser = cache.Get();
        laser.gameObject.SetActive(true);
        laser.widthMultiplier = 0;
        laser.widthCurve = AnimationCurve.Constant(0, 1, 1);
        laser.positionCount = 2;
        laser.SetPositions(new Vector3[] {from, to});
        laser.startColor = start;
        laser.endColor = end;
        Timer.Add(showTime,
            (anim) => {
                float width = Mathf.Max(0, curve.Evaluate(anim) * laserWeight);
                laser.widthMultiplier = width;
            },
            () => {
                laser.widthMultiplier = 0;
                laser.gameObject.SetActive(false);
            });
    }
}
