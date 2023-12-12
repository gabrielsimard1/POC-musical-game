using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    Animator animator;
    [SerializeField] BoxCollider2D swordCollider;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void OnSwordSlash()
    {
        animator.SetBool("isSwordSlashing", true);
    }

    void SetSwordSlashingFalse()
    {
        animator.SetBool("isSwordSlashing", false);
    }
}
