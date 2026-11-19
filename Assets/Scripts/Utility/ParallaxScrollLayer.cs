using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public sealed class ParallaxScrollLayer : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField, Range(0f, 1f)]
    [Tooltip("0 follows the camera fully. 1 keeps the layer fixed in the world.")]
    private float ratio = 0f;

    private Vector2 initialCameraPosition;
    private Vector2 initialLayerPosition;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        initialCameraPosition = targetCamera.transform.position;
        initialLayerPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
            return;

        Vector2 cameraMovement = (Vector2)targetCamera.transform.position - initialCameraPosition;
        Vector2 layerPosition = initialLayerPosition + cameraMovement * (1f - ratio);
        transform.position = new Vector3(layerPosition.x, layerPosition.y, transform.position.z);
    }
}
