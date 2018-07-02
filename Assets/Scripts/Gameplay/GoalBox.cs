using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalBox : MonoBehaviour {

    public float waitTime = 2;
    public GameObject winText;
    public Camera winCamera;
    public GameObject lampHead;
    public string nextScene;

    bool win = false;

	// Use this for initialization
	void Start () {
        winCamera.enabled = false;
        winText.SetActive(false);
        lampHead.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	    if(win)
        {
            waitTime -= Time.deltaTime;
        }
        if(waitTime <= 0)
        {
            SceneManager.LoadScene(nextScene);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        win = true;
        winCamera.enabled = true;
        winCamera.depth = 100;
        winText.SetActive(true);
        lampHead.SetActive(true);
    }
}
