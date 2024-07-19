using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float rotationSpeed = 5f;
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float smoothness = 10f;
    public float minYAngle = 10f;
    public float maxYAngle = 80f;

    private float currentZoom;
    private Vector3 previousPosition;
    private Vector3 targetRotationEuler;
    private float targetZoom;

    void Start()
    {
        currentZoom = Vector3.Distance(transform.position, target.position);
        targetZoom = currentZoom;
        targetRotationEuler = transform.eulerAngles;
    }

    void LateUpdate()
    {
        HandleRotation();
        HandleZoom();
        UpdateCameraTransform();
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - previousPosition;
            targetRotationEuler.y += delta.x * rotationSpeed * Time.deltaTime; // Changed minus to plus
            targetRotationEuler.x -= delta.y * rotationSpeed * Time.deltaTime; // Changed plus to minus
            targetRotationEuler.x = Mathf.Clamp(targetRotationEuler.x, minYAngle, maxYAngle);
            previousPosition = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    void UpdateCameraTransform()
    {
        // Smoothly interpolate rotation
        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothness);

        // Smoothly interpolate zoom
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * smoothness);

        // Update camera position
        Vector3 targetPosition = target.position - transform.forward * currentZoom;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);
    }
}