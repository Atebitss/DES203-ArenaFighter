using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxAnimator : MonoBehaviour
{
    public Animator animator;
    public Image[] boxes;
    private float waitTime = 3;

    public void AnimateBoxes()
    {
        StartCoroutine(AnimateBoxesPrivate());
    }
    private IEnumerator AnimateBoxesPrivate()
    {

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].gameObject.SetActive(true);
            animator.SetTrigger("trigger" + i);
            if (PlayerData.devMode)
            {
                //print(i);
                yield return new WaitForSeconds(1);
            }
            else { yield return new WaitForSeconds(waitTime); }
        }
    }

    public void SetWaitTime(float newTime)
    {
        waitTime = newTime;
        StartCoroutine(AnimateBoxesPrivate());
    }
}
