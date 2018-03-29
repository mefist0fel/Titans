using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RocketComponent : MonoBehaviour, ITitanComponent {
    public void Attach(TitanView titan) {
    }

    public void Detach() {
    }

    public IInterfaceController[] GetInterfaceControllers() {
        return null;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
