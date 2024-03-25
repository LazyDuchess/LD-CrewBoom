using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCameraController : MonoBehaviour
{
    public float Sensitivity = 2f;
    public float ZoomSensitivity = 1f;
    public float Height = 0.5f;
    public float Yaw = 45f;
    public float Pitch = 45f;
    public float Distance = 1f;
    private CharacterPreviewController _characterPreviewController;
    private void Start()
    {
        _characterPreviewController = CharacterPreviewController.Instance;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(Pitch, Yaw, 0f);
        transform.position = (-transform.forward * Distance) + (Vector3.up * Height);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Yaw += Input.GetAxisRaw("Mouse X") * Sensitivity;
            Pitch -= Input.GetAxisRaw("Mouse Y") * Sensitivity;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Distance += Input.GetAxisRaw("Mouse Y") * ZoomSensitivity;
        }

        Pitch = Mathf.Clamp(Pitch, -90f, 90f);
        Distance = Mathf.Clamp(Distance, 0f, 10f);
    }
}
