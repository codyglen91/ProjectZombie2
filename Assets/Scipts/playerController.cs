using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Renderer model;
    [SerializeField] LayerMask ignoreLayer;


    [Header("----- Stats -----")]
    [Range(10, 100)][SerializeField] int hp;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;

    [Header("----- Physics -----")]
    [Range(15, 40)][SerializeField] int gravity;

    [Header("----- Guns -----")]
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int magazineSize;

    [Header("UI")]
    [SerializeField] Image healthBar;


    int jumpCount;
    int HPOriginal;

    float shootTimer;
    int remainingShots;

    Vector3 moveDir;
    Vector3 playerVelocity;




    void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        HPOriginal = hp;

        remainingShots = magazineSize;
        shootTimer = shootRate; // so you can shoot immediately

        UpdateHealthBar();

        Debug.Log($"[Player] Start. HP={hp}, Ammo={remainingShots}/{magazineSize}");

    }


    void Update()
    {
        movement();
        sprint();

        shootTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            reload();
        }
    }

    void movement()
    {
        moveDir = Input.GetAxis("Horizontal") * transform.right +
                  Input.GetAxis("Vertical") * transform.forward;

        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVelocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            jumpCount = 0;
            if (playerVelocity.y < 0) playerVelocity.y = 0;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVelocity.y = jumpSpeed;
            jumpCount++; //increment jump
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void Shoot()
    {

        if (remainingShots <= 0)
        {
            Debug.Log("[Player] Click! Out of ammo.");
            shootTimer = 0f;
            return;
        }

        shootTimer = 0f;
        remainingShots--;

        Vector3 origin = Camera.main.transform.position;
        Vector3 dir = Camera.main.transform.forward;

        RaycastHit hit;

        int mask = ~ignoreLayer.value; // hit everything EXCEPT layers in ignoreLayer

        Debug.DrawRay(origin, dir * shootDist, Color.red, 1f);

        if (Physics.Raycast(origin, dir, out hit, shootDist, mask, QueryTriggerInteraction.Collide))
        {
            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                Debug.Log($"[Player] Hit {hit.collider.name} for {shootDamage}");
                dmg.takeDamage(shootDamage);
            }
            else
            {
                Debug.Log($"[Player] Hit {hit.collider.name} but it has no IDamage.");
            }
        }
        else
        {
            Debug.Log("[Player] Shot missed.");
        }
    }

    public void AddAmmo(int amount)
    {
        remainingShots += amount;

        if (remainingShots > magazineSize) remainingShots = magazineSize;

        Debug.Log("Player Picked Up Ammo {amount}. Ammo now {remainingShots}/{magazoneSize}");
    }


    public void takeDamage(int amount)
    {
        hp -= amount; // FIXED (was hp = amount)
        hp = Mathf.Clamp(hp, 0, HPOriginal);

        UpdateHealthBar();

        Debug.Log($"[Player] Took {amount} damage. HP now {hp}");

        if (hp <= 0)
        {
            // you had gameManager.instance.youLose();
            if (gameManager.instance != null)
                gameManager.instance.youLose();
            else
                Debug.Log("[Player] Dead, but gameManager.instance is null.");
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > HPOriginal) hp = HPOriginal;
        Debug.Log($"[Player] Healed {amount}. HP now {hp}");

        UpdateHealthBar();
    }

    public void reload()
    {

        remainingShots = magazineSize;
        Debug.Log($"[Player] Reloaded. Ammo={remainingShots}/{magazineSize}");
    }

    void UpdateHealthBar()
    {
        if (healthBar != null && HPOriginal > 0)
        {
            healthBar.fillAmount = (float)hp / HPOriginal;
        }
    }




}// Normal is the side of a surface that has the side you can see, like the front of a wall
