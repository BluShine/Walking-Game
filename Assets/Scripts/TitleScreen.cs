
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

    public float delayTimer = 5;
    public float slideTimer = 4;
    public float slideSpeed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        } else if(slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;
            transform.position += Vector3.down * slideSpeed * Time.deltaTime;
        }
	}
}
