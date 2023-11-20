using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;            // The position that camera will be following
    [SerializeField] private float smoothing = 5f;        // The speed with which the camera will be following

    private Vector3 offset;                     // The initial offset from the target

    void Start()
    {
        // Calculate the initial offset
        offset = transform.position - target.position;
    }
    
    void FixedUpdate()
    {
        // Create a position the camera is aiming for based on the offset from the target
        Vector3 targetCamPos = target.position + offset;

        // Smoothly interpolate between the camera's current position and it's target position
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }
}
