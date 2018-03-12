using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{

    public GameObject Credits;
    public float Speed;

    private float finish;
    private float actualSpeed;

    // Use this for initialization
    void Start()
    {
        var lastChild = Credits.transform.GetChild(Credits.transform.childCount - 1);
        var childPosY = lastChild.position.y;
        var childHeight = lastChild.GetComponent<RectTransform>().rect.height;
        var canvasHeight = Credits.transform.parent.GetComponent<RectTransform>().rect.height;

        finish = Math.Abs(childPosY) + childHeight + canvasHeight;
    }

    // Update is called once per frame
    void Update()
    {
        // scale speed by deltaTime
        actualSpeed = Speed * (60 * Time.deltaTime);

        // get Credit's current y position
        var posY = Credits.GetComponent<RectTransform>().anchoredPosition.y;

        // scroll until the defined height
        if (posY < finish)
            Credits.GetComponent<RectTransform>().Translate(0, actualSpeed, 0);

        // Back to Main Menu
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
