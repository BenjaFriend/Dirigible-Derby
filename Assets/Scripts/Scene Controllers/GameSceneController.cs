using UnityEngine;
using System.Collections;

public class GameSceneController : SceneController
{
    public override void OnControllerActivated()
    {
        // find spawn points and tell gm
        GameManager.Instance.SetSpawnPoints(GameObject.FindGameObjectsWithTag(Constants.Tags.SpawnPoint));

        // spawn players
        GameManager.Instance.SpawnAllPlayers();
    }

    public override void OnControllerDeactivated()
    {
    }
}
