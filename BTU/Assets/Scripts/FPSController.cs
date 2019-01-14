using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    public bool jump;
    public bool isJumping;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isPreviouslyGrounded;
    public bool isInteracting;
    public bool isInteractionZone;

    public List<string> inventory;

    public float maxVertRot = 90f; //Maximum Vertical Rotation;
    public float sensitivity = 2.0f;
    public float walkSpeed = 3.0f;
    public float gravityMultiplier = 2.0f;

#pragma warning disable IDE0044 // Add readonly modifier
    private float speed;
    private float runSpeed;
    private float jumpSpeed = 5.0f;
    private float stickGroundForce;

    public GameObject targetingObject;
    private Vector2 movementInput;
    private Vector3 moveDir;

    private Camera fps_camera;
    private CharacterController cController;
    private CollisionFlags collisionFlags;

    private Quaternion characterTargetRot;
    private Quaternion cameraTargetRot;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        fps_camera = Camera.main;
        cController = GetComponent<CharacterController>();
        isJumping = false;

        characterTargetRot = transform.localRotation;
        cameraTargetRot = fps_camera.transform.localRotation;

        runSpeed = walkSpeed * 2;
    }


    private void Update()
    {
        CheckRotation();
        RayCast();
        if (!jump)
        {
            jump = Input.GetButtonDown("Jump");
        }

        if (!isPreviouslyGrounded && cController.isGrounded)
        {
            moveDir.y = 0f;
            isJumping = false;
        }

        if (!cController.isGrounded && !isJumping && isPreviouslyGrounded)
        {
            moveDir.y = 0f;
        }

        isPreviouslyGrounded = cController.isGrounded;
    }

    private void FixedUpdate()
    {
        float speed;
        CheckInteractionInput();
        CheckMovement(out speed);

        Vector3 desiredMove = transform.forward * movementInput.y + transform.right * movementInput.x;

        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, cController.radius, Vector3.down, out hitInfo, cController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        moveDir.x = desiredMove.x * speed;
        moveDir.z = desiredMove.z * speed;

        if (cController.isGrounded)
        {
            moveDir.y = -stickGroundForce;

            if (jump)
            {
                moveDir.y = jumpSpeed;
                jump = false;
                isJumping = true;
            }
        }
        else
        {
            moveDir += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }
        collisionFlags = cController.Move(moveDir * Time.fixedDeltaTime);
    }

    private void CheckMovement(out float speed)
    {
        isWalking = !Input.GetKey(KeyCode.LeftShift) && moveDir != Vector3.zero;
        isRunning = Input.GetKey(KeyCode.LeftShift) && moveDir != Vector3.zero;
        isIdle = moveDir == Vector3.zero;
        speed = isWalking ? walkSpeed : runSpeed;
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;
        movementInput = new Vector2(horizontal, vertical);

        if (movementInput.sqrMagnitude > 1)
        {
            movementInput.Normalize();
        }
    }

    private void CheckRotation()
    {
        float yRot = Input.GetAxis("Mouse X") * sensitivity;
        float xRot = Input.GetAxis("Mouse Y") * sensitivity;

        characterTargetRot *= Quaternion.Euler(0, yRot, 0);
        cameraTargetRot *= Quaternion.Euler(-xRot, 0, 0);

        cameraTargetRot = ClampRotation(cameraTargetRot);

        transform.localRotation = characterTargetRot;
        fps_camera.transform.localRotation = cameraTargetRot;

    }

    private void GetInput(out float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        speed = isWalking ? walkSpeed : runSpeed;

        movementInput = new Vector2(horizontal, vertical);

        if (movementInput.sqrMagnitude > 1)
        {
            movementInput.Normalize();
        }

        //TODO CycleBob thingy.
    }

    Quaternion ClampRotation(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -maxVertRot, maxVertRot);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }

    private void CheckInteractionInput()
    {
        isInteracting = Input.GetKeyDown(KeyCode.Mouse0);
    }

    private void RayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(fps_camera.gameObject.transform.position, fps_camera.gameObject.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(fps_camera.gameObject.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            targetingObject = hit.transform.gameObject;
            if (isInteracting && targetingObject.GetComponent<Block>() != null)
            {

                if(!targetingObject.GetComponent<Block>().isMissing)
                {
                targetingObject.GetComponent<Block>().Trigger();
                }
                else
                {
                    if(inventory.Contains(targetingObject.name))
                    {
                        targetingObject.GetComponent<Renderer>().enabled = true;
                        GameObject.Find("Main_Door").GetComponent<MainDoor>().open = true;
                    }
                }
                
            }
            if (isInteracting && targetingObject.GetComponent<Door>() != null)
            {
                targetingObject.GetComponent<Door>().Trigger();
            }
        }
        else
        {
            Debug.DrawRay(fps_camera.gameObject.transform.position, fps_camera.gameObject.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            targetingObject = null;
            //Debug.Log("Did not Hit");
        }
        
    }
    
}