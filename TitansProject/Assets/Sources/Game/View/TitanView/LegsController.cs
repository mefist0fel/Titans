using UnityEngine;

/// <summary>
/// Component controls legs steps order
/// </summary>
public sealed class LegsController : MonoBehaviour {
    [SerializeField]
    private LegStepTarget[] legs; // Set from editor
    [SerializeField]
    private float stepTime = 0.1f;
    [SerializeField]
    private int stepCount = 1;

    private float timer;
    private int currentLegId;

	private void Start () {
		
	}
	
	private void Update () {
        timer -= Time.deltaTime;
        if (timer < 0) {
            for (int i = 0; i < stepCount; i++) {
                MoveNextLeg();
            }
            timer = stepTime;
        }
	}

    private void MoveNextLeg() {
        var currentLeg = legs[currentLegId];
        currentLeg.StopStep();
        currentLegId = (currentLegId + 1) % legs.Length;
        var nextLeg = legs[currentLegId];
        nextLeg.StartStep();
    }
}
