using UnityEngine;

public class MainMenuController : SceneController
{
    /// <summary>
    /// A button that is to be selected by default.
    /// </summary>
    public UnityEngine.UI.Button FirstSelectButton;

    public override void OnControllerActivated()
    {
        if(FirstSelectButton != null)
        {
            // Select the first button so that the input system knows what to do
            FirstSelectButton.Select();
        }        
    }

    /// <summary>
    /// Load the main game scene
    /// </summary>
    public void LoadGame()
    {
        GameManager.Instance.ChangeScene(Constants.Scenes.PlayerLobby);
    }

    public void LoadCredits()
    {
        GameManager.Instance.ChangeScene(Constants.Scenes.Credits);
    }

    /// <summary>
    /// Calls Application.quit
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    public override void OnControllerDeactivated()
    {
    }
}