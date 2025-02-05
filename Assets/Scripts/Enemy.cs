using TMPro;  // Include the TextMeshPro namespace
using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
    public int health = 10;
    private Animator animator;
    private Rigidbody2D rb;

    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;
    public float stunDuration = 0.5f;
    private bool isKnockedBack = false;
    private bool isStunned = false;

    public EnemyHealthBar healthBar; // Reference to the EnemyHealthBar
    public TMP_Text hitCountText;  // Reference to TextMeshPro object
    private int hitCount = 0;      // Counter for the number of hits

    // Audio
    public AudioClip hitSound;  // Assign in Inspector
    private AudioSource audioSource;

    private bool isTakingDamage = false;  // Flag to prevent multiple knockbacks stacking

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // Initialize health bar and hit count text
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(health);
        }

        if (hitCountText != null)
        {
            hitCountText.text = "COMBO 0";
        }
        else
        {
            Debug.LogError("Hit Count Text is not assigned in the inspector.");
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        health -= damage;
        hitCount++;  // Increment hit count for each hit

        Debug.Log("Enemy took damage! Health: " + health);

        // Update the health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(health);
        }

        // Update hit count text
        if (hitCountText != null)
        {
            hitCountText.text = "COMBO " + hitCount.ToString();
        }

        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Always apply stun (can be triggered multiple times during the hit)
        if (!isStunned)
        {
            StartCoroutine(ApplyHitStun());
        }

        // Apply knockback after hit
        if (!isKnockedBack)
        {
            StartCoroutine(ApplyKnockback(knockbackDirection));
        }

        // If health reaches 0, destroy enemy
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 knockbackDirection = Vector2.zero;

        if (collision.CompareTag("PunchHitbox"))
        {
            // Pull enemy closer (horizontal knockback toward player)
            knockbackDirection = (collision.transform.position - transform.position).normalized;
            knockbackDirection.y = 0; // Horizontal only
            TakeDamage(5, knockbackDirection);
        }
        else if (collision.CompareTag("AerialHitbox"))
        {
            // Upward knockback
            knockbackDirection = Vector2.up;
            TakeDamage(10, knockbackDirection);
        }
        else if (collision.CompareTag("KickHitbox"))
        {
            // Vertical knockback
            knockbackDirection = Vector2.up; // Straight upward
            TakeDamage(7, knockbackDirection);
        }
        else if (collision.CompareTag("Aerial2Hitbox"))
        {
            // Diagonal upward knockback
            knockbackDirection = new Vector2(1f, 1f).normalized;
            TakeDamage(8, knockbackDirection);
        }
        else if (collision.CompareTag("Punch2Hitbox"))
        {
            // Diagonal upward knockback
            knockbackDirection = new Vector2(1f, 1f).normalized;
            TakeDamage(8, knockbackDirection);
        }
    }

    private IEnumerator ApplyHitStun()
    {
        isStunned = true;

        if (animator != null)
        {
            animator.SetTrigger("Was Hit");
            Debug.Log("Hit stun triggered!");
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;  // Stop movement during stun
        }

        yield return new WaitForSeconds(stunDuration);

        isStunned = false; // Reset stun flag to allow further hits
        Debug.Log("Hit stun ended.");
    }

    private IEnumerator ApplyKnockback(Vector2 knockbackDirection)
    {
        isKnockedBack = true;

        if (rb != null)
        {
            rb.velocity = knockbackDirection * knockbackForce;
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Stop knockback movement
        }

        isKnockedBack = false;
    }
}