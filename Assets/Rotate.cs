using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Speed")]
    [Tooltip("The speed at which the object rotates.")]
    public float rotationSpeed = 10.0f;

    [Header("Rotation Axis")]
    [Tooltip("The axis around which the object rotates.")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
