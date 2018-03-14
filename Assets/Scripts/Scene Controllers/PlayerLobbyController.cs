using System.Collections.Generic;
using UnityEngine;

public class PlayerLobbyController : SceneController
{
    #region Fields

    public Sprite[] BalloonSprites;
    public string[] PlayerNames;
    public Color[] PlayerColors;

    /// <summary>
    /// List of what players have been added to the game to prevent the same player joining twice
    /// </summary>
    private bool[] _activePlayers;

    private int _activePlayerCount;

    #endregion

    public override void OnControllerActivated()
    {
        _activePlayers = new bool[Constants.PlayerLobby.MaxPlayers];

        // find spawn points and tell gm
        GameManager.Instance.SetSpawnPoints(GameObject.FindGameObjectsWithTag(Constants.Tags.SpawnPoint));

        addRewiredDelegates();
    }

    public override void OnControllerDeactivated()
    {
        removeReiwredDelegates();
    }

    private void activatePlayer(int playerID)
    {
        if (isPlayerActive(playerID))
        {
            logWarningFormat("Attempt to activate player {0}, but player is already active!", playerID);
            return;
        }

        _activePlayers[playerID] = true;
        _activePlayerCount++;

        int playerIndex = _activePlayerCount - 1;

        // stop listening for join game press
        //Rewired.ReInput.players.GetPlayer(playerID).RemoveInputEventDelegate(onJoinGamePressedEvent);

        // get playerdata from game manager, or create new playerdata instance if this player is null in gm
        PlayerData playerData = GameManager.Instance.PlayerData[playerIndex] ?? new PlayerData();

        // set player data (TODO: do this via ui before spawning player)
        playerData.RewiredPlayerID = playerID;
        playerData.ID = playerIndex;
        playerData.Active = true;

        // set sprites (TOOD: should be done via UI)
        playerData.BalloonSprite = BalloonSprites[playerIndex];
        playerData.Color = PlayerColors[playerIndex];
        playerData.Name = PlayerNames[playerIndex];

        GameManager.Instance.SetPlayerData(playerIndex, playerData);

        // spawn player (TODO: this should happen after the player is done customizing)
        GameManager.Instance.SpawnPlayer(playerIndex);
    }

    private bool isPlayerActive(int playerID)
    {
        return _activePlayers[playerID];
    }

    /// <summary>
    /// Leave the player lobby and go to the game
    /// </summary>
    private void startGame()
    {
        //TODO: remove hardcoded game scene
        GameManager.Instance.ChangeScene(Constants.Scenes.Playtesting);
    }

    #region Rewired Delegates

    private void addRewiredDelegates()
    {
        // start listening for join game pressed for all players
        for (int i = 0; i < Rewired.ReInput.players.playerCount; i++)
        {
            Rewired.ReInput.players.GetPlayer(i).AddInputEventDelegate(
                onJoinGamePressedEvent,
                Rewired.UpdateLoopType.Update,
                Rewired.InputActionEventType.ButtonJustPressed,
                Constants.RewiredInputActions.JoinGame
                );
        }
    }

    private void removeReiwredDelegates()
    {
        // stop listening for join game pressed for all inactive players
        for (int i = 0; i < Rewired.ReInput.players.playerCount; i++)
        {
            //if (!isPlayerActive(i))
                Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(onJoinGamePressedEvent);
        }
    }

    private void onJoinGamePressedEvent(Rewired.InputActionEventData data)
    {
        if (!isPlayerActive(data.playerId))
            activatePlayer(data.playerId);
        else // start game (TODO: probably want a different system for this)
            startGame();
    }

    #endregion

    #region Logging
    private void logWarningFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogWarningFormat("[PlayerLobbyController] " + format, args);
#endif
    }

    private void logErrorFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogErrorFormat("[PlayerLobbyController] " + format, args);
#endif
    }

    private void logFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogFormat("[PlayerLobbyController] " + format, args);
#endif
    }
    #endregion
}
