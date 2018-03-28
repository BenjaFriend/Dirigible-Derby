using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public EventSystem EventSystem;
    public Button ResumeButton, QuitButton;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);

        // start listening for join game pressed for all players
        for (int i = 0; i < Rewired.ReInput.players.playerCount; i++)
        {
            Rewired.ReInput.players.GetPlayer(i).AddInputEventDelegate(
                onStartPressedEvent,
                Rewired.UpdateLoopType.Update,
                Rewired.InputActionEventType.ButtonJustPressed,
                Constants.RewiredInputActions.JoinGame // joingame is start lol
                );
        }

        GameManager.Instance.OnUnpause += deactivate;
        GameManager.Instance.OnSceneChange += onSceneChange;
    }

    public void OnQuitButtonPressed()
    {
        GameManager.Instance.Unpause();

        // maybe we should have a confirmation screen?
        GameManager.Instance.ChangeScene(Constants.Scenes.PlayerLobby);
    }

    public void OnResumeButtonPressed()
    {
        GameManager.Instance.Unpause();
    }

    public void onStartPressedEvent(Rewired.InputActionEventData data)
    {
        if (GameManager.Instance.IsPaused) // unpause
        {
            GameManager.Instance.Unpause();
        }
        else // pause
        {
            activate();
        }
    }

    private void activate()
    {
        gameObject.SetActive(true);
        GameManager.Instance.Pause();
        EventSystem.SetSelectedGameObject(ResumeButton.gameObject);
    }

    private void deactivate()
    {
        gameObject.SetActive(false);
    }        

    private void onSceneChange()
    {
        // stop listening for join game pressed for all players
        for (int i = 0; i < Rewired.ReInput.players.playerCount; i++)
        {
            Rewired.ReInput.players.GetPlayer(i).RemoveInputEventDelegate(onStartPressedEvent);
        }

        GameManager.Instance.OnUnpause -= deactivate;
        GameManager.Instance.OnSceneChange -= onSceneChange;
    }
}
