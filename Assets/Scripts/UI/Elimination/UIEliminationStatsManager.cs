using UnityEngine;
using UnityEngine.UI;

public class UIEliminationStatsManager : MonoBehaviour
{
    /// <summary>
    /// How much space should there be between stats objects?
    /// </summary>
    public float Spacing;
    private GameObject rootStatsObject; // base object from which other UIEliminationPlayerStats objects are created

    public void Initialize()
    {
        if(!GameManager.Instance.SceneHasController
            || GameManager.Instance.SceneController.GetType() != typeof(GameSceneController)
            || ((GameSceneController)GameManager.Instance.SceneController).GamemodeType != GamemodeType.Elimination)
        {
            Debug.LogError("[UIEliminationStatsManager] Elimination stats manager in scene without elimination game controller!");
        }

        rootStatsObject = GetComponentInChildren<UIEliminationPlayerStats>().gameObject;
        initializeStatsObjects();
    }


    private void initializeStatsObjects()
    {
        float playerCount = GameManager.Instance.PlayerCount;
        Vector2 startPos = new Vector2(
            (playerCount - 1) * Spacing / -2f, 
            rootStatsObject.GetComponent<RectTransform>().anchoredPosition.y);

        rootStatsObject.GetComponent<RectTransform>().anchoredPosition = startPos;
        rootStatsObject.GetComponent<UIEliminationPlayerStats>().Initialize(GameManager.Instance.Players[0]);

        for (int i = 1; i < playerCount; i++)
        {
            GameObject statsObj = Instantiate(rootStatsObject, this.transform);
            statsObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(i * Spacing, 0);
            statsObj.GetComponent<UIEliminationPlayerStats>().Initialize(GameManager.Instance.Players[i]);
        }
    }
}
