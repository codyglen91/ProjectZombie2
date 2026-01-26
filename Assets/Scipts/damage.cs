using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{

    enum damageType { moving, stationary, DOT } // moving: moves forward and deals damage on contact, stationary: stays in place and deals damage on contact, DOT: deals damage over time when in contact

    [SerializeField] damageType type; // Type of damage behavior
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] GameObject hitEffect;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(type == damageType.moving)
        {
            rb.linearVelocity = transform.forward * speed; // Set the velocity of the Rigidbody to move forward at the specified speed
            Destroy(gameObject, destroyTime); // Destroy the game object after the specified time
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        // if(other.isTrigger) return; // Ignore trigger colliders

        IDamage d = other.GetComponentInParent<IDamage>(); // Try to get the IDamage component from the other object
        if (d != null)
        {
            if(d != null && type != damageType.DOT) // If the other object has an IDamage component and the damage type is not DOT
            {
                d.takeDamage(damageAmount);
            }
            if(type == damageType.moving)
            Destroy(gameObject); // Destroy the damage object after dealing damage

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger) return; // Ignore trigger colliders

        IDamage damage = other.GetComponent<IDamage>(); // Try to get the IDamage component from the other object
        if (damage != null && type == damageType.DOT && !isDamaging) // If the damage type is DOT and not already damaging
        {
            StartCoroutine(damageOther(damage)); // Start the damage over time coroutine
        }
        
    }

    IEnumerator damageOther (IDamage d) // Coroutine to deal damage over time
    {
        isDamaging = true; // Set the isDamaging flag to true
        d.takeDamage(damageAmount); // Deal damage to the other object
        yield return new WaitForSeconds(damageRate); // Wait for the specified damage rate
        isDamaging = false; // Set the isDamaging flag to false


    }
}
