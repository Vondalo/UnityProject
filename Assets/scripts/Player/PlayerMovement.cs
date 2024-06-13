using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.IO;

using System;
using TMPro;

namespace Player
{
    public class PlayerMovement : Unity.Netcode.NetworkBehaviour
    {
        [SerializeField]
        private float speed;
        [SerializeField]
        private Rigidbody2D rb;
        private float speedX;
        private float speedY;
        private NPC_Controller npc;
        PlayerAnimationController animationController;


        [Header("Player Z Axis")]
        [SerializeField] private float zAxis;


        private void Awake()
        {
            animationController = GetComponent<PlayerAnimationController>();
        }




        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                enabled = false;
                return;
            }
            // get the maincamera
            Camera mainCamera = Camera.main;
            Camera_Controller controller = mainCamera.GetComponent<Camera_Controller>();
            controller.CustomStart(transform);



            transform.position = new Vector3(transform.position.x, transform.position.y, zAxis);
        }


        void Update()
        {
            
            rb.velocity = new Vector2(0, 0);
            if (!inDialogue())
            {
                if (npc != null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        npc.ActivateDialogue(this.gameObject);
                    }
                }
                speedX = Input.GetAxisRaw("Horizontal");
                speedY = Input.GetAxisRaw("Vertical");
                if (inDialogue())
                {
                    speedX = 0;
                    speedY = 0;
                }
                rb.velocity = new Vector2(speedX * speed, speedY * speed);

               
                if(speedX != 0 || speedY != 0)
                {
                    animationController.SetValues(speedX, speedY);
                }
                else
                {
                    animationController.SetState(false);
                }
                

                //if (speedY != 0)
                //{
                //    rb.velocity = new Vector2(0, speedY * speed);
                //}
                //else if (speedX != 0)
                //{
                //    rb.velocity = new Vector2(speedX * speed, 0);
                //}
            }


        }

        private bool inDialogue()
        {
            if (npc != null)
            {
                return npc.dialogueActive();
            }
            else return false;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("in");
           
            switch (collision.gameObject.tag)
            {
                  case "NPC":
                    npc = collision.gameObject.GetComponent<NPC_Controller>();
                    break;
                case "DOOR":
                    npc = collision.gameObject.GetComponent<NPC_Controller>();
                    break;
            }
            
        }



        private void OnTriggerExit2D(Collider2D collision)
        {
            npc = null;
        }
    }
}

