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
    public int MaxPlayerCount = 4;

    public GameObject PlayerPrefab;

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

        // give the rewired index
        player.Initialize(rewiredPlayerId);

        _activePlayers.Add(rewiredPlayerId);
    }

}
