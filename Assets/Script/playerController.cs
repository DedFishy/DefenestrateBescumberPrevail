using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;

public class playerController : MonoBehaviour
{

    private const float KOBoundBottom = -20.0f;
    private const float KOBoundTop = 20.0f;
    private const float KOBoundLeft = -20.0f;
    private const float KOBoundRight = 20.0f;

    private const int maxJumps = 1;
    private int jumpsLeft;


    private Rigidbody2D playerRigidbody;
    public float moveSpeed = 1.0F;
    public float jumpSpeed = 1.0F;

    private GameObject mainCamera;

    private GameObject KOEffectParticleObject;
    private GameObject KOSFXObject;
    private AudioSource KOSFX;

    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRenderer;
    private bool hasSpawnedKO = false;

    public bool isFlipped = false;
    public int attachedController;
    public TMP_Text damagePercentDisplay;
    private int damagePercent = 0;
    private Vector2 movementVector = new Vector2();
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        KOEffectParticleObject = GameObject.Find("KO Effect Particles");
        KOSFXObject = GameObject.Find("KO SFX");
        KOSFX = KOSFXObject.GetComponent<AudioSource>();
        mainCamera = GameObject.Find("Main Camera");
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    bool isOutOfKOBounds()
    {
        return (
            transform.position.y < KOBoundBottom ||
            transform.position.y > KOBoundTop ||
            transform.position.x > KOBoundRight ||
            transform.position.x < KOBoundLeft
        );
    }

    void FixedUpdate() {
        float movementX = movementVector.x != 0 ? movementVector.x : playerRigidbody.velocity.x;
        float movementY = movementVector.y != 0 ? movementVector.y : playerRigidbody.velocity.y;

        playerRigidbody.velocity = new Vector2(movementX, movementY);
        movementVector.y = 0;
        movementVector.x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOutOfKOBounds()) { 
            if (!hasSpawnedKO) {
                GameObject KOEffectParticleObjectLocal = Instantiate(KOEffectParticleObject);
                KOEffectParticleObjectLocal.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    -3
                    );
                Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -3);
                KOEffectParticleObjectLocal.transform.up = targetPosition - KOEffectParticleObjectLocal.transform.position;
                //KOEffectParticleObjectLocal.transform.Rotate(new Vector3(0, 0, -90));
                ParticleSystem KOParticles = KOEffectParticleObjectLocal.GetComponent<ParticleSystem>();
                KOParticles.Play();
                KOSFX.Play();
                hasSpawnedKO = true;
            }
            Destroy(gameObject);
        }

        bool isRunning = false;

        if (attachedController != -1) {

            if (Input.GetKey("a")) {
                movementVector.x = -moveSpeed;
                isRunning = !isRunning;
                setSpriteFlipped(false);

            }
            if (Input.GetKey("d")) {
                movementVector.x = moveSpeed;
                isRunning = !isRunning;
                setSpriteFlipped(true);
            }
            if (Input.GetKeyDown("w") && jumpsLeft > 0) {
                movementVector.y = jumpSpeed;
                jumpsLeft--;
            }
        }

        playerAnimator.SetBool("isRunning", isRunning);

        double upwardVelocity = Math.Round(playerRigidbody.velocity.y);
        playerAnimator.SetBool("isGoingUp", upwardVelocity > 0);
        playerAnimator.SetBool("isGoingDown", upwardVelocity < 0);

        
    }

    void setSpriteFlipped(bool flipped) {
        if (isFlipped == flipped) return;
        print("setting!");
        if (flipped) {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            damagePercentDisplay.transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            damagePercentDisplay.transform.localScale = new Vector3(1, 1, 1);
            
        }
        isFlipped = flipped;

    }

    public void takeDamage(Vector2 velocity, Vector2 impact) {
        
        damagePercent += (int) velocity.magnitude;
        damagePercentDisplay.text = damagePercent.ToString() + "%";
        playerRigidbody.AddForce(impact * damagePercent);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor" || col.gameObject.tag == "Player") {
            if (col.transform.position.y > transform.position.y || col.transform.position.y < transform.position.y) {
                jumpsLeft = maxJumps;
            }
        }
    }
}
