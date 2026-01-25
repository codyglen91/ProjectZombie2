using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] int healAmount = 10;
    [SerializeField] bool destroyOnPickup = true;

    [Header("visual Movement")]
    [SerializeField] bool enableBobAndRotate = true;
    [SerializeField] float rotateSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] float bobSpeed;

    Vector3 startPos;

    MedkitSpawner spawner;

    public void SetSpawner(MedkitSpawner s)
    {
        spawner = s;
        startPos = transform.localPosition;

    }

    void Update()
    {
        if (!enableBobAndRotate) return;

        transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime, Space.Self);

        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startPos + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerController pc = other.GetComponent<playerController>();
        if (pc == null)
            pc = other.GetComponentInParent<playerController>();

        if (pc == null)
            return;

        pc.heal(healAmount);

        // IMPORTANT: notify BEFORE destroying
        if (spawner != null)
            spawner.NotifyMedkitPickedUp();

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
