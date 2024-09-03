using UnityEngine;
using System.Collections;

public class CameraTransition : MonoBehaviour
{
    public float transitionDuration = 2f;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // Save the original position and rotation of the camera
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void StartTransition(Transform target)
    {
        StartCoroutine(TransitionToTarget(target));
    }

    private IEnumerator TransitionToTarget(Transform target)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        Vector3 targetPosition = target.position;
        Quaternion targetRotation = target.rotation;

        while (elapsedTime < transitionDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public void ResetToOriginalPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
