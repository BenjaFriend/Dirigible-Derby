using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(CanvasGroup))]
public class UIWinTextController : MonoBehaviour
{
    /// <summary>
    /// How long (in seconds) it takes for the text to fade in
    /// </summary>
    public float FadeInLength;

    private CanvasGroup _canvasGroup;
    private Text _text;
    private float _fadeInTime;

    private void Start()
    {
        ((GameSceneController)GameManager.Instance.SceneController).OnGameWon += onGameWonEvent;

        _canvasGroup = GetComponent<CanvasGroup>();
        _text = GetComponent<Text>();

        _canvasGroup.alpha = 0f;
    }

    private void onGameWonEvent(PlayerController winner)
    {
        _text.color = winner.PlayerData.Color;
        _fadeInTime = FadeInLength;
    }

    private void Update()
    {
        if(_fadeInTime > 0)
        {
            _fadeInTime -= Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, (FadeInLength - _fadeInTime) / FadeInLength);
        }
    }
}
