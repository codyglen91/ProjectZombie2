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

    void Start()
    {
        // Spawn up to maxAlive at the start
        for (int i = 0; i < maxAlive; i++)
        {
            SpawnOne();
        }
    }

    // Called by the pickup when the player collects it
    public void NotifyMedkitPickedUp()
    {
        aliveCount--;
        if (aliveCount < 0) aliveCount = 0;

        // Wait X seconds, then spawn another (at a spawn point)
        Invoke(nameof(SpawnOne), respawnDelay);
    }

    void SpawnOne()
    {
        if (medkitPrefab == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;
        if (aliveCount >= maxAlive) return;

        int idx = GetRandomSpawnIndexDifferentFromLast();
        Transform p = spawnPoints[idx];
        if (p == null) return;

        GameObject kit = Instantiate(medkitPrefab, p.position, p.rotation);
        aliveCount++;

        // Let the pickup know who its spawner is, so it can notify on pickup
        HealthPickup pickup = kit.GetComponent<HealthPickup>();
        if (pickup != null)
        {
            pickup.SetSpawner(this);
        }
    }

    int GetRandomSpawnIndexDifferentFromLast()
    {
        if (spawnPoints.Length == 1)
            return 0;

        int idx = Random.Range(0, spawnPoints.Length);
        if (idx == lastSpawnIndex)
            idx = (idx + 1) % spawnPoints.Length; // quick �different� pick

        lastSpawnIndex = idx;
        return idx;
    }
}
