using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    public Transform[] spawnTransforms;
    Vector3[] spawnPoints;
    Quaternion[] spawnRotations;

	// Use this for initialization
	void Start () {
        spawnPoints = new Vector3[spawnTransforms.Length];
        spawnRotations = new Quaternion[spawnTransforms.Length];
        for(int i = 0; i < spawnTransforms.Length; i++)
        {
            spawnPoints[i] = spawnTransforms[i].position;
            spawnRotations[i] = spawnTransforms[i].rotation;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Respawn()
    {
        for(int i = 0; i < spawnTransforms.Length; i++)
        {
            spawnTransforms[i].position = spawnPoints[i];
            spawnTransforms[i].rotation = spawnRotations[i];
        }
        foreach(Transform t in spawnTransforms)
        {
            foreach(Rigidbody b in t.GetComponentsInChildren<Rigidbody>())
            {
                b.velocity = Vector3.zero;
                b.angularVelocity = Vector3.zero;
            }
        }
    }
}
