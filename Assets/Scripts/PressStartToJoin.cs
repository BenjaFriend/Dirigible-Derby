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
    public int maxPlayerCount = 4;

    public GameObject playerPrefab;

    private int _currentPlayerCount = 0;
    #endregion

    private void Update()
    {
        // Watch for JoinGame action in each Player
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            if (ReInput.players.GetPlayer(i).GetButtonDown(Constants.RewiredInputActions.JoinGame))
            {
                createNextPlayer(i);
            }
        }
    }

    private void createNextPlayer(int rewiredPlayerId)
    {
        if(_currentPlayerCount >= maxPlayerCount)
        {
            return;
        }

        // instatiate
        // give the rewired index

        _currentPlayerCount++;
    }

}
