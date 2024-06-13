using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorWindow : MonoBehaviour
{
    [SerializeField] private Animator animator = null;


    public void ChangeState()
    {
        animator.SetBool("Exit", !animator.GetBool("Exit"));
    }
    public void DisableWindow()
    {
        animator.gameObject.SetActive(false);
    }

}
