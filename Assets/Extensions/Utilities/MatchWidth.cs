using NaughtyAttributes;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour
{

    // Set this to the in-world distance between the left & right edges of your scene.
    [ShowIf("IsOrg")] public float sceneWidth = 10;
    [HideIf("IsOrg")] public float horizontalFoV = 90.0f;
    Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        Recalculate();
    }

    public bool IsOrg()
    {
        if (!_camera)
            _camera = GetComponent<Camera>();
        return _camera.orthographic;
    }
    public void Recalculate()
    {
        if (!_camera)
            _camera = GetComponent<Camera>();
        if (_camera.orthographic)
        {
            float unitsPerPixel = sceneWidth / Screen.width;

            float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

            _camera.orthographicSize = desiredHalfHeight;
        }
        else
        {

            float halfWidth = Mathf.Tan(0.5f * horizontalFoV * Mathf.Deg2Rad);

            float halfHeight = halfWidth * Screen.height / Screen.width;

            float verticalFoV = 2.0f * Mathf.Atan(halfHeight) * Mathf.Rad2Deg;

            _camera.fieldOfView = verticalFoV;
        }
    }

    // Adjust the camera's height so the desired scene width fits in view
    // even if the screen/window size changes dynamically.
    void Update()
    {
#if UNITY_EDITOR
        Recalculate();
#endif
    }
}