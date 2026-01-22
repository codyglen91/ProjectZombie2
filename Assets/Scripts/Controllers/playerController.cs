using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Renderer model;
    [SerializeField] LayerMask ignoreLayer;


    [Header("----- Stats -----")]
    [Range(0,10)][SerializeField] int hp;
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

    int jumpCount;
    int HPOriginal;

    float shootTimer;
    int remaningShots;

    Vector3 moveDir;
    Vector3 playerVelocity;



   
    void Start()
    {
     remaningShots = magazineSize;
     HPOriginal = hp;
     
    }

    
    void Update()
    {
        movement();
        sprint();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime; 

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVelocity * Time.deltaTime);

        

        if (controller.isGrounded) 
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
            remaningShots -= 1;
        }

        if(Input.GetButton("Fire2"))
        {
            reload();
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
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if(Input.GetButtonUp ("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void Shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if(remaningShots <= 0)
            {
                return;
            }
            else if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
             
            }

        }
    }

    public void takeDamage(int amount)
    {
        Debug.Log(amount + " damage taken!");
        hp -= amount;

        //Check if the player is dead
        if (hp <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void reload()
    {
    
            remaningShots = magazineSize;
        
    }

}// Normal is the side of a surface that has the side you can see, like the front of a wall
