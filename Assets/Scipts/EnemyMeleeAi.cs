using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemymeleeAI : MonoBehaviour, IDamage
{
    GameObject player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer, playerLayer;
    [SerializeField] Transform headPos;

    [SerializeField] GameObject dropObject;
    [SerializeField] float offsetY;

    [SerializeField] int hp;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInsight, playerInAttackRange;

    [SerializeField] Renderer model;
    [SerializeField] float meleeDamage;

    [SerializeField] Animator animator;

    float angleToPlayer;
    float roamTimer;
    float stoppingDistOrig;

    Vector3 playerDir;
    Vector3 startingPos;

    bool playerInRange;
    Color colorOrig;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.sharedMaterial.color;

        gameManager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        //shootTimer += Time.deltaTime;
        if(agent.remainingDistance < 0.01f)
        roamTimer += Time.deltaTime;


        if (playerInRange && canSeePlayer())
        {
            checkRoam();
        }
        else if(!playerInRange)
        {
            checkRoam();
        }
    }

    bool canSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (angleToPlayer <= FOV / 2 && hit.collider.CompareTag("Player"))
            {
                agent.SetDestination(gameManager.instance.player.transform.position); // Move towards player

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget(); // Can see player
                }

                meleeAttack();
                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false; // Cannot see player
    }

    void checkRoam()
    {
        if(agent.remainingDistance < 0.01f && roamTimer >= roamPauseTime)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    void meleeAttack()
    {
        animator.SetTrigger("Attack");
        agent.SetDestination(transform.position);
    }

    //can be used for all game objects that take damage
    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            // Can instantiate a scriptable game object
            dropItem();

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed()); // Start the flashRed coroutine
        }
    }

    IEnumerator flashRed()
    {
        model.sharedMaterial.color = Color.red; // change color to red
        yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
        model.sharedMaterial.color = colorOrig; // change color back to original

    }
    void dropItem()
    {
        if (dropObject != null)
            Instantiate(dropObject, new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z), transform.rotation);
    }
}
