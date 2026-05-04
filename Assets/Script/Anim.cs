using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Anim : MonoBehaviour
{
    public Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        //Ejemplo
        //animator = GameObject.FindGameObjectWithTag("Dragon").GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        animator.enabled = true;
        animator.Play("Blue");
    }

    public void StopAnim() {
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
