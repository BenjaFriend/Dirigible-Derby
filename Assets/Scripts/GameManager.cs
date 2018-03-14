using System.Collections.Generic;
using UnityEngine;

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

    //public PlayerData[] PlayerData;

    private Dictionary<int, PlayerData> _playerData;

    public Dictionary<int, PlayerData> PlayerData;

    private void Awake()
    {
        setInstanceOrDestroySelf();

        _playerData = new Dictionary<int, PlayerData>();

        // TODO: Add delegates for the UI
        AddListeners();
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

    private void AddListeners()
    {
        PlayerController.OnPlayerPopped += PlayerPopped;
    }

    private void RemoveListeners()
    {
        PlayerController.OnPlayerPopped -= PlayerPopped;
    }

    private void OnDisable()
    {
        RemoveListeners();
    }

    private void PlayerPopped(int pId)
    {
        _playerData[pId].Health--;
    }
}
