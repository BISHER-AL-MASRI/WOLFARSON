using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform player;        // Reference to the player
    public float sensitivity = 100f; // Sensitivity for mouse movement

    private float mouseX, mouseY;

    void Update()
    {
        transform.position = player.position;

        // Get mouse movement input
        mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);  // sens = x, -90 > x < 90

        transform.rotation = Quaternion.Euler(mouseY, mouseX, 0f);
    }
}
