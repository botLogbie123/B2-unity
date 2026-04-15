using Unity.Mathematics;
using UnityEngine;

public class FlyingController : MonoBehaviour
{
    public float moveSpeed;
    public float maxFloatHeight = 10;
    public float minFloatHeight;

    public Camera PlayerFollowCamera;
    private float currentHeight;
    private Animator anim; // todo

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHeight = transform.position.y;
        // anim = GetComponent<Animator>();
    
        Cursor.lockState= CursorLockMode.Locked;
    }


    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float verticle = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.W)) 
        {

            MoveCharacter();
       
        }
        else 
        {

            DisableMovement();
        
        }

        currentHeight = Mathf.Clamp(transform.position.y, currentHeight, maxFloatHeight);
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    }

    private void MoveCharacter() 
    {
        Vector3 cameraForward = new Vector3(PlayerFollowCamera.transform.forward.x, 0, PlayerFollowCamera .transform.forward.z);
        transform.rotation = Quaternion.LookRotation(cameraForward);
        transform.Rotate(new Vector3(0,0,0), Space.Self);

        // anim.SetBool("isFlying", true);

        Vector3 forward = PlayerFollowCamera.transform.forward;
        Vector3 flyDirection = forward.normalized;

        currentHeight += flyDirection.y * moveSpeed * Time.deltaTime;
        currentHeight = Mathf.Clamp(currentHeight, minFloatHeight, maxFloatHeight);

        transform.position += flyDirection * moveSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
    
    }

    private void DisableMovement()
    {
        // anim.SetBool("isflying", false);
        transform.rotation = quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    
    
    }

} 
