using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int hp;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;


    [SerializeField] Transform meleePos;
    [SerializeField] float attackRange;
    [SerializeField] float attackRate;
    [SerializeField] int attackDamage;

    Color colorOrig;
    float shootTimer;
    float meleeTimer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;

        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        shootTimer += Time.deltaTime;

        meleeTimer += Time.deltaTime;


        agent.SetDestination(gameManager.instance.player.transform.position); // Set the destination of the NavMeshAgent to the player's position

        if (shootTimer >= shootRate)
        {
            shoot();
        }

        
        if (Vector3.Distance(transform.position, gameManager.instance.player.transform.position) <= attackRange && meleeTimer >= attackRate)
        {
            melee();
        }

    }

    void shoot()
    {
        shootTimer = 0;

        Instantiate(bullet, shootPos.position, transform.rotation); // Spawn bullet at shootPos with enemy rotation
    }

    void melee()
    {
        meleeTimer = 0;

        Collider[] hits = Physics.OverlapSphere(meleePos.position, attackRange);

        for (int i = 0; i < hits.Length; i++)
        {
            IDamage damage = hits[i].GetComponent<IDamage>();
            if (damage != null)
            {
                damage.takeDamage(attackDamage);
                break; //only hit one target
            }
        }
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
