using UnityEngine;
using UnityEngine.UI;

public class ObjectFollower : MonoBehaviour
{
    public Transform target3DObject; // Reference to the 3D object to follow
    public RectTransform canvasRectTransform; // Reference to the RectTransform of the overlay canvas
    public Vector3 offset; // Offset between the 3D object and the canvas object

    public Camera mainCamera; // Reference to the main camera

    void Start()
    {
        // Find the main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }
    }

    void Update()
    {
        if (target3DObject == null || canvasRectTransform == null || mainCamera == null)
            return;

        // Convert the 3D object's position to screen space
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target3DObject.position + offset);

        // Set the position of the canvas object to the screen position
        canvasRectTransform.position = screenPos;
    }
}
