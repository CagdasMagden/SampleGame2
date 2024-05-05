using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxPuzzleMechanism : MonoBehaviour
{
    public GameObject doorsToOpen;
    public Sprite sprite1;
    public Sprite sprite2;
    public bool isItOpen = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PuzzlePlace"))
        {
            isItOpen = true;
            doorsToOpen.GetComponent<SpriteRenderer>().sprite = sprite2;
            Debug.Log("puzzle has been completed");
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PuzzlePlace"))
        {
            isItOpen = false;
            doorsToOpen.GetComponent<SpriteRenderer>().sprite = sprite1;
            Debug.Log("puzzle hasnt been completed");
        }
    }// kodu yarýn iyileþtir
    // bir objeye scripti atamadan 2 object arasý nasýl halledilir öðren
    
}
