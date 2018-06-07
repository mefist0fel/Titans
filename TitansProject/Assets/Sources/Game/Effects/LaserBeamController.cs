using System.Collections.Generic;
using UnityEngine;

public sealed class LaserBeamController : MonoBehaviour {
    private static LaserBeamController instance;

    private List<LineRenderer> cache = new List<LineRenderer>();

    [SerializeField]
    public LineRenderer laserLinePrototype; // Set from editor
    [SerializeField]
    public float laserWeight = 0.1f; // Set from editor
    [SerializeField]
    public AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.2f, 1f), new Keyframe(1f, 0f) });

    private void Awake() {
        instance = this;
    }

    public static void Show(Vector3 from, Vector3 to, float time) {
        if (instance != null) {
            instance.ShowLaser(from, to, Color.red, Color.red, time);
        }
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
        var laser = CreateLaser();
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

    private LineRenderer CreateLaser() {
        foreach (var laser in cache) {
            if (laser == null) {
                Debug.LogError("Empty laser in laser cache!");
                continue;
            }
            if (!laser.gameObject.activeSelf) {
                return laser;
            }
        }
        var newLaser = Instantiate<LineRenderer>(laserLinePrototype, transform);
        newLaser.gameObject.SetActive(false);
        cache.Add(newLaser);
        return newLaser;
    }
}
