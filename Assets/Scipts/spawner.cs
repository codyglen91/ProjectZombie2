using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int spawnRate;
    [SerializeField] int spawnDist;

    private int spawnAmount;
    int spawnCount;
    float spawnTimer;

    bool startSpawning;

    private bool levelStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    } 

    // Update is called once per frame
    void Update()
    {
        if (!startSpawning) return;
        spawnTimer += Time.deltaTime;

        if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
            spawn();

        if(spawnCount >= spawnAmount)
        {
            startSpawning = false;
        }
    }




    public void StartLevel(int amount)
    {
        spawnAmount = amount;
        spawnCount = 0;
        spawnTimer = 0f;

        startSpawning = true;

        gameManager.instance.updateGameGoal(spawnAmount);
    }
    void spawn()
    {
        spawnTimer = 0;
        spawnCount++;

        Vector3 ranPos = Random.insideUnitSphere * spawnDist;
        ranPos += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, spawnDist, 1);

        Instantiate(objectToSpawn, hit.position, Quaternion.Euler(0f, Random.Range(0f, 300f), 0f));
    }
}
