using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
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
            animator.SetFloat("X", x); animator.SetFloat("Y", y);
            SetState(true);
        }

        public void SetState(bool state)
        {
            animator.SetBool("isWalking", state);
        }
    }
}
