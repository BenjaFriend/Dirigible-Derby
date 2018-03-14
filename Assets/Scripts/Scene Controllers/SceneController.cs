using UnityEngine;

/// <summary>
/// Works with the GameManager to control the behavior of the current scene
/// </summary>
public abstract class SceneController : MonoBehaviour
{
    /// <summary>
    /// Tells the GameManager that this is the current controller
    /// </summary>
    protected void Start()
    {
        GameManager.Instance.SetActiveSceneController(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance.SceneController == this)
            GameManager.Instance.DeactivateCurrentController();
    }

    /// <summary>
    /// Called by the GameManager when this controller becomes the active controller.
    /// Use this for initialization.
    /// </summary>
    public abstract void OnControllerActivated();

    /// <summary>
    /// Called by the GameManager when this controller is replaced as active controller.
    /// Use this for cleanup.
    /// </summary>
    public abstract void OnControllerDeactivated();
}
