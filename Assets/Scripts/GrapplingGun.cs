using UnityEngine;

/// <summary>
/// GrapplingGun allows the player to grapple onto objects in the game world,
/// providing momentum and a rope-like effect while grappling.
/// </summary>
public class GrapplingGun : MonoBehaviour
{
    // Components
    [Header("Components")]

    private LineRenderer lr; // Line renderer for visualizing the grapple rope
    private Vector3 grapplePoint; // The point where the grapple connects
    public LayerMask whatIsGrappleable; // Layer mask to determine what can be grappled
    public Transform gunTip; // The position from which the grapple is fired
    public new Transform camera; // Reference to the camera
    public Transform player; // Reference to the player
    private float maxDistance = 100f; // Maximum distance for the grapple
    private SpringJoint joint; // The spring joint used for grappling
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody

    [Header("Grappling Settings")]
    // Grappling settings
    [Tooltip("Force applied while grappling.")]
    public float grappleForce = 10f; // Force applied while grappling
    [Tooltip("Maximum speed while grappling.")]
    public float maxGrappleSpeed = 20f; // Maximum speed while grappling

    void Awake()
    {
        // Initialize components
        lr = GetComponent<LineRenderer>(); // Get the LineRenderer component
        playerRigidbody = player.GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    void Update()
    {
        // Start grappling on left mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        // Stop grappling on left mouse button up
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    // Called after Update
    void LateUpdate()
    {
        DrawRope(); // Draw the rope visual
    }

    /// <summary>
    /// Call whenever we want to start a grapple.
    /// </summary>
    void StartGrapple()
    {
        RaycastHit hit;
        // Check if we hit a grappleable object
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point; // Set the grapple point
            joint = player.gameObject.AddComponent<SpringJoint>(); // Create a spring joint
            joint.autoConfigureConnectedAnchor = false; // Do not auto-configure the anchor
            joint.connectedAnchor = grapplePoint; // Set the grapple point as the anchor

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint); // Calculate distance from the grapple point

            // Set joint distances
            joint.maxDistance = distanceFromPoint * 0.8f; // Maximum distance the grapple can stretch
            joint.minDistance = distanceFromPoint * 0.25f; // Minimum distance the grapple can stretch

            // Adjust spring joint settings
            joint.spring = 4.5f; // Spring strength
            joint.damper = 7f; // Damping effect
            joint.massScale = 4.5f; // Mass scale for the spring joint

            lr.positionCount = 2; // Set the line renderer position count
            currentGrapplePosition = gunTip.position; // Initialize current grapple position
        }
    }

    /// <summary>
    /// Call whenever we want to stop a grapple.
    /// </summary>
    void StopGrapple()
    {
        lr.positionCount = 0; // Reset line renderer position count
        Destroy(joint); // Destroy the spring joint
    }

    private Vector3 currentGrapplePosition; // Current position of the grapple point

    /// <summary>
    /// Draws the rope visual for the grapple.
    /// </summary>
    void DrawRope()
    {
        // If not grappling, don't draw the rope
        if (!joint) return;

        // Smoothly transition to the grapple point
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        // Set the line renderer positions
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

        // Apply momentum
        ApplyMomentum();
    }

    /// <summary>
    /// Applies momentum to the player while grappling.
    /// </summary>
    void ApplyMomentum()
    {
        // Calculate the direction to the grapple point
        Vector3 direction = (grapplePoint - player.position).normalized;

        // Apply force in the grapple direction if under max speed
        if (playerRigidbody.velocity.magnitude < maxGrappleSpeed)
        {
            playerRigidbody.AddForce(direction * grappleForce); // Apply force
        }
    }

    /// <summary>
    /// Checks if the player is currently grappling.
    /// </summary>
    /// <returns>True if grappling, false otherwise.</returns>
    public bool IsGrappling()
    {
        return joint != null; // Return if the joint is active
    }

    /// <summary>
    /// Gets the current grapple point.
    /// </summary>
    /// <returns>The current grapple point.</returns>
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint; // Return the grapple point
    }
}
