using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 65f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        yaw = transform.eulerAngles.y;
        pitch = cameraPivot.localEulerAngles.x;

        if (cameraPivot == null) { Debug.LogError("[PlayerLook] cameraPivot not assigned.", this); }
        if (playerCamera == null) { Debug.LogError("[PlayerLook] playerCamera not assigned.", this); }
    }

    private void Update()
    {
        if (cameraPivot == null) { return; }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX;
        pitch -= mouseY;
        pitch = ClampAngle(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180f) { angle -= 360; }
        return Mathf.Clamp(angle, min, max);
    }
}
