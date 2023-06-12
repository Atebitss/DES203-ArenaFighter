using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

   

    public void Bounce()
    {
        animator.SetTrigger("Bouncing");
        //print("should be bouncing");
    }
}


