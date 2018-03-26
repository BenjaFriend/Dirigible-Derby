using UnityEngine;

/// <summary>
/// Spawns players when they press the JoinGame button with Rewired delegates.
/// Calls the GameManager to spawn those objects.
/// </summary>
public class PlayerLobbyController : SceneController
{
    #region Fields
    /// <summary>
    /// This is an object that says something like "Press Start to join" or something like that.
    /// Will be diabled when all players have joined the lobby
    /// </summary>
    public GameObject JoinUIObject;

    [Space(10)]
    [Header("Player attributes")]
    public Sprite[] BalloonSprites;
    public string[] PlayerNames;
    public Color[] PlayerColors;
    public SpriteRenderer[] PlayerReadyIndicators;
    public Color ReadyIndicatorColor = Color.green;
    public Color UnreadyIndicatorColor = Color.white;

    [Space(10)]
    [Header("Debug Info")]
    [SerializeField]
    [Tooltip("If false this script will not log and debug information")]
    private bool _showDebugLogs = true;

    /// <summary>
    /// List of what players have been added to the game to prevent the same player joining twice
    /// </summary>
    private bool[] _activePlayers;

    /// <summary>
    /// List of what players have confirmed that they are ready to start the 
    /// </summary>
    private bool[] _readyPlayers;


    private int _activePlayerCount;
    private int _readyPlayerCount;

    #endregion

    public override void OnControllerActivated()
    {
        _activePlayers = new bool[Constants.PlayerLobby.MaxPlayers];
        _readyPlayers = new bool[Constants.PlayerLobby.MaxPlayers];

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

        // Set the join UI object as disabled
        if (_activePlayerCount >= Constants.PlayerLobby.MaxPlayers && JoinUIObject != null)
        {
            JoinUIObject.SetActive(false);
        }
    }

    private bool isPlayerActive(int playerID)
    {
        return _activePlayers[playerID];
    }

    private bool isPlayerReady(int playerID)
    {
        // Ensure that this number is within the range of our array just in case
        if (playerID < 0 || playerID > Constants.PlayerLobby.MaxPlayers) return false;

        return _readyPlayers[playerID];
    }

    /// <summary>
    /// Leave the player lobby and go to the game
    /// </summary>
    private void startGame()
    {
        //TODO: remove hardcoded game scene
        GameManager.Instance.ChangeScene(Constants.Scenes.Playtesting);
    }

    /// <summary>
    /// Sets this player index as ready.
    /// If all players are now ready, then start the game
    /// </summary>
    private void readyPlayer(int rewiredId)
    {
        // Check if all players are ready
        _readyPlayers[rewiredId] = true;
        _readyPlayerCount++;

        // Set the little sprite as green
        if (PlayerReadyIndicators[rewiredId] != null)
        {
            PlayerReadyIndicators[rewiredId].color = ReadyIndicatorColor;
        }

        // Check if we need to star the game
        if (_readyPlayerCount >= _activePlayerCount)
        {
            startGame();
        }
    }

    /// <summary>
    /// Sets this player index as not ready.
    /// </summary>
    private void unreadyPlayer(int rewiredId)
    {
        _readyPlayers[rewiredId] = false;
        _readyPlayerCount--;

        // Set the indicator back to white
        if(PlayerReadyIndicators[rewiredId] != null)
        {
            PlayerReadyIndicators[rewiredId].color = UnreadyIndicatorColor;
        }

        // Cancel the countdown here if we need to!
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

            Rewired.ReInput.players.GetPlayer(i).AddInputEventDelegate(
                onCancelPressedEvent,
                Rewired.UpdateLoopType.Update,
                Rewired.InputActionEventType.ButtonJustPressed,
                Constants.RewiredInputActions.Cancel
                );
        }
    }

    private void removeReiwredDelegates()
    {
        // stop listening for join game pressed for all inactive players
        for (int i = 0; i < Rewired.ReInput.players.playerCount; i++)
        {
            Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(onJoinGamePressedEvent);
            Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(onCancelPressedEvent);
        }
    }

    private void onJoinGamePressedEvent(Rewired.InputActionEventData data)
    {
        if (!isPlayerActive(data.playerId))
        {
            activatePlayer(data.playerId);
        }
        else if(!isPlayerReady(data.playerId))
        {
            readyPlayer(data.playerId);
        }
    }

    private void onCancelPressedEvent(Rewired.InputActionEventData data)
    {
        if(isPlayerReady(data.playerId))
        {
            unreadyPlayer(data.playerId);
        }
    }

    #endregion

    #region Logging
    private void logWarningFormat(string format, params object[] args)
    {
        if (!_showDebugLogs) return;
#if UNITY_EDITOR
        Debug.LogWarningFormat("[PlayerLobbyController] " + format, args);
#endif
    }

    private void logErrorFormat(string format, params object[] args)
    {
        if (!_showDebugLogs) return;

#if UNITY_EDITOR
        Debug.LogErrorFormat("[PlayerLobbyController] " + format, args);
#endif
    }

    private void logFormat(string format, params object[] args)
    {
        if (!_showDebugLogs) return;

#if UNITY_EDITOR
        Debug.LogFormat("[PlayerLobbyController] " + format, args);
#endif
    }
    #endregion
}
