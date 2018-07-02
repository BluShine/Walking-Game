using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBonk : MonoBehaviour {

    public float maxPitch = 2;
    public float minPitch = .5f;
    public float maxVel = 1;

    public float maxVolume = .6f;

    AudioSource audio;

	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / maxVel) * maxVolume;
        float pitch = Random.Range(minPitch, maxPitch);
        audio.volume = volume;
        audio.pitch = pitch;
        audio.Play();
    }
}
