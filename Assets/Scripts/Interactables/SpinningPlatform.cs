using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPlatform : MonoBehaviour
{
    [SerializeField] private int speed;



    void Update()
    {
        transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));

        if ((transform.eulerAngles.z >= -30 && transform.eulerAngles.z <= 30) || (transform.eulerAngles.z >=150  && transform.eulerAngles.z <= -150))
        {
            gameObject.layer = LayerMask.NameToLayer("Wall");
            //print("Wall");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Ground");
        }        
    }
}
