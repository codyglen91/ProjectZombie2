using UnityEngine;
// Continue Lecture 2 at 3:34:40, this note is from 1/10/26
// 1/10/26 Look for Player Collider, not currently taking damage
// For top down games, just drag the camera to the Player in Unity Hierarchy
public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Component -----")]// creates sections in Player controller when attached to a GameObject
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(1,10)][SerializeField] int hp;
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

    Vector3 moveDir;
    Vector3 playerVelocity;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     HPOriginal = hp;
     
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime; //shoot timer counts up

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();// Call jump first
        controller.Move(playerVelocity * Time.deltaTime); // Only jump need gravity

        

        if (controller.isGrounded) // controller colliders are on top, bottom, and sides
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;// Add gravity so he comes down
        }

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            Shoot();
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
            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }

        }
    }

    public void takeDamage(int amount)
    {
        hp -= amount;

        //Check if the player is dead
        if(hp <=0)
        {
            gameManager.instance.youLose();
        }
    }

    public void heal(int amount)
    {
               hp += amount;
        if(hp > HPOriginal)
        {
            hp = HPOriginal;
        }
    }
}// Normal is the side of a surface that has the side you can see, like the front of a wall
