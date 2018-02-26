using UnityEngine;

/// <summary>
/// In charge of the player collision with other players. 
/// Uses a raycast down from a specified point for a specified
/// length, and checks if we have hit another player
/// </summary>
public class PlayerCollision : MonoBehaviour
{
    /// <summary>
    /// Position in which the raycast starts
    /// </summary>
    public Transform RaycastStart;

    /// <summary>
    /// The length that this raycast will check for if we hit a player
    /// </summary>
    public float Length;

    /// <summary>
    /// A 2D raycast hit object that will store the hit information
    /// </summary>
    private RaycastHit2D _rayHit;

	void Update ()
    {
        // This should be in fixed update, but because the player controller 
        // does the movement in update we need to do it here as well
        checkCollisions();
	}

    /// <summary>
    /// Sends a raycast out from the start to teh length, 
    /// if we hit an enemy then they should pop
    /// </summary>
    private void checkCollisions()
    {
        // Calculate the end of the raycast based on our current rotation
        Vector3 raycastEnd = (-transform.up * Length);

#if UNITY_EDITOR
        Debug.DrawRay(RaycastStart.position, raycastEnd, Color.magenta);
#endif
        // Check for a hit on the raycast
        _rayHit = Physics2D.Raycast(RaycastStart.position, raycastEnd, Length);

        // If we have hit a players balloon
        if(_rayHit.collider != null && _rayHit.collider.CompareTag("Balloon"))
        {
            // Pop the balloon
            Destroy(_rayHit.collider.gameObject.transform.parent.gameObject);
            // TODO: Implement a health system for popping balloons
           
            Debug.Log("HIT : " + _rayHit.collider.name + "Layer: " + _rayHit.collider.gameObject.layer.ToString());
        }
    }
}
