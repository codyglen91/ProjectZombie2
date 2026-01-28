using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{

    enum damageType { moving, stationary, DOT } // moving: moves forward and deals damage on contact, stationary: stays in place and deals damage on contact, DOT: deals damage over time when in contact

    [SerializeField] damageType type = damageType.moving; // Type of damage behavior
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
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (type == damageType.moving)
        {
            if (rb != null)
                rb.linearVelocity = transform.forward * speed;

            Destroy(gameObject, destroyTime);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        // ignore triggers so bullets don’t “hit” trigger volumes
        if (other.isTrigger) return;

        IDamage target = other.GetComponentInParent<IDamage>();
        if (target == null) return;

        target.takeDamage(damageAmount);

        if (hitEffect != null)
            Instantiate(hitEffect, transform.position, transform.rotation);

        // moving projectiles should die on hit
        if (type == damageType.moving)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (type != damageType.DOT) return;
        if (other.isTrigger) return;
        if (isDamaging) return;

        IDamage target = other.GetComponentInParent<IDamage>();
        if (target == null) return;

        StartCoroutine(damageOther(target));

    }

    IEnumerator damageOther(IDamage d) // Coroutine to deal damage over time
    {
        isDamaging = true; // Set the isDamaging flag to true
        d.takeDamage(damageAmount); // Deal damage to the other object
        yield return new WaitForSeconds(damageRate); // Wait for the specified damage rate
        isDamaging = false; // Set the isDamaging flag to false


    }
}
