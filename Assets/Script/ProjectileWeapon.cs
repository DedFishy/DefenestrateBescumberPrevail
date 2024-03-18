using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    public int damageFactor;
    public int knockbackFactor;
    public Vector2 knockbackVelocityHint;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasComponent <T>() where T:Component
    {
    return GetComponent<T>() != null;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasComponent<Rigidbody2D>()) return;
        Rigidbody2D weaponRigidbody = GetComponent<Rigidbody2D>();
        if (col.gameObject.tag == "Player") {
            print("Murder! :)");
            col.gameObject.GetComponent<playerController>().takeDamage(weaponRigidbody.velocity * damageFactor, (weaponRigidbody.velocity + knockbackVelocityHint) * knockbackFactor);

        }
    }
}
