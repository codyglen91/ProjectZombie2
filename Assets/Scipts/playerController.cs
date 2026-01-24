using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, IPickup
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
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int magazineSize;

    int jumpCount;
    int HPOriginal;
    int gunListPos;

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
        // takes gun list to make sure you have a gun and checks ammo to see if you can shoot
        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCurrent > 0 && shootTimer >= shootRate)
        {
            Shoot();
            remaningShots -= 1;
        }

        if(Input.GetButton("Fire3"))
        {
            reload();
        }

        selectGun();
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

        gunList[gunListPos].ammoCurrent -= 1; //Decrease current ammo of the gun being used if 0 can't shoot

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
   
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)hp / HPOriginal;
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

     void reload()
    {
    
            if(Input.GetButtonDown("Fire3"))
            {
             gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;
            }
        
    }
    // Found in IPickup to make Player shoot stats be gun stats
    // That way his damage output follows what gun he has.
    //Inventory system needs below here
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun(); //Change gun is the function for all of the stat changes and gun change

    }
    void changeGun()
    {
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;
        //for transitioning the gun model renderer and mesh so picked up gun is shown
        // Always do this this way for ease.
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos > gunList.Count - 1)
        {
            gunListPos++; //scrolling up changes gun up
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos < 0)
        {
            gunListPos--; //scrolling up changes gun up
            changeGun();
        }
        
    }
}
// Normal is the side of a surface that has the side you can see, like the front of a wall
