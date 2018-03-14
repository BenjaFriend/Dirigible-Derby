using UnityEngine;

/// <summary>
/// Quick and dirty utility class for placing and finding spawn points in the scene
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    public Vector3 Position
    {
        get { return transform.position; }
    }
}
