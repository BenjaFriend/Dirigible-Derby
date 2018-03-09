using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{

    public GameObject Credits;
    public float Speed;

    private const int _FINISH = 1350;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var posY = Credits.GetComponent<RectTransform>().anchoredPosition.y;

        if (posY < _FINISH)
            Credits.GetComponent<RectTransform>().Translate(0, Speed, 0);
    }
}
