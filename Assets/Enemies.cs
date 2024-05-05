using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour

{
    // karakterlerin baþka objelerle çarpýþmamalarý lazým masslarý 100
    public GameObject orc;
    private Rigidbody2D rb;
    //private Rigidbody2D rbOrc;

    public float speed = 1f, timeLeftToChange = 2f;
    public bool goingRight = true;
    
    void Start()
    {
        //rbOrc = orc.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        //rbOrc.Sleep();
    }

    // Update is called once per frame
    void Update()
    {
        if (orc != null)
        {
            if (goingRight)
                orc.transform.Translate(Vector2.right * speed * Time.deltaTime);
            else
                orc.transform.Translate(Vector2.left * speed * Time.deltaTime);

            timeLeftToChange -= Time.deltaTime;
            if (timeLeftToChange <= 0)
            {
                orc.transform.localScale = new Vector2(orc.transform.localScale.x * -1, orc.transform.localScale.y);
                //orc.transform.position = new Vector2(transform.position.x - 0.25f, transform.position.y);
                goingRight = !goingRight;
                timeLeftToChange = 5f;
            }
        }
        
    }
}
