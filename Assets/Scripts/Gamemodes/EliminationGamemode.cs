using UnityEngine;
using System.Collections;

/// <summary>
/// Elimination gamemode. Each player has 3 lives, last player alive wins!
/// </summary>
public class EliminationGamemode : Gamemode
{
    private int[] _playerLives;
    public int[] PlayerLives
    {
        get { return _playerLives; }
    }

    private int _playersAlive;

    public EliminationGamemode(GameSceneController parent) 
        : base(parent, GamemodeType.Elimination) { }

    public override void Initialize()
    {
        _playerLives = new int[Constants.PlayerLobby.MaxPlayers];

        for (int i = 0; i < Constants.PlayerLobby.MaxPlayers; i++)
        {
            if(gameManager.PlayerData[i] != null && gameManager.PlayerData[i].Active)
            {
                // set starting lives
                _playerLives[i] = Constants.Gamemodes.Elimination.Lives;

                // setup event handlers
                gameManager.Players[i].OnPlayerPopped += onPlayerPoppedEvent;

                _playersAlive++;
            }
        }
    }

    public override void Unload()
    {
    }

    public override void Update()
    {
    }

    private void onPlayerPoppedEvent(PlayerController playerPopped, PlayerController otherPlayer)
    {
        int index = playerPopped.PlayerData.ID;

        // lose a life
        PlayerLives[index]--;

        if(isPlayerAlive(index))
        {
            // respawn player
        }
        else
        {
            // check how many players are left
            _playersAlive--;
            if(_playersAlive == 1)
            {
                gameOver();
            }
        }
    }

    private void gameOver()
    {
        gameSceneController.GameOver();
    }

    /// <summary>
    /// Checks whether or not a player is alive
    /// </summary>
    private bool isPlayerAlive(int index)
    {
        return _playerLives[index] > 0;
    }
}
