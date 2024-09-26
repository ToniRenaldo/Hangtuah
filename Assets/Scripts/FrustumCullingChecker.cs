using UnityEngine;

public class FrustumCullingChecker : MonoBehaviour
{
    private Camera mainCamera;
    private Renderer objectRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (IsObjectInView(mainCamera, objectRenderer))
        {
            Debug.Log(gameObject.name + " is in view frustum");
        }
        else
        {
            Debug.Log(gameObject.name + " is outside view frustum");
        }
    }

    bool IsObjectInView(Camera cam, Renderer rend)
    {
        if (rend == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, rend.bounds);
    }
}