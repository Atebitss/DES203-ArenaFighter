 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Menu_Vfx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GameObject.Find("FallingLeaves").GetComponent<ParticleSystem>();

        ps = GameObject.Find("Confetti").GetComponent<ParticleSystem>();

        ps.Play();


    }

    
}
