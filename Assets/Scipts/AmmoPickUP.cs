using Unity.VisualScripting;
using UnityEngine;

public class AmmoPickUP : MonoBehaviour
{
    // Configurable fields
    //=====================
    // Amount of ammo provided by this pickup
    [Header("Ammo")]
    [SerializeField] int ammoAmount = 10;
    [SerializeField] bool destroyOnPickup = true;

    [Header("Visual Movement")]
    [SerializeField] bool enableBobAndRotate = true;
    [SerializeField] float rotateSpeed;
    [SerializeField] float bobHeight;
    [SerializeField] float bobSpeed;

    Vector3 startPos;

    // Reference to the spawner that created this pickup
    AmmoSpawner spawner;

    int spawnIndex = -1;

    public void SetSpawner(AmmoSpawner s)
    {
        spawner = s;
        startPos = transform.localPosition;
        spawnIndex = -1;
    }

    public void SetSpawner(AmmoSpawner s, int idx)
    {
        spawner = s;
        spawnIndex = idx;
        startPos = transform.localPosition;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!enableBobAndRotate) return;

        // Rotate the pickup
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.Self);

        // Bob the pickup up and down
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startPos + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerControllerNew pc = other.GetComponent<playerControllerNew>();

        if (pc == null)
            pc = other.GetComponentInParent<playerControllerNew>();

        // Add ammo to the player
        if (pc != null)
        {
            pc.RefillCurrentMagazine();
        }


        // Notify the spawner before destroying
        if (spawner != null)
            spawner.NotifyAmmoPickUp();

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
