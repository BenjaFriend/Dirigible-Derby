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

    public PlayerData[] PlayerData;

    private void Awake()
    {
        setInstanceOrDestroySelf();
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
