using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Collectable : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public SpriteRenderer spriteRenderer;
    public Light2D light;

    public void PickUp()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }

        if (light != null)
        {
            light.enabled = false;
        }

        spriteRenderer.enabled = false;

        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
}
