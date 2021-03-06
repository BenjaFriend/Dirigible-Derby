﻿using UnityEngine;
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

    public delegate void OnPlayerHealthChangedEvent(PlayerController p);
    public OnPlayerHealthChangedEvent OnPlayerHealthChanged;

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

        UIEliminationStatsManager uiStatsManager = GameObject.FindObjectOfType<UIEliminationStatsManager>();
        if (uiStatsManager == null) Debug.LogError("[EliminationGamemode] Couldn't find UIEliminationStatsManager in scene!");
        uiStatsManager.Initialize();

        UIWinTextController winTextController = GameObject.FindObjectOfType<UIWinTextController>();
        if (winTextController == null) Debug.LogError("[EliminationGamemode] Couldn't find UIWinTextController in scene!");
        winTextController.Initialize();
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
        _playerLives[index]--;
        OnPlayerHealthChanged(playerPopped);

        logFormat("Player {0} popped! {1} lives left", index, _playerLives[index]);

        if(isPlayerAlive(index))
        {
            // respawn player
            playerPopped.RespawnDelayed(Constants.Gamemodes.Elimination.RespawnTime);
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
        PlayerController winner = null;
        for (int i = 0; i < Constants.PlayerLobby.MaxPlayers; i++)
        {
            if (_playerLives[i] > 0)
                winner = gameManager.Players[i];
        }

        gameSceneController.GameOver(winner);
    }

    /// <summary>
    /// Checks whether or not a player is alive
    /// </summary>
    private bool isPlayerAlive(int index)
    {
        return _playerLives[index] > 0;
    }

    #region Logging
    private void logFormat(string format, params object[] args)
    {
        Debug.LogFormat("[EliminationGamemode] " + format, args);
    }

    private void logWarningFormat(string format, params object[] args)
    {
        Debug.LogWarningFormat("[EliminationGamemode] " + format, args);
    }

    private void logErrorFormat(string format, params object[] args)
    {
        Debug.LogErrorFormat("[EliminationGamemode] " + format, args);
    }
    #endregion
}
