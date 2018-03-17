using UnityEngine;
using UnityEngine.UI;

public class UIEliminationPlayerStats : MonoBehaviour
{
    public Text Name;
    public Image Background;
    public Image[] Hearts;

    public float HeartActiveAlpha;
    public float HeartInactiveAlpha;

    private EliminationGamemode _gamemode;
    private PlayerController _player;

    public void Initialize(PlayerController player)
    {
        _player = player;

        // get gamemode and register event handler
        GameSceneController gameController = (GameSceneController)GameManager.Instance.SceneController;
        _gamemode = (EliminationGamemode)gameController.Gamemode;
        _gamemode.OnPlayerHealthChanged += onPlayerHealthChangedEvent;

        // set colors according to player colors
        setGraphicColorPreserveAlpha(Name, player.PlayerData.Color);
        setGraphicColorPreserveAlpha(Background, player.PlayerData.Color);

        // initialize hearts
        updateHeartsGraphic();

        // set player name text
        Name.text = player.PlayerData.Name;
    }

    private void onPlayerHealthChangedEvent(PlayerController player)
    {
        updateHeartsGraphic();
    }

    private void updateHeartsGraphic()
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            float alpha = i < _gamemode.PlayerLives[_player.PlayerData.ID] ? HeartActiveAlpha : HeartInactiveAlpha;
            setGraphicAlpha(Hearts[i], alpha);
        }
    }

    private void setGraphicAlpha(Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void setGraphicColorPreserveAlpha(Graphic graphic, Color color)
    {
        color.a = graphic.color.a;
        graphic.color = color;
    }

}