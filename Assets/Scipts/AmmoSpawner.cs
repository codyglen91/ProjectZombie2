using UnityEngine;

public class AmmoSpawner : MonoBehaviour
{

    [Header("What To Spawn")]
    [SerializeField] GameObject ammoPrefab;

    [Header("Where To Spawn")]
    [SerializeField] Transform[] spawnPoints;

    [Header("How Many To Spawn")]
    [SerializeField] int maxAlive = 3;

    [Header("When To Spawn")]
    [SerializeField] float respawnDelay = 10f;

    int aliveCount = 0;

    GameObject[] activeAtPoint;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[AmmoSpawner] No spawn points assigned.", this);
            return;
        }

        activeAtPoint = new GameObject[spawnPoints.Length];


        for (int i = 0; i < maxAlive; i++)
        {
            SpawnOne();
        }
    }

    public void NotifyPickedUp(int spawnIndex)
    {
        aliveCount--;
        if (aliveCount < 0) aliveCount = 0;

        if (activeAtPoint != null && spawnIndex >= 0 && spawnIndex < activeAtPoint.Length)
            activeAtPoint[spawnIndex] = null;

        Invoke(nameof(SpawnOne), respawnDelay);
    }

    public void NotifyAmmoPickUp()
    {
        NotifyPickedUp(-1);
    }

    void SpawnOne()
    {
        if (ammoPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (aliveCount >= maxAlive) return;

        int idx = GetRandomFreeSpawnIndex();
        if (idx == -1) return;

        Transform p = spawnPoints[idx];
        if (p == null) return;

        GameObject ammo = Instantiate(ammoPrefab, p.position, p.rotation);
        aliveCount++;

        activeAtPoint[idx] = ammo;

        //let the pickup tell us when it gets collected
        AmmoPickUP pickUP = ammo.GetComponent<AmmoPickUP>();
        if (pickUP != null)
        {
            pickUP.SetSpawner(this);
        }
        else
        {
            Debug.LogWarning("[AmmoSpawner] Spawned ammoPrefab has no AmmoPickUP component.", ammo);
        }
    }

    int GetRandomFreeSpawnIndex()
    {
        int[] free = new int[spawnPoints.Length];
        int freeCount = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            // free if nothing tracked OR tracked object destroyed
            if (activeAtPoint[i] == null)
                free[freeCount++] = i;
        }

        if (freeCount == 0) return -1;

        int pick = Random.Range(0, freeCount);
        return free[pick];
    }

}
