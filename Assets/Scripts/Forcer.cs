using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies a force every frame this component is active
/// </summary>
public class Forcer : MonoBehaviour
{
    /// <summary>
    /// The Rigidbody to apply the force to. 
    /// If null on Start, the forcer will attempt to use an attached body
    /// </summary>
    public Rigidbody2D Rigidbody;

    /// <summary>
    /// The force to be applied next frame
    /// </summary>
    public Vector2 Force;

    /// <summary>
    /// Whether or not the force is relative to this Transform's up
    /// </summary>
    public bool UseRelativeForce;

    private void Start()
    {
        if(Rigidbody == null)
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }

    private void Update()
    {
        Vector2 appliedForce = Force;
        if (UseRelativeForce)
            appliedForce = transform.TransformVector(Force);

        Rigidbody.AddForceAtPosition(appliedForce, transform.position);
        Debug.DrawLine(transform.position, (Vector2)transform.position + appliedForce, Color.red);
    }
}
