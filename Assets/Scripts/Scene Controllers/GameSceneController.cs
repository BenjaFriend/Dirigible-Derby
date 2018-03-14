using UnityEngine;
using System.Collections;

public class GameSceneController : SceneController
{
    public GamemodeType GamemodeType;
    public Gamemode Gamemode;

    private bool _gameOver;
    public bool IsGameOver
    {
        get { return _gameOver; }
    }
    
    public override void OnControllerActivated()
    {
        // find spawn points and tell gm
        GameManager.Instance.SetSpawnPoints(GameObject.FindGameObjectsWithTag(Constants.Tags.SpawnPoint));

        // spawn players
        GameManager.Instance.SpawnAllPlayers();

        // initialize gamemode
        Gamemode = Gamemode.Create(GamemodeType, this);
        Gamemode.Initialize();
    }

    private void Update()
    {
        // update gamemode
        Gamemode.Update();
    }

    public override void OnControllerDeactivated()
    {
        // unload gamemode
        Gamemode.Unload();
    }

    public void GameOver()
    {
        _gameOver = true;
        Debug.Log("Game over!");
        //TODO: OnGameOver delegate
    }
}
