
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton controller for players, game state, and gamemode
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    //private Dictionary<int, PlayerData> _playerData;

    //public Dictionary<int, PlayerData> PlayerData;
    /// <summary>
    /// Per-player data and settings, like ReWired player index, balloon sprites, and color
    /// </summary>
    private PlayerData[] _playerData;
    public PlayerData[] PlayerData
    {
        get { return _playerData; }
        set
        {
            PlayerData = value;
            updatePlayerCount();
            // TODO: onplayerdatachanged delegate, or remove this and only use SetPlayerData
        }
    }

    private PlayerController[] _players;
    public PlayerController[] Players
    {
        get { return _players; }
    }

    private int _playerCount;
    public int PlayerCount
    {
        get { return _playerCount; }
    }

    public GameObject PlayerPrefab;

    private Transform[] _spawnPoints;
    public Transform[] SpawnPoints
    {
        get { return _spawnPoints; }
    }

    private bool _paused;
    /// <summary>
    /// Whether or not the game is paused
    /// </summary>
    public bool IsPaused
    {
        get { return _paused; }
    }

    public delegate void PauseEvent();
    public PauseEvent OnPause;
    public PauseEvent OnUnpause;

    public delegate void SceneChangeEvent();
    public SceneChangeEvent OnSceneChange;

    #region SceneController
    /// <summary>
    /// The active scene controller
    /// </summary>
    private SceneController _sceneController;

    /// <summary>
    /// The active scene contoller
    /// </summary>
    public SceneController SceneController
    {
        get { return _sceneController; }
    }

    /// <summary>
    /// Whether or not the current scene has a controller
    /// </summary>
    public bool SceneHasController
    {
        get { return _sceneController != null; }
    }

    /// <summary>
    /// Sets the currently active scene controller, and deactivates the previous one (if present)
    /// </summary>
    public void SetActiveSceneController(SceneController controller)
    {
        // deactivate old controller if we still have one
        if (SceneHasController)
            DeactivateCurrentController();

        // activate new controller
        _sceneController = controller;
        controller.OnControllerActivated();
    }

    /// <summary>
    /// Deactivates the current scene controller, you shouldn't ever need to call this yourself
    /// </summary>
    public void DeactivateCurrentController()
    {
        if (!SceneHasController)
        {
            logWarningFormat("Attempt to deactivate scene controller, but there already isn't one!");
        }

        _sceneController.OnControllerDeactivated();
        _sceneController = null;
    }
    #endregion SceneController

    /// <summary>
    /// Changes to a new scene
    /// </summary>
    public void ChangeScene(string nextScene)
    {
        if (SceneHasController)
            DeactivateCurrentController();

        if (OnSceneChange != null)
            OnSceneChange();

        // clear players array
        _players = new PlayerController[Constants.PlayerLobby.MaxPlayers];

        SceneManager.LoadScene(nextScene);
    }

    private void Awake()
    {
        setInstanceOrDestroySelf();

        //_playerData = new Dictionary<int, PlayerData>();

        // TODO: Add delegates for the UI
        AddListeners();
        // init arrays
        _playerData = new PlayerData[Constants.PlayerLobby.MaxPlayers];
        _players = new PlayerController[Constants.PlayerLobby.MaxPlayers];
    }

    /// <summary>
    /// Tells the GameManager where the spawn points in the scene are 
    /// </summary>
    /// <param name="points"></param>
    public void SetSpawnPoints(Transform[] points)
    {
        _spawnPoints = points;
    }

    /// <summary>
    /// Tells the GameManager where the spawn points in the scene are 
    /// </summary>
    public void SetSpawnPoints(GameObject[] points)
    {
        _spawnPoints = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            _spawnPoints[i] = points[i].transform;
        }
    }

    /// <summary>
    /// Sets the initialization data for a player
    /// </summary>
    public void SetPlayerData(int playerIndex, PlayerData data)
    {
        if (playerIndex >= PlayerData.Length)
        {
            logErrorFormat("Attempt to set data for player {0}, but that index is out of range!", playerIndex);
            return;
        }

        PlayerData[playerIndex] = data;
        updatePlayerCount();
        // TODO: OnPlayerDataChanged delegate
    }

    public void Pause()
    {
        _paused = true;

        if (OnPause != null)
            OnPause();
    }

    public void Unpause()
    {
        _paused = false;

        if (OnUnpause != null)
            OnUnpause();
    }

    /// <summary>
    /// Called whenever player data changes, counts active and non null players
    /// </summary>
    private void updatePlayerCount()
    {
        _playerCount = 0;
        for (int i = 0; i < PlayerData.Length; i++)
        {
            if (PlayerData[i] != null && PlayerData[i].Active)
            {
                _playerCount++;
            }
        }
    }

    /// <summary>
    /// Spawns all the players
    /// </summary>
    public void SpawnAllPlayers()
    {
        for (int i = 0; i < PlayerData.Length; i++)
        {
            if (PlayerData[i] != null && PlayerData[i].Active)
                SpawnPlayer(i);
        }
    }

    public PlayerController SpawnPlayer(int playerNum)
    {
        // check that we have the player data, and the player is active
        if (PlayerData[playerNum] == null || !PlayerData[playerNum].Active)
        {
            logErrorFormat("Attempt to spawn player {0} but data is either null or player is not active!", playerNum);
            return null;
        }

        // check that we have a spawn point for the player
        if (SpawnPoints.Length <= playerNum || SpawnPoints[playerNum] == null)
        {
            logErrorFormat("No spawn point for player {0}!", playerNum);
            return null;
        }

        // check that the player hasn't already been spawned
        if (Players[playerNum] != null)
        {
            logErrorFormat("Player {0} has already been spawned!");
            return Players[playerNum];
        }


        // spawn player at point
        GameObject playerObj = Instantiate<GameObject>(PlayerPrefab, SpawnPoints[playerNum]);
        PlayerController player = playerObj.GetComponent<PlayerController>();

        // initialize player
        player.Initialize(PlayerData[playerNum]);

        // add to active players array
        _players[playerNum] = player;

        return player;
    }

    /// <summary>
    /// Either sets this instance as the singleton,
    /// or destroys itself if there's already an instance
    /// </summary>
    private void setInstanceOrDestroySelf()
    {
        if (_instance != null)
        {
            Debug.LogWarning("[GameManager] There's already an instance of the game manager! Destroying self...");
            DestroyImmediate(this); // intentionally only destroying the component, in case there's any important scripts attached to the object
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void AddListeners()
    {
        //PlayerController.OnPlayerPopped += PlayerPopped;
    }

    private void RemoveListeners()
    {
        //PlayerController.OnPlayerPopped -= _instance.PlayerPopped;
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void PlayerPopped(PlayerController playerPopped, PlayerController otherPlayer)
    {
        //_playerData[pId].Health--;
    }

    #region Logging
    private void logWarningFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogWarningFormat("[GameManager] " + format, args);
#endif
    }

    private void logErrorFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogErrorFormat("[GameManager] " + format, args);
#endif
    }

    private void logFormat(string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogFormat("[GameManager] " + format, args);
#endif
    }
    #endregion

}
