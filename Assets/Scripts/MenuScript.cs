using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using Rewired;

public class MenuScript : MonoBehaviour {

    public int playerId = 0; // Rewired player id

    private Player player; // Rewired player

    private bool start;
    private bool quit;

    private void Awake()
    {
        player = ReInput.players.GetPlayer(playerId);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        start = player.GetButtonDown("Start");
        quit = player.GetButtonDown("Quit");

        if (Input.GetKey("f1") || start)
        {
            Debug.Log("play");

            SceneManager.LoadScene("PrototypeLevel");
        }

        if (Input.GetKey("f2") || quit)
        {
            Debug.Log("quit");

            Application.Quit();
        }
	}
}
