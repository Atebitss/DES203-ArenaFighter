using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackTrigger : MonoBehaviour
{
    private BoxCollider2D[] cols = new BoxCollider2D[10];
    private string triggeringCollider;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(this.transform.parent + " attack triggered by " + collision.gameObject.transform.name);
        triggeringCollider = collision.gameObject.transform.name;

        /*if (this.gameObject.transform.parent.name == "Player0")
        {
            Debug.Log(this.transform.parent + " hits " + collision);
            Debug.Log(cols.Length);
        }*/

        //for each collider in array,
        //find one that is empty,
        //then provide it the current collider & increase max collisions & end for loop 
        if (collision.gameObject.GetComponent<BoxCollider2D>() != null)
        {
            for (int colIndex = 0; colIndex < cols.Length; colIndex++)
            {
                if (cols[colIndex] == null)
                {
                    cols[colIndex] = collision.gameObject.GetComponent<BoxCollider2D>();

                    /*if (this.gameObject.transform.parent.name == "Player0")
                    {
                        Debug.Log(this.gameObject.transform.parent.name + " added " + cols[colIndex].gameObject.name + " to pos " + colIndex);
                    }*/

                    colIndex = cols.Length;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log(this.transform.parent + " leaves " + collision.gameObject.tag);

        //for each collider in array,
        //find the current collider,
        //then remove it
        for (int colIndex = 0; colIndex < cols.Length; colIndex++)
        {
            //Debug.Log("col index " + colIndex + ": " + cols[colIndex]);
            if (cols[colIndex] == collision)
            {
                /*if (this.gameObject.transform.parent.name == "Player0")
                {
                    Debug.Log(this.gameObject.transform.parent.name + " removed " + cols[colIndex].gameObject.name + " from pos " + colIndex);
                }*/
                cols[colIndex] = null;
            }
        }
    }



    public BoxCollider2D[] GetColliders()
    {
        return cols;
    }

    public BoxCollider2D GetEnemyBackCollider()
    {
        for (int colIndex = 0; colIndex <= cols.Length; colIndex++)
        {
            if (cols[colIndex].tag == "PlayerBack")
            {
                return cols[colIndex];
            }
        }

        return null;
    }

    public string GetTriggeringCollider()
    {
        return triggeringCollider;
    }
}
