using UnityEngine;

public class FreeCameraController : MonoBehaviour
{
    public float slowSpeed = 100f;
    public float fastSpeed = 250f;
    public float mouseSensitivity = 200f;

    private bool runLock = false;
    private bool heightLock = false;

    private void Update()
    {
        // Mouse look rotation
        Vector3 lookRotation = new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0f) * mouseSensitivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + lookRotation);

        // Movement
        if (Input.GetKey(KeyCode.CapsLock))
        {
            runLock = !runLock;
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            heightLock = !heightLock;
        }
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 movement = moveInput * Time.deltaTime * (runLock || Input.GetKey(KeyCode.LeftShift) ? fastSpeed : slowSpeed);
        if (heightLock)
        {
            Vector3 worldMovement = transform.TransformDirection(movement);
            float worldSpeed = worldMovement.magnitude;
            worldMovement.y = 0f;
            transform.Translate(worldMovement.normalized * worldSpeed, Space.World);
        }
        else
        {
            transform.Translate(movement);
        }
    }
}
