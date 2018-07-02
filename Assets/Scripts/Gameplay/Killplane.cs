using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killplane : MonoBehaviour {

    Respawner respawner;

	// Use this for initialization
	void Start () {
        respawner = FindObjectOfType<Respawner>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        respawner.Respawn();
    }
}
