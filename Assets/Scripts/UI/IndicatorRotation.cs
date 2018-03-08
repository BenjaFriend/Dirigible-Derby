using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorRotation : MonoBehaviour {
    
    /// <summary>
    /// Indicator Canvas
    /// </summary>
    public GameObject canvas;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        canvas.transform.rotation = Quaternion.Euler(Vector3.zero);
	}
}
