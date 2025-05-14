using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Movement")]
    public float speed = 3f;
    public float dashSpeed = 6f;
    private bool isDashing;
    public float dashTime = 0.3f;
    private float dashCounter;

    [Header("Jump")]
    public float jumpForce = 8f;
    public int maxJumpCount = 2;
    private int jumpCount;

    [Header("Attack")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireCooldown = 0.2f;
    private float fireTimer;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Vector2 groundCheckOffset;
    public Vector2 groundCheckSize;

    [Header("Health")]
    public int hp = 3;
    public float knockbackForce = 3f;

    [Header("Camera Limits")]
    public float leftLimit = -11f;
    public float rightLimit = 11f;

    private Vector2 respawnPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetRespawnPositionByScene();
    }

    private void Update()
    {
        HandleMove();
        HandleJump();
        HandleAttack();
        HandleDash();

        ClampPositionToCameraLimits();
    }

    private void HandleMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * speed, rb.velocity.y);

        if (h != 0)
        {
            sr.flipX = h < 0;
        }
    }

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            isDashing = true;
            speed = dashSpeed;
            dashCounter = dashTime;
        }

        if (isDashing)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0f)
            {
                isDashing = false;
                speed = 3f;
            }
        }
    }

    private void HandleJump()
    {
        if (IsGrounded())
        {
            jumpCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    private void HandleAttack()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && fireTimer <= 0f)
        {
            Vector2 spawnPos = transform.position + (sr.flipX ? Vector3.left : Vector3.right);
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = new Vector2((sr.flipX ? -1 : 1) * bulletSpeed, 0f);

            fireTimer = fireCooldown;
        }
    }

    private bool IsGrounded()
    {
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapBox(checkPos, groundCheckSize, 0f, groundLayer);
    }

    private void ClampPositionToCameraLimits()
    {
        float clampedX = Mathf.Clamp(transform.position.x, leftLimit, rightLimit);
        transform.position = new Vector2(clampedX, transform.position.y);
    }

    private void SetRespawnPositionByScene()
    {
        string scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "Stage0":
                respawnPos = new Vector2(-9f, -1.5f);
                break;
            case "Stage1":
                respawnPos = new Vector2(-9f, -4.5f);
                break;
            case "Stage2":
                respawnPos = new Vector2(-9f, -6.5f);
                break;
            default:
                respawnPos = transform.position;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyBullet"))
        {
            hp--;

            float direction = transform.position.x < collision.transform.position.x ? -1f : 1f;
            rb.velocity = new Vector2(direction * knockbackForce, rb.velocity.y);

            if (hp <= 0)
            {
                Respawn();
            }
        }
    }

    private void Respawn()
    {
        transform.position = respawnPos;
        hp = 3;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 checkPos = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(checkPos, groundCheckSize);
    }
}
