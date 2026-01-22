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
    [SerializeField] float meleeDamage;

    [SerializeField] int hp;

    [SerializeField] Animator animator;

    Color colorOrig;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (model == null)
            model = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        colorOrig = model.material.color;
        player = GameObject.Find("Player");

        gameManager.instance.updateGameGoal(1);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInsight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (agent == null) agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (agent == null || player == null) return;

        agent.SetDestination(player.transform.position);

        if (playerInsight && playerInAttackRange)
        {
            meleeAttack();
        }
    }

    void meleeAttack()
    {
        animator.SetTrigger("Attack");
        agent.SetDestination(transform.position);
    }

    public int GetV(int amount)
    {
        return hp -= amount;
    }

    //can be used for all game objects that take damage
    public void takeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(flashRed()); // Start the flashRed coroutine
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red; // change color to red
        yield return new WaitForSeconds(0.1f); // wait for 0.1 seconds
        model.material.color = colorOrig; // change color back to original

    }
}
