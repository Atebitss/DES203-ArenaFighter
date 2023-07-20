using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxAnimator : MonoBehaviour
{
    public Animator animator;
    public Image[] boxes;

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
            print(i);
            yield return new WaitForSeconds(3);
        }
    }
}
