using System;
using System.Collections;
using System.Collections.Generic;
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
    private bool hasSpawnedKO = false;
    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        KOEffectParticleObject = GameObject.Find("KO Effect Particles");
        KOSFXObject = GameObject.Find("KO SFX");
        KOSFX = KOSFXObject.GetComponent<AudioSource>();
        mainCamera = GameObject.Find("Main Camera");
        playerAnimator = GetComponent<Animator>();
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

    // Update is called once per frame
    void Update()
    {
        if (isOutOfKOBounds()) { 
            if (!hasSpawnedKO) {
                GameObject KOEffectParticleObjectLocal = UnityEngine.Object.Instantiate(KOEffectParticleObject);
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
        }

        bool isRunning = false;

        if (Input.GetKey("a")) {
            playerRigidbody.AddForce(Vector3.left * moveSpeed);
            isRunning = !isRunning;
        }
        if (Input.GetKey("d")) {
            playerRigidbody.AddForce(Vector3.right * moveSpeed);
            isRunning = !isRunning;
        }
        if (Input.GetKeyDown("w") && jumpsLeft > 0) {
            playerRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode2D.Impulse);
            jumpsLeft--;
        }

        playerAnimator.SetBool("isRunning", isRunning);

        double upwardVelocity = Math.Round(playerRigidbody.velocity.y);
        playerAnimator.SetBool("isGoingUp", upwardVelocity > 0);
        playerAnimator.SetBool("isGoingDown", upwardVelocity < 0);

        
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Floor") {
            jumpsLeft = maxJumps;
        }
    }
}
