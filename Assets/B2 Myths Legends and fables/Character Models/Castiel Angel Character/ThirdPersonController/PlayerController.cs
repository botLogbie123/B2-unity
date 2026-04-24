using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{
    public Animator playerAnim;
    public Rigidbody playerRigid;
    public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed;
    public bool walking;
    public Transform playerTrans;


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRigid.linearVelocity = transform.forward * w_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRigid.linearVelocity = -transform.forward * w_speed * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
      
    {
       if (Input.GetKeyDown(KeyCode.W)) 
       {
            playerAnim.SetTrigger("Walk");
            playerAnim.ResetTrigger("Idle");
            walking = true;
            //steps1.SetActive(true);
       }
        if (Input.GetKeyUp(KeyCode.W)) 
        {
            playerAnim.ResetTrigger("Walk");
            playerAnim.SetTrigger("Idle");
            walking = false;
            //steps1.SetActive(false);
        } 
        if (Input.GetKeyDown(KeyCode.S)) 
        {
            playerAnim.SetTrigger("Walkback");
            playerAnim.ResetTrigger("Idle");
            //Steps1.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.S)) 
        {
            playerAnim.ResetTrigger("Walkback");
            playerAnim.SetTrigger("Idle");
            //steps1.SetActive(false);
        }

        if (Input.GetKey(KeyCode.A)) 
        { 
            playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D)) 
        { 
            playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }
        if (walking == true) 
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) 
            {
                //steps1.SetActive(false);
                //steps2.SetActive(true);
                w_speed = olw_speed;
                playerAnim.ResetTrigger("run");
                playerAnim.SetTrigger("Walk");
            }
        }
    }   
}
