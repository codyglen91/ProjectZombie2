using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class EnemymeleeAI : MonoBehaviour, IDamage
{
    GameObject player;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] LayerMask groundLayer, playerLayer;

    [SerializeField] float sightRange, attackRange;
    [SerializeField] bool playerInsight, playerInAttackRange;

    [SerializeField] Renderer model;
    [SerializeField] int meleeDamage;
    [SerializeField] float hitCooldown = 1.0f;

    [SerializeField] int hp;

    [SerializeField] Animator animator;

    Color colorOrig;

    float hitTimer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (model == null) model = GetComponentInChildren<Renderer>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (model != null && model.material.color != null)
            colorOrig = model.sharedMaterial.color;
        else
            Debug.LogWarning("[EnemyMeleeAI] Missing Render or material on {name}", this);

        player = GameObject.FindGameObjectWithTag("Player");

        if (gameManager.instance != null)
            gameManager.instance.updateGameGoal(1);

        hitTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }

        hitTimer -= Time.deltaTime;

        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        if (!playerInSight) return;

        agent.SetDestination(player.transform.position);

        bool playerInAttack = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (playerInAttack && hitTimer <= 0f)
        {
            meleeAttack();
            hitTimer = hitCooldown;
        }
    }

    void meleeAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        // do direct damage if close enough
        IDamage dmg = player.GetComponentInParent<IDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(meleeDamage);
            Debug.Log($"[Enemy] Hit player for {meleeDamage}");
        }
    }

    //can be used for all game objects that take damage
    public void takeDamage(int amount)
    {
        hp -= amount;

        if (hp <= 0)
        {
            if (gameManager.instance != null)
                gameManager.instance.updateGameGoal(-1);

            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red; // change color to red
        yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
        model.material.color = colorOrig; // change color back to original

    }
}
