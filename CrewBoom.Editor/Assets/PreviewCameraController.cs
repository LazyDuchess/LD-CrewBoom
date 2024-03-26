using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewCameraController : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float Sensitivity = 2f;
    public float ZoomSensitivity = 1f;
    public Vector3 Pivot = new Vector3(0f, 0.5f, 0f);
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
        var moveSpeed = MoveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed *= 2f;

        transform.rotation = Quaternion.Euler(Pitch, Yaw, 0f);
        transform.position = (-transform.forward * Distance) + Pivot;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Yaw += Input.GetAxisRaw("Mouse X") * Sensitivity;
                Pitch -= Input.GetAxisRaw("Mouse Y") * Sensitivity;
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                Distance += Input.GetAxisRaw("Mouse Y") * ZoomSensitivity;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            Pivot += transform.right * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Pivot -= transform.right * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.W))
        {
            Pivot += transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Pivot -= transform.forward * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            Pivot -= Vector3.up * moveSpeed * Time.unscaledDeltaTime;
        }

        if (Input.GetKey(KeyCode.E))
        {
            Pivot += Vector3.up * moveSpeed * Time.unscaledDeltaTime;
        }

        var pivotLimit = 5f;
        Pivot = new Vector3(Mathf.Clamp(Pivot.x, -pivotLimit, pivotLimit), Mathf.Clamp(Pivot.y, -pivotLimit, pivotLimit), Mathf.Clamp(Pivot.z, -pivotLimit, pivotLimit));

        Pitch = Mathf.Clamp(Pitch, -90f, 90f);
        Distance = Mathf.Clamp(Distance, 0f, 10f);
    }
}
