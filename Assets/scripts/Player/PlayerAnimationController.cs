using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : NetworkBehaviour
    {
        Animator animator;


        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetValues(float x, float y)
        {
            if (x == 0f && y == 0f)
            {
                SetState(false);
                return;

            }
            HandleAnimationServerRpc(x, y);
            SetState(true);
        }

        [ServerRpc]
        private void HandleAnimationServerRpc(float x, float y)
        {

            animator.SetFloat("X", x); animator.SetFloat("Y", y);
        }

        [ServerRpc]
        private void SetStateServerRpc(bool state)
        {
            animator.SetBool("isWalking", state);
        }

        public void SetState(bool state)
        {
            SetStateServerRpc(state);
        }
    }
}
