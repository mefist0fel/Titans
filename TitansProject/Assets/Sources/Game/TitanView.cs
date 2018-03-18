using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TitanView : MonoBehaviour {
    [SerializeField]
    public float speed = 10;

    private Faction faction;

	void Start () {
		
	}

	void Update () {
		
	}

    public void Init(Faction controlFaction, Vector3 position) {
        faction = controlFaction;
        transform.position = position;
        transform.rotation = Quaternion.LookRotation(position.normalized) * Quaternion.Euler(90, 0, 0);
    }
}
