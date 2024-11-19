using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnim;
    public Transform camTrans;
    public CharacterController playerChar;
    private float currentSpeed;
    public float walk_speed, run_speed, ro_speed;
    public bool walking;
    public Transform playerTrans;
    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = camTrans.forward * z + camTrans.right * x;
            move.y = 0;
            //playerTrans.rotation = Quaternion.Slerp(playerTrans.rotation, Quaternion.LookRotation(move), ro_speed * Time.deltaTime);

            playerChar.Move(move * currentSpeed * Time.deltaTime);
        }
    }
    

    void Update()
    {
        Move();
    }

    public void Move()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            playerAnim.SetBool("walk", true);
            walking = true;
        } else {
            playerAnim.SetBool("walk", false);
            walking = false;
        }
        if (walking) {
            if (Input.GetKey(KeyCode.LeftShift)) {
                playerAnim.SetBool("run", true);
                currentSpeed = run_speed;
            } else {
                playerAnim.SetBool("run", false);
                currentSpeed = walk_speed;
            }
        }
    }
}
