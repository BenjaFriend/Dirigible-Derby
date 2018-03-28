using UnityEngine;

/// <summary>
/// Contains the functionality of the main menu such as loading the game
/// and quitting.
/// </summary>
public class MenuScript : MonoBehaviour
{
    /// <summary>
    /// A button that is to be selected by default.
    /// </summary>
    public UnityEngine.UI.Button FirstSelectButton;

    /// <summary>
    /// Select the first button so that the input system knows
    /// what to do
    /// </summary>
    private void Start()
    {
        if(FirstSelectButton != null)
        {
            FirstSelectButton.Select();
        }        
    }

    /// <summary>
    /// Load the main game scene
    /// </summary>
    public void LoadGame()
    {
        loadScene(Constants.Scenes.PlayerLobby);
    }

    public void LoadCredits()
    {
        loadScene(Constants.Scenes.Credits);
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

    /// <summary>
        /// Loads the scene using SceneManager. Don't forget the scene 
        /// must be added to build settings
        /// </summary>
        /// <param name="sceneName">The name of hte scene to load</param>
    private void loadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
