using UnityEngine;

public class MedkitSpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] GameObject medkitPrefab;

    [Header("Where to spawn")]
    [SerializeField] Transform[] spawnPoints;

    [Header("How many")]
    [SerializeField] int maxAlive = 2;

    [Header("Respawn")]
    [SerializeField] float respawnDelay = 10f;

    int aliveCount = 0;
    int lastSpawnIndex = -1;

    GameObject[] activeAtPoint;

    void Start()
    {
        // Spawn up to maxAlive at the start
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[MedkitSpawner] No spawn points assigned.", this);
            return;
        }

        activeAtPoint = new GameObject[spawnPoints.Length];

        // Spawn up to maxAlive at the start
        for (int i = 0; i < maxAlive; i++)
        {
            SpawnOne();
        }
    }

    // Called by the pickup when the player collects it
    public void NotifyMedkitPickedUp(int spawnIndex)
    {
        aliveCount--;
        if (aliveCount < 0) aliveCount = 0;

        if (activeAtPoint != null && spawnIndex >= 0 && spawnIndex < activeAtPoint.Length)
        {
            activeAtPoint[spawnIndex] = null;
        }

        // Wait X seconds, then spawn another (at a spawn point)
        Invoke(nameof(SpawnOne), respawnDelay);
    }

    void SpawnOne()
    {
        if (medkitPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (aliveCount >= maxAlive) return;

        // In case something got destroyed without notifying, clean nulls
        for (int i = 0; i < activeAtPoint.Length; i++)
        {
            if (activeAtPoint[i] == null) continue;
            // If the object was destroyed, Unity makes it "== null"
            // so this loop mainly exists for clarity / safety.
        }

        int idx = GetRandomFreeSpawnIndex();
        if (idx == -1)
        {
            // No free points available
            return;
        }

        Transform p = spawnPoints[idx];
        if (p == null) return;

        GameObject kit = Instantiate(medkitPrefab, p.position, p.rotation);
        aliveCount++;

        activeAtPoint[idx] = kit;

        // Let the pickup know who spawned it + which spawn point index it used
        HealthPickup pickup = kit.GetComponent<HealthPickup>();
        if (pickup != null)
        {
            pickup.SetSpawner(this, idx);
        }
        else
        {
            Debug.LogWarning("[MedkitSpawner] Spawned medkitPrefab has no HealthPickup component.", kit);
        }
    }

    int GetRandomFreeSpawnIndex()
    {
        // Gather all FREE spawn indices
        int[] free = new int[spawnPoints.Length];
        int freeCount = 0;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;

            // Free if nothing currently tracked OR tracked object got destroyed
            if (activeAtPoint[i] == null)
            {
                free[freeCount++] = i;
            }
        }

        if (freeCount == 0) return -1;

        int pick = Random.Range(0, freeCount);
        return free[pick];
    }
}
