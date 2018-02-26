using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// This class will be in charge 
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

    private void createNextPlayer(int rewiredPlayerId)
    {
        if(_activePlayers.Count >= MaxPlayerCount)
        {
            return;
        }

        // instatiate
        GameObject playerObj = Instantiate<GameObject>(PlayerPrefab);
        PlayerController player = playerObj.GetComponent<PlayerController>();

        // Set the gameobject on the correct physics layer! 
        switch (rewiredPlayerId)
        {
            case 0:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_0_Layer);
                break;
            case 1:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_1_Layer);
                break;
            case 2:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_2_Layer);
                break;
            case 3:
                setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_3_Layer);
                break;
            default:
                Debug.Log("There is no player physics layer for index " + rewiredPlayerId.ToString());
                break;
        }

        // give the rewired index
        player.Initialize(rewiredPlayerId);

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

}
