using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject players;
    public float minLimitX;
    public float maxLimitX;
    public float minLimitY;
    public float maxLimitY;

    public float cameraSpeed = 2;
    public float fovChangeFactor = 1;
    public float baseFov = 100;
    public float targetFov;
    private Camera playerCamera;

    private Vector2 targetPosition;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = transform.position;
        playerCamera = GetComponent<Camera>();
        targetFov = playerCamera.fieldOfView;

    }

    // Update is called once per frame
    void Update()
    {
        float minX = 0;
        float minY = 0;
        float maxX = 0;
        float maxY = 0;
        foreach(Transform child in players.transform) {
            if (child.position.x > maxX) maxX = child.position.x;
            if (child.position.x < minX) minX = child.position.x;
            if (child.position.y > maxY) maxY = child.position.y;
            if (child.position.y < minY) minY = child.position.y;

        }
        targetPosition = new Vector2((maxX+minX)/2, (maxY+minY)/2);
        targetFov = ((maxX-minX) + (maxY-minY)) * fovChangeFactor;
        
    }

    void FixedUpdate() {
        transform.position += new Vector3(
            (targetPosition.x - transform.position.x) * cameraSpeed,
            (targetPosition.y - transform.position.y) * cameraSpeed,
            0
        );
        playerCamera.fieldOfView = (targetFov - playerCamera.fieldOfView) * cameraSpeed + baseFov;
    }
}
