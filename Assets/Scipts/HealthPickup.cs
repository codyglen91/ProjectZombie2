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

    // NEW: which spawn point this pickup belongs to
    int spawnIndex = -1;

    // Keep this (in case you still call it anywhere)
    public void SetSpawner(MedkitSpawner s)
    {
        spawner = s;
        startPos = transform.localPosition;
        spawnIndex = -1;
    }

    // REQUIRED for your current MedkitSpawner.cs
    public void SetSpawner(MedkitSpawner s, int idx)
    {
        spawner = s;
        spawnIndex = idx;
        startPos = transform.localPosition;
    }

    void Update()
    {
        if (!enableBobAndRotate) return;

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);

        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startPos + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Match your player script class name:
        // If yours is still "playerController" lowercase, change PlayerController -> playerController
        playerControllerNew pc = other.GetComponent<playerControllerNew>();
        if (pc == null)
            pc = other.GetComponentInParent<playerControllerNew>();

        if (pc == null)
            return;

        pc.Heal(healAmount);

        // IMPORTANT: notify BEFORE destroying so the spawn point frees up
        if (spawner != null)
            spawner.NotifyMedkitPickedUp(spawnIndex);

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
