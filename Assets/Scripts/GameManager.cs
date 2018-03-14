using UnityEngine;
using System.Collections.Generic;

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

    /// <summary>
    /// Per-player data and settings, like ReWired player index, balloon sprites, and color
    /// </summary>
    public PlayerData[] PlayerData;

    public GameObject PlayerPrefab;

    private SpawnPoint[] _spawnPoints;

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
            Debug.LogWarning("[GameManager] Attempt to deactivate scene controller, but there already isn't one!");
        }

        _sceneController.OnControllerDeactivated();
        _sceneController = null;
    }
    #endregion SceneController

    private void Awake()
    {
        setInstanceOrDestroySelf();
    }

    /// <summary>
    /// Tells the GameManager where the SpawnPoints in the scene are 
    /// </summary>
    /// <param name="points"></param>
    public void SetSpawnPoints(SpawnPoint[] points)
    {
        _spawnPoints = points;
    }

    /// <summary>
    /// Spawns all the players
    /// </summary>
    public void SpawnAllPlayers()
    {
        // error if spawnpoints not set or empty
        // spawn all players
    }

    public void SpawnPlayer(int playerNum)
    {
        // spawn player at point
    }

    /// <summary>
    /// Either sets this instance as the singleton,
    /// or destroys itself if there's already an instance
    /// </summary>
    private void setInstanceOrDestroySelf()
    {
        if(_instance != null)
        {
            Debug.LogWarning("[GameManager] There's already an instance of the game manager! Destroying self...");
            DestroyImmediate(this); // intentionally only destroying the component, in case there's any important scripts attached to the object
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }
}
