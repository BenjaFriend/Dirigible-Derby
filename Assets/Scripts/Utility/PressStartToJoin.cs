using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

using UnityEngine.UI;

/// <summary>
/// This class will be in charge 
/// TODO: I don't want this class to be in charge. 
/// Move this stuff to the player lobby manager or the game manager
/// </summary>
public class PressStartToJoin : MonoBehaviour
{

    #region Fields
    /// <summary>
    /// The max number of players that can be in the game
    /// </summary>
    public int MaxPlayerCount = 4;

    /// <summary>
    /// The player prefab to be instantiated when they press start
    /// </summary>
    public GameObject PlayerPrefab;

    /// <summary>
    /// A list of transforms that will be used as spawn positions
    /// </summary>
    public List<Transform> SpawnPoints;

    public List<Sprite> balloonSprites;

    public List<string> PlayerNames;
    public List<Color> playerColors;

    /// <summary>
    /// List of what players have been added to the game to prevent the same player joining twice
    /// </summary>
    private List<int> _activePlayers;
    #endregion

    private void Awake()
    {
        _activePlayers = new List<int>();
    }

    private void Update()
    {
        // Watch for JoinGame action in each Player
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (_activePlayers.Contains(i)) // skip players who have already joined
                continue;

            if (ReInput.players.GetPlayer(i).GetButtonDown(Constants.RewiredInputActions.JoinGame))
            {
                createNextPlayer(i);
            }
        }
    }

    /// <summary>
    /// Creates an instance of the player prefab, sets the layer to the proper
    /// one, and initializes their rewired input.
    /// </summary>
    /// <param name="rewiredPlayerId">The rewired player who we are instantiating</param>
    private void createNextPlayer(int rewiredPlayerId)
    {
        if(_activePlayers.Count >= MaxPlayerCount)
        {
            return;
        }

        Vector3 spawnPoint = new Vector3(0, 5, 0);

        // Set the spawn location of the player to one of the spawn points if there is one
        try
        {
            spawnPoint = SpawnPoints[_activePlayers.Count].position;
        }
        catch(System.Exception e)
        {
            Debug.LogWarning("There is no spawn point for player " + _activePlayers.Count.ToString() + "\nError: \n" + e.Message);
        }
        
        // instatiate the player
        GameObject playerObj = Instantiate<GameObject>(PlayerPrefab, spawnPoint, Quaternion.identity);
        PlayerController player = playerObj.GetComponent<PlayerController>();

        // Set player indicator color and text
        setPlayerIndicators(rewiredPlayerId, playerObj);

        playerObj.GetComponent<PlayerEngineFire>().InitRewired (rewiredPlayerId);

        // Set the gameobject on the correct physics layer! 
        switch (rewiredPlayerId)
        {
            case 0:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_0_Layer);
                player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[0];
                break;
            case 1:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_1_Layer);
                player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[1];

                break;
            case 2:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_2_Layer);
                player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[2];

                break;
            case 3:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_3_Layer);
                player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[3];

                break;
            default:
                Debug.Log("There is no player physics layer for index " + rewiredPlayerId.ToString());
                player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[0];

                break;
        }

        // give the rewired index
        player.Initialize(new PlayerData(rewiredPlayerId));

        _activePlayers.Add(rewiredPlayerId);
    }

    /// <summary>
    /// Iterate through the object and set its layer to the paramater
    /// </summary>
    /// <param name="obj">Object to change it and all children to</param>
    /// <param name="layer">Layer to put these objects on</param>
    private static void setLayerOfObject(Transform obj, int layer)
    {
        Stack<Transform> moveTargets = new Stack<Transform>();
        moveTargets.Push(obj);

        Transform currentTarget;
        while (moveTargets.Count != 0)
        {
            currentTarget = moveTargets.Pop();
            currentTarget.gameObject.layer = layer;
            foreach (Transform child in currentTarget)
                moveTargets.Push(child);
        }
    }

    /// <summary>
    /// Sets color of engine particles and player name.
    /// </summary>
    /// <param name="rewiredPlayerId"></param>
    /// <param name="playerPrefab"></param>
    private void setPlayerIndicators(int rewiredPlayerID, GameObject playerObj)
    {
        Debug.Log("Setting up Player: " + rewiredPlayerID + " | Color: " + playerColors[rewiredPlayerID]);

        setIndicatorColors(rewiredPlayerID, playerObj, playerColors[rewiredPlayerID]);
        setIndicatorText(rewiredPlayerID, playerObj);
    }

    /// <summary>
    /// Sets colors for indicator.
    /// </summary>
    /// <param name="rewiredPlayerID"></param>
    /// <param name="playerPrefab"></param>
    /// <param name="color"></param>
    private void setIndicatorColors(int rewiredPlayerID, GameObject playerObj, Color color)
    {
        Debug.Log("Setting Colors for player: " + rewiredPlayerID + " | Color: " + playerColors[rewiredPlayerID]);

        var particlesLeft = playerObj.transform.Find("Left Prop").Find("Engine Fire_Left").GetComponent<ParticleSystem>().main;
        var particlesRight = playerObj.transform.Find("Right Prop").Find("Engine Fire_Right").GetComponent<ParticleSystem>().main;
        
        particlesLeft.startColor = color;
        particlesRight.startColor = color;

        playerObj.transform.Find("Indicator").Find("Name").GetComponent<Text>().color = color;
    }

    /// <summary>
    /// Sets text for indicator.
    /// </summary>
    /// <param name="rewiredPlayerID"></param>
    /// <param name="playerPrefab"></param>
    private void setIndicatorText(int rewiredPlayerID, GameObject playerObj)
    {
        Debug.Log("Setting Text for player: " + rewiredPlayerID + " | Text: " + PlayerNames[rewiredPlayerID]);

        var text = PlayerNames[rewiredPlayerID];
        playerObj.transform.Find("Indicator").Find("Name").GetComponent<Text>().text = text;
    }
		
}
