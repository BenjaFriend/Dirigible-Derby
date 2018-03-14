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

    //#region Fields
    //#endregion

    //private void Awake()
    //{
    //    _activePlayers = new List<int>();
    //}

    //private void Update()
    //{
    //    // Watch for JoinGame action in each Player
    //    for (int i = 0; i < ReInput.players.playerCount; i++)
    //    {
    //        if (_activePlayers.Contains(i)) // skip players who have already joined
    //            continue;

    //        if (ReInput.players.GetPlayer(i).GetButtonDown(Constants.RewiredInputActions.JoinGame))
    //        {
    //            createNextPlayer(i);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Creates an instance of the player prefab, sets the layer to the proper
    ///// one, and initializes their rewired input.
    ///// </summary>
    ///// <param name="rewiredPlayerId">The rewired player who we are instantiating</param>
    //private void createNextPlayer(int rewiredPlayerId)
    //{
    //    if(_activePlayers.Count >= MaxPlayerCount)
    //    {
    //        return;
    //    }

    //    Vector3 spawnPoint = new Vector3(0, 5, 0);

    //    // Set the spawn location of the player to one of the spawn points if there is one
    //    try
    //    {
    //        spawnPoint = SpawnPoints[_activePlayers.Count].position;
    //    }
    //    catch(System.Exception e)
    //    {
    //        Debug.LogWarning("There is no spawn point for player " + _activePlayers.Count.ToString() + "\nError: \n" + e.Message);
    //    }
        
    //    // instatiate the player
    //    GameObject playerObj = Instantiate<GameObject>(PlayerPrefab, spawnPoint, Quaternion.identity);
    //    PlayerController player = playerObj.GetComponent<PlayerController>();

    //    // Set player indicator color and text
    //    setPlayerIndicators(rewiredPlayerId, playerObj);

    //    playerObj.GetComponent<PlayerEngineFire>().InitRewired (rewiredPlayerId);

    //    // Set the gameobject on the correct physics layer! 
    //    switch (rewiredPlayerId)
    //    {
    //        case 0:
    //            setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_0_Layer);
    //            player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[0];
    //            break;
    //        case 1:
    //            setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_1_Layer);
    //            player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[1];

    //            break;
    //        case 2:
    //            setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_2_Layer);
    //            player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[2];

    //            break;
    //        case 3:
    //            setLayerOfObject(player.transform, Constants.PlayerPhysicsLayers.Player_3_Layer);
    //            player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[3];

    //            break;
    //        default:
    //            Debug.Log("There is no player physics layer for index " + rewiredPlayerId.ToString());
    //            player.GetComponent<QuickPlayerSprites>().balloonSprite.sprite = balloonSprites[0];

    //            break;
    //    }

    //    // give the rewired index
    //    player.Initialize(new PlayerData(rewiredPlayerId));

    //    _activePlayers.Add(rewiredPlayerId);
    //}

}
