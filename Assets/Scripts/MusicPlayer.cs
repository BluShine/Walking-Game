using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    static MusicPlayer player = null;
    public AudioClip music;

	// Use this for initialization
	void Start () {
		if(player == null)
        {
            player = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            AudioSource source = player.GetComponent<AudioSource>();
            if (source.clip.name != music.name)
            {
                source.clip = music;
                source.Play();
            }
            Destroy(gameObject);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
