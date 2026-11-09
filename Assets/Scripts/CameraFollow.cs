using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer floor;

    private Camera followCamera;

    private void Awake()
    {
        followCamera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Bounds floorBounds = floor.bounds;
        Vector3 playerPosition = player.position;

        float halfCameraHeight = followCamera.orthographicSize;
        float halfCameraWidth = halfCameraHeight * followCamera.aspect;
        float cameraHeight = halfCameraHeight * 2f;
        float cameraWidth = halfCameraWidth * 2f;

        float cameraX;

        if (floorBounds.size.x <= cameraWidth)
        {
            cameraX = floorBounds.center.x;
        }
        else
        {
            float minCameraX = floorBounds.min.x + halfCameraWidth;
            float maxCameraX = floorBounds.max.x - halfCameraWidth;
            cameraX = Mathf.Clamp(playerPosition.x, minCameraX, maxCameraX);
        }

        float cameraY;

        if (floorBounds.size.y <= cameraHeight)
        {
            cameraY = floorBounds.center.y;
        }
        else
        {
            float minCameraY = floorBounds.min.y + halfCameraHeight;
            float maxCameraY = floorBounds.max.y - halfCameraHeight;
            cameraY = Mathf.Clamp(playerPosition.y, minCameraY, maxCameraY);
        }

        float cameraZ = transform.position.z;
        transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }
}
