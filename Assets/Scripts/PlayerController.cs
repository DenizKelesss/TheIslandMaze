using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 7f;
    private float rotationSpeed = 300f; // Speed of camera rotation
    private float jumpForce = 3f;
    private bool isGrounded = true; // Check if the player is on the ground
    private bool canJump = true; // Check if the player can jump

    private Rigidbody playerRB;
    private Camera mainCamera;

    // Variables to limit vertical camera rotation
    private float verticalRotation = 0f;
    private float maxVerticalAngle = 60f; // Limit for vertical camera rotation

    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        mainCamera = Camera.main; // Get the main camera

        // Lock rotation on the X and Z axes to keep the player upright
        playerRB.freezeRotation = true;
    }

    void Update()
    {
        // Handle camera rotation
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    // Simple player movement
    void PlayerMovement()
    {
        // Get input direction relative to camera
        Vector3 inputDirection = GetInputDirection();

        // Move the player in the input direction
        playerRB.velocity = new Vector3(inputDirection.x * speed, playerRB.velocity.y, inputDirection.z * speed);
    }

    // Rotate the camera and player based on mouse input
    void RotateCamera()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Horizontal rotation (rotate the player)
        transform.Rotate(0, mouseX, 0);

        // Vertical rotation (camera pitch)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle); // Limit vertical rotation

        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // Get input direction relative to camera rotation
    Vector3 GetInputDirection()
    {
        // Get camera forward direction without vertical component
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f;
        cameraForward = cameraForward.normalized;

        // Get camera right direction without vertical component
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight = cameraRight.normalized;

        // Get input direction based on horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = (cameraForward * verticalInput) + (cameraRight * horizontalInput);

        // Normalize the input direction to ensure consistent speed in all directions
        if (inputDirection.magnitude > 1f)
            inputDirection.Normalize();

        return inputDirection;
    }

    // Jump function
    void Jump()
    {
        playerRB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Player is now in the air
        canJump = false; // Prevent double jumping
    }

    // Check if player is grounded
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is grounded
            canJump = true; // Reset double jump prevention
        }
    }
}
