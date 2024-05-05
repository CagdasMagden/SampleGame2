using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleCharacterMovement : MonoBehaviour
{
    //Ctrl + K, Ctrl + C

    // karakterin rigidbody2d de z rotationunu kapad�n unutma bir s�k�nt� olursa onada bak 

    // genel kodlar i�in variablaller
    public float jump = 500f;
    public float maxJump = 4f;
    public bool didntJumpedFirstButCanJumpSecond = true;
    public float speed;
    public float move;
    private bool canDoubleJump = false;
    private bool isTouchingDoor = false;

    // kendi yapt���m sa�a sola d�nme variable
    public float positionChange = 0.125f;
    public bool facingRight = true;
    public bool facingLeft = false;

    // raycast yere de�ip de�meme
    public Vector2 rayCastBoxSize;
    public float rayCastDistance;
    public LayerMask groundLayer;
    bool grounded;

    // dash i�in
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
    //ba�ka classlar
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
    { // karakterin i�ine olay� koymay� unutma!!!!!!!!!!!!!!!!!!!!!!!!!

        isGrounded();

        if (Object.ReferenceEquals(enemies.orc, null))
        {
            // nesne yok edildi, bir �ey yapma
        }
        else if (Alttam�(enemies.orc))
        {
            Destroy(enemies.orc);
        }
        else if (Alttam�(gameObject))
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
            //zaman� gelince trape �arpt��� zaman yukar� sa�a sola nas�l at�l�r ��ren
        }
        if (collision.gameObject.CompareTag("PuzzleBox"))
        {
            speed = 2.5f; // karakter iterken yava�l�yor
            StartCoroutine(SlowDownBox(collision.gameObject.GetComponent<Rigidbody2D>()));
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            transform.position = CharacterStartingPoint;
            healthManager.DamageCharacter();
        }
    }

    //�ok ekstra kod var bi s�ra d�zelt
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PuzzleBox"))
        {
                speed = 5f; // Kutu ile temas kesildi�inde karakterin h�z�n� tekrar ayarla
        }
    }

    private IEnumerator SlowDownBox(Rigidbody2D boxRigidbody)
    {
        float slowdownFactor = 0.9f; // Ayarlanabilir de�er
        float timeToSlowdown = 0.1f;  // Ayarlanabilir de�er

        while (boxRigidbody.velocity.magnitude > 0.1f)
        {
            boxRigidbody.velocity *= slowdownFactor;
            yield return new WaitForSeconds(timeToSlowdown);
        }       
            speed = 5f; // itme bitince karakter eski h�z�na d�n�yor              
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
    // karakterin �zelliklerini belli bir para kar��l��� satan bir vendor olacak ona t�klay�nca sat�n alacaks�n 
    private bool Alttam�(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false; // E�er gameObject null ise false d�nd�r
        }

        else if (gameObject.transform == null)
        {
            return false; // E�er gameObject'in transformu null ise false d�nd�r
        }
        else if (gameObject.transform.position.y <= -30)
        {
            return true;
        }
        return false;
    }
}
