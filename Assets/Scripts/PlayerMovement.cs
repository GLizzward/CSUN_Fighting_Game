using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    float horizontalInput;
    float moveSpeed = 5f;
    float jumpPower = 7f;
    bool isGrounded = false;
    bool isDashing = false;

    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    private float dashTime;

    public GameObject punchHitbox;
    public GameObject punch2Hitbox; // New Punch2 hitbox
    public GameObject kickHitbox;
    public GameObject aerialHitbox;
    public GameObject aerial2Hitbox; // Aerial2 hitbox
    private Animator animator;
    private Rigidbody2D rb;
    private CharacterSoundFX soundFX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        soundFX = GetComponent<CharacterSoundFX>();

        // Find child objects
        if (punchHitbox == null)
        {
            punchHitbox = transform.Find("PunchHitbox")?.gameObject;
            if (punchHitbox == null) Debug.LogError("PunchHitbox not found as child object.");
        }

        if (punch2Hitbox == null) // Initialize Punch2 hitbox
        {
            punch2Hitbox = transform.Find("Punch2Hitbox")?.gameObject;
            if (punch2Hitbox == null) Debug.LogError("Punch2Hitbox not found as child object.");
        }

        if (aerialHitbox == null)
        {
            aerialHitbox = transform.Find("AerialHitbox")?.gameObject;
            if (aerialHitbox == null) Debug.LogError("AerialHitbox not found as child object.");
        }

        if (aerial2Hitbox == null)
        {
            aerial2Hitbox = transform.Find("Aerial2Hitbox")?.gameObject;
            if (aerial2Hitbox == null) Debug.LogError("Aerial2Hitbox not found as child object.");
        }

        if (kickHitbox == null)
        {
            kickHitbox = transform.Find("KickHitbox")?.gameObject;
            if (kickHitbox == null) Debug.LogError("KickHitbox not found as child object.");
        }

        // Deactivate hitboxes initially
        if (punchHitbox != null) punchHitbox.SetActive(false);
        if (punch2Hitbox != null) punch2Hitbox.SetActive(false);
        if (aerialHitbox != null) aerialHitbox.SetActive(false);
        if (aerial2Hitbox != null) aerial2Hitbox.SetActive(false);
        if (kickHitbox != null) kickHitbox.SetActive(false);
    }

    void Update()
    {
        if (!isDashing)
        {
            horizontalInput = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                isGrounded = false;
                animator.SetBool("isJumping", !isGrounded);

                if (soundFX != null) soundFX.PlaySound("Jump");
            }

            if (Input.GetKeyDown(KeyCode.K) && !isGrounded)
            {
                Debug.Log("Aerial Attack Triggered");
                PlayAerialAttack();
            }

            if (Input.GetKeyDown(KeyCode.J) && !isGrounded) // Aerial2 Attack
            {
                Debug.Log("Aerial2 Attack Triggered");
                PlayAerialAttack2();
            }

            if (Input.GetKeyDown(KeyCode.K) && isGrounded)
            {
                PlayKick();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                PlayPunch();
            }

            if (Input.GetKeyDown(KeyCode.I)) // New Punch2 input
            {
                PlayPunch2();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartDash();
            }
        }

        if (isDashing)
        {
            DashUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
            animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
            animator.SetFloat("yVelocity", rb.velocity.y);
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTime = dashDuration;

        float dashDirection = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);
        animator.SetTrigger("Dash");

        if (soundFX != null) soundFX.PlaySound("Dash");
    }

    private void DashUpdate()
    {
        dashTime -= Time.deltaTime;

        if (dashTime <= 0)
        {
            isDashing = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void PlayKick()
    {
        animator.SetTrigger("Kick");

        if (kickHitbox != null)
        {
            StartCoroutine(ActivateHitboxAtEndOfAnimation(kickHitbox));
        }

        if (soundFX != null) soundFX.PlaySound("Kick");
    }

    private void PlayPunch()
    {
        animator.SetTrigger("Punch");

        if (punchHitbox != null)
        {
            StartCoroutine(ActivateHitboxAtEndOfAnimation(punchHitbox));
        }

        if (soundFX != null) soundFX.PlaySound("Punch");
    }

    private void PlayPunch2() // New Punch2 attack
    {
        animator.SetTrigger("Punch2");

        if (punch2Hitbox != null)
        {
            StartCoroutine(ActivateHitboxAtEndOfAnimation(punch2Hitbox));
        }

        if (soundFX != null) soundFX.PlaySound("Punch2"); // Add "Punch2" sound to your sound manager
    }

    private void PlayAerialAttack()
    {
        animator.SetTrigger("AerialAttack");

        if (aerialHitbox != null)
        {
            StartCoroutine(ActivateHitboxAtEndOfAnimation(aerialHitbox));
        }

        if (soundFX != null) soundFX.PlaySound("AerialAttack");
    }

    private void PlayAerialAttack2()
    {
        animator.SetTrigger("AerialAttack2");

        if (aerial2Hitbox != null)
        {
            StartCoroutine(ActivateHitboxAtEndOfAnimation(aerial2Hitbox));
        }

        if (soundFX != null) soundFX.PlaySound("AerialAttack2");
    }

    private IEnumerator ActivateHitboxAtEndOfAnimation(GameObject hitbox)
    {
        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationDuration * 0.2f);
        hitbox.SetActive(true);
        StartCoroutine(DeactivateHitbox(hitbox));
    }

    private IEnumerator DeactivateHitbox(GameObject hitbox)
    {
        yield return new WaitForSeconds(0.5f);
        hitbox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }
}