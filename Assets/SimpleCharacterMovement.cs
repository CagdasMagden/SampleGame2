using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleCharacterMovement : MonoBehaviour
{
    //Ctrl + K, Ctrl + C

    // karakterin rigidbody2d de z rotationunu kapadýn unutma bir sýkýntý olursa onada bak 

    // genel kodlar için variablaller
    public float jump = 500f;
    public float maxJump = 4f;
    public bool didntJumpedFirstButCanJumpSecond = true;
    public float speed;
    public float move;
    private bool canDoubleJump = false;
    private bool isTouchingDoor = false;

    // kendi yaptýðým saða sola dönme variable
    public float positionChange = 0.125f;
    public bool facingRight = true;
    public bool facingLeft = false;

    // raycast yere deðip deðmeme
    public Vector2 rayCastBoxSize;
    public float rayCastDistance;
    public LayerMask groundLayer;
    bool grounded;

    // dash için
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 10f;
    private float dashingTime = 0.3f;
    private float dashingCD = 1f;
    private float originalGravitiy;
    [SerializeField] private TrailRenderer dashTR;

    // Teleport
    private GameObject currentTeleporter;
    //

    private Rigidbody2D rb;
    private Vector2 CharacterStartingPoint;
    //baþka classlar
    public CoinManager coinManager;
    public HealthManager healthManager;
    public PopUpManager popUpManager;
    public BoxPuzzleMechanism boxPuzzleMechanism;
    public Enemies enemies;
    public Npc npc;

    void Start()
    {
        //rbMove = GetComponent<Rigidbody2D>();
        //rbJump = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        CharacterStartingPoint = transform.position;        
    }
    void Update()
    { // karakterin içine olayý koymayý unutma!!!!!!!!!!!!!!!!!!!!!!!!!

        isGrounded();

        if (Object.ReferenceEquals(enemies.orc, null))
        {
            // nesne yok edildi, bir þey yapma
        }
        else if (Alttamý(enemies.orc))
        {
            Destroy(enemies.orc);
        }
        else if (Alttamý(gameObject))
        {
            rb.velocity = new Vector2(0f, 0f);
            transform.position = CharacterStartingPoint;
            healthManager.CurrentHealth--;
        }

        if (isDashing)
        {
            return;
        }

        move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (move > 0 && facingLeft == true)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            transform.position = new Vector2(transform.position.x + positionChange, transform.position.y);
            facingRight = true;
            facingLeft = false;
        }
        else if (move < 0 && facingRight == true)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            transform.position = new Vector2(transform.position.x - positionChange, transform.position.y);
            facingLeft = true;
            facingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTouchingDoor && boxPuzzleMechanism.isItOpen)
            {
                SceneManager.LoadSceneAsync(0);
            }
            if (currentTeleporter != null)
            {
                transform.position = currentTeleporter.GetComponent<Teleporter>().GetDestination().position;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded())
            {
                Jump();
                didntJumpedFirstButCanJumpSecond = false;
                canDoubleJump = true;
            }
            else if (canDoubleJump || didntJumpedFirstButCanJumpSecond)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                Jump();
                canDoubleJump = false;
                didntJumpedFirstButCanJumpSecond = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private void Jump()
    {
        rb.AddForce(new Vector2(rb.velocity.x, jump));
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        originalGravitiy = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        dashTR.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        dashTR.emitting = false;
        rb.gravityScale = originalGravitiy;
        isDashing = false;
        yield return new WaitForSeconds(dashingCD);
        canDash = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            coinManager.coinCount++;
        }
        else if (other.gameObject.CompareTag("Star"))
        {
            Destroy(other.gameObject);
            coinManager.coinCount += 5;
        }
        else if (other.gameObject.CompareTag("Potions"))
        {
            if (healthManager.CurrentHealth < healthManager.MaxHealth)
            {
                Destroy(other.gameObject);
                healthManager.CurrentHealth++;
            }
            else if (healthManager.CurrentHealth >= healthManager.MaxHealth)
            {
                popUpManager.annen();
            }
        }

        if (other.gameObject.CompareTag("Doors"))
        {
            isTouchingDoor = true;
        }

        if (other.gameObject.CompareTag("Teleporter"))
        {
            currentTeleporter = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Doors"))
        {
            isTouchingDoor = false;
        }

        if (other.gameObject.CompareTag("Teleporter"))
        {
            if (other.gameObject == currentTeleporter)
            {
                currentTeleporter = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            transform.position = CharacterStartingPoint;
            healthManager.DamageCharacter();

            //didntJumpedFirstButCanJumpSecond = true;            
            //rb.AddForce(new Vector2(rb.velocity.x, 400));
            //zamaný gelince trape çarptýðý zaman yukarý saða sola nasýl atýlýr öðren
        }
        if (collision.gameObject.CompareTag("PuzzleBox"))
        {
            speed = 2.5f; // karakter iterken yavaþlýyor
            StartCoroutine(SlowDownBox(collision.gameObject.GetComponent<Rigidbody2D>()));
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            transform.position = CharacterStartingPoint;
            healthManager.DamageCharacter();
        }
    }

    //çok ekstra kod var bi sýra düzelt
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PuzzleBox"))
        {
                speed = 5f; // Kutu ile temas kesildiðinde karakterin hýzýný tekrar ayarla
        }
    }

    private IEnumerator SlowDownBox(Rigidbody2D boxRigidbody)
    {
        float slowdownFactor = 0.9f; // Ayarlanabilir deðer
        float timeToSlowdown = 0.1f;  // Ayarlanabilir deðer

        while (boxRigidbody.velocity.magnitude > 0.1f)
        {
            boxRigidbody.velocity *= slowdownFactor;
            yield return new WaitForSeconds(timeToSlowdown);
        }       
            speed = 5f; // itme bitince karakter eski hýzýna dönüyor              
    }
    
    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, rayCastBoxSize, 0, -transform.up, rayCastDistance, groundLayer))
        {
            didntJumpedFirstButCanJumpSecond = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * rayCastDistance, rayCastBoxSize);
    }
    //void OnCollisionEnter2D(Collision2D other)
    //{
    //    isInAir = false;
    //}
    //private bool isInAir;
    // karakterin özelliklerini belli bir para karþýlýðý satan bir vendor olacak ona týklayýnca satýn alacaksýn 
    private bool Alttamý(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false; // Eðer gameObject null ise false döndür
        }

        else if (gameObject.transform == null)
        {
            return false; // Eðer gameObject'in transformu null ise false döndür
        }
        else if (gameObject.transform.position.y <= -30)
        {
            return true;
        }
        return false;
    }
}
