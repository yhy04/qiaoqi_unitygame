using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;       // ƽ���ٶ�
    public float zoomSpeed = 500f;      // �����ٶ�
    public float minZoom = 40f;          // ������С����
    public float maxZoom = 100f;         // ����������

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // WASDƽ�ƿ���
        float horizontal = Input.GetAxis("Horizontal");  // A��D
        float vertical = Input.GetAxis("Vertical");      // W��S
        Vector3 direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

        // ��������
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (cam.orthographic)
        {
            // �����������
            float newSize = cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime;
            cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
        else
        {
            // ͸���������
            float newFov = cam.fieldOfView - scroll * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(newFov, minZoom, maxZoom);
        }
    }
}
