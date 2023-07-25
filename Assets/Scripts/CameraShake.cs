using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.localPosition;
    }
    public IEnumerator Shake(float duration, float magnitude) 
   {

        float elapsed = 0.0f;

        while (elapsed < duration)
        {

            float x = Random.Range(-1 * originalPos.x, 1f * originalPos.x) * magnitude;
            float y = Random.Range(-1f * originalPos.y, 1f * originalPos.y) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            
            yield return null;

        }

        transform.localPosition = originalPos;

   }
}
