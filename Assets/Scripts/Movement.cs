using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

public class Movement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20.0f;
    [SerializeField] private float movementMultiplier = 10.0f;
    [SerializeField] private float airMultiplier = 0.5f;
    [SerializeField] private float gravity = 3.0f;

    [SerializeField] private ClientNetworkTransform nt;

    [Header("Camera")]
    public float sensX = 50.0f;
    public float sensY = 50.0f;
    [SerializeField] Transform cam;
    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    public Quaternion TargetRotation { private set; get; }

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4.0f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float acceleration = 10.0f;
    [SerializeField] float fovChangeSpeed = 0.2f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 27.0f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6.0f;
    [SerializeField] private float airDrag = 2.0f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    float groundDistance = 0.4f;

    [Header("Camera")]
    [SerializeField] private Camera camStats;
    [SerializeField] private Transform head;
    [SerializeField] private float sprintingFOV = 100.0f;
    [SerializeField] private float normalFOV = 90.0f;

    bool isGrounded, mouseIsHidden = true;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    private void Start()
    {
        //if(!IsLocalPlayer) { return; }
        if (IsLocalPlayer)
        {
            nt.CanCommitToTransform = true;
            camStats.gameObject.SetActive(true);
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            TargetRotation = transform.rotation;
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        camStats.transform.localPosition = head.localPosition;

        MyInput();

        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(mouseIsHidden)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                mouseIsHidden = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                mouseIsHidden = true;
            }
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        orientation.transform.localRotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
    }

    private void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        //if (!IsLocalPlayer) { return; }
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            camStats.fieldOfView = Mathf.Lerp(camStats.fieldOfView, sprintingFOV, fovChangeSpeed * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            camStats.fieldOfView = Mathf.Lerp(camStats.fieldOfView, normalFOV, fovChangeSpeed * Time.deltaTime);
        }
    }

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void ResetTargetRotation()
    {
        TargetRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
}

