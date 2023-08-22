using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXCleanup : MonoBehaviour
{
    [SerializeField] private float duration = 10;
    private float timer;

    private void Start()
    {
        StartCoroutine(Timer());
    }
   
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
