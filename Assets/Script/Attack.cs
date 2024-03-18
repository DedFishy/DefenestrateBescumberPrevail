using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum ATTACKS {
    Throw
}

public enum ATTACKTYPE {
    Main,
    Secondary,
    Falling,
    Jumping
}

public class Attack : MonoBehaviour
{
    public ATTACKS attack;
    public ATTACKTYPE attackType;

    public string attackAnimation;

    public GameObject attackObject;
    public GameObject attackObjectParent;
    public Vector3 attackObjectParentOffset;
    public Quaternion attackObjectRotation;

    private Animator playerAnimator;

    private Func<bool> attackKey = () => {return false;};

    private playerController player;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        switch (attackType)
        {
            case ATTACKTYPE.Main: 
            attackKey = delegate() {return Input.GetMouseButton(0); };
            break;

            case ATTACKTYPE.Secondary: 
            attackKey = delegate() {return Input.GetMouseButton(1); };
            break;
        }

        player = GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.attachedController != -1 && attackKey.Invoke()) {
            playerAnimator.SetBool("isAttacking", true);
            playerAnimator.SetBool("attack_" + attackAnimation, true);
            if (attackAnimation == "throw" && attackObject.transform.parent == null) {
                Destroy(attackObject.GetComponent<Rigidbody2D>());
                attackObject.transform.parent = attackObjectParent.transform;
                Vector3 offset = attackObjectParentOffset;
                if (player.isFlipped) {
                    offset.x = -offset.x;
                }
                attackObject.transform.position = attackObjectParent.transform.position + offset;
                attackObject.transform.rotation = attackObjectRotation;
            }
            
        } else {
            playerAnimator.SetBool("isAttacking", false);
            playerAnimator.SetBool("attack_" + attackAnimation, false);
        }
    }

    void throwObject() {
        print("Throw that thang");
        
        attackObject.transform.parent = null;
        Rigidbody2D rigidBody = attackObject.AddComponent<Rigidbody2D>();

        bool isReversed = player.isFlipped;

        print(isReversed);

        rigidBody.velocity = isReversed ? new Vector2(10, 1) : new Vector2(-10, 1);
        
    }
}
