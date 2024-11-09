using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;       // 平移速度
    public float zoomSpeed = 500f;      // 缩放速度
    public float minZoom = 40f;          // 缩放最小距离
    public float maxZoom = 100f;         // 缩放最大距离

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // WASD平移控制
        float horizontal = Input.GetAxis("Horizontal");  // A和D
        float vertical = Input.GetAxis("Vertical");      // W和S
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // 滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (cam.orthographic)
        {
            // 正交相机缩放
            float newSize = cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            // 透视相机缩放
            float newFov = cam.fieldOfView - scroll * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(newFov, minZoom, maxZoom);
        }
    }
}
