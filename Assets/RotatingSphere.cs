using UnityEngine;

public class RotatingSphere : MonoBehaviour
{
    public float rotationSpeed = 10f; // Rotation speed

    void Update()
    {
        // Rotate the sphere around its Y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
