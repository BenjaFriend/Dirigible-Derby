using UnityEngine;

/// <summary>
/// Move this element in a sin wave pattern based on a given amplitude nad magnitude
/// </summary>
public class SineWaveUI : MonoBehaviour
{
    [SerializeField]
    private float _magnitude = 5f;

    [SerializeField]
    private float _amlpitude = 10f;

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y += Mathf.Sin(Time.time * _amlpitude) * Time.deltaTime * _magnitude;
        transform.position = pos;
    }
}
