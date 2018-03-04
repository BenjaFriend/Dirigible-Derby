using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey("f1"))
        {
            Debug.Log("play");

            SceneManager.LoadScene("InputTesting");
        }

        if (Input.GetKey("f2"))
        {
            Debug.Log("quit");

            Application.Quit();
        }
	}
}
