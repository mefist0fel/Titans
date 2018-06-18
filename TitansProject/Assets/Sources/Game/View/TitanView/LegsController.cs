using UnityEngine;

/// <summary>
/// Component controls legs steps order
/// </summary>
public sealed class LegsController : MonoBehaviour {
    [SerializeField]
    private Leg[] legs; // Set from editor
    [SerializeField]
    private float stepTime = 0.1f;

    private float timer;
    private int currentLegId;

	private void Start () {
		
	}
	
	private void Update () {
        timer -= Time.deltaTime;
        if (timer < 0) {
            MoveNextLeg();
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
