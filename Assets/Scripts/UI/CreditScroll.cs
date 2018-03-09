using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{

    public GameObject Credits;
    public float Speed;

    private const int _FINISH = 1350;

    private float actualSpeed;

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // scale speed by deltaTime
        actualSpeed = Speed * (60 * Time.deltaTime);

        // get Credit's current y position
        var posY = Credits.GetComponent<RectTransform>().anchoredPosition.y;

        // scroll until the defined height
        // TODO: Runtime grab defined height from all credit items?
        if (posY < _FINISH)
            Credits.GetComponent<RectTransform>().Translate(0, actualSpeed, 0);

        // Back to Main Menu
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
