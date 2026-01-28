using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerNew : MonoBehaviour , IDamage , IPickup
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera playerCamera;
    [SerializeField] Transform attackPoint;
    [SerializeField] LayerMask ignoreLayer;

    [Header("----- Stats -----")]
    [Range(0, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int speed;
    [Range(2, 5)][SerializeField] int sprintMod;
    [Range(8, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;

    [Header("----- Phyisics -----")]
    [Range(15, 40)][SerializeField] int gravity;

    [Header("----- Guns -----")]
    [SerializeField] List<ProjectileGun> gunList = new List<ProjectileGun>();

    [SerializeField] GameObject gunModel;
    [SerializeField] public float shootForce;
    [SerializeField] public float upwardForce;
    [SerializeField] public float timeBetweenShooting;
    [SerializeField] public float spread;
    [SerializeField] public float reloadTime;
    [SerializeField] public float timeBetweenShots;
    [SerializeField] public int magazineSize;
    [SerializeField] public int bulletsPerTap;
    [SerializeField] public bool allowButtonHold;
    [SerializeField] int bulletsShot;


    bool shooting;
    bool readyToShoot;
    bool reloading;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    //[SerializeField] float shootRate;

//stuff 

    int jumpCount;
    int HPOrig;
    int gunListPos;

    bool allowInvoke = true;

    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;

    void Start()
    {
        HPOrig = HP;
        readyToShoot = true;
        updateplayerUI();
    }

    void Update()
    {
        
       movement();
       Sprint();

        MyInput();

      //  if (ammunitionDisplay != null)
      //      ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + "/" + magazineSize / bulletsPerTap);
    }

    bool HasValidGun()
    {
        return gunList.Count > 0 && gunListPos >= 0 && gunListPos < gunList.Count;
    }

    void movement()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        shootTimer += Time.deltaTime;

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump(); 
        controller.Move(playerVel * Time.deltaTime);

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        selectGun();
    }

    void Sprint()
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

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updateplayerUI();
        StartCoroutine(flashRed());

        //Check if the player is dead
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    private void MyInput()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        if (allowButtonHold)
            shooting = Input.GetKey(KeyCode.Mouse0);
        else
            shooting = Input.GetKeyDown(KeyCode.Mouse0);

        

        if (Input.GetKeyDown(KeyCode.R) && gun.bulletsLeft < magazineSize && !reloading)
            Reload();

        if (readyToShoot && shooting && !reloading && gun.bulletsLeft <= 0)
            Reload();

        if (readyToShoot && shooting && !reloading && gun.bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        Debug.Log("Shoot() called");

        readyToShoot = false;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); ;
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(gunList[gunListPos].bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        // for normal bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //if (gunList[gunListPos].muzzleFlash != null)
        //   Instantiate(gunList[gunListPos].muzzleFlash, gunList[gunListPos].attackPoint.position, Quaternion.identity);

        gun.bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && gun.bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        if (!HasValidGun()) return;

        gunList[gunListPos].bulletsLeft = gunList[gunListPos].magazineSize;
        reloading = false;
    }


    public void getGunStats(ProjectileGun gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();

    }

    void changeGun()
    { 
        if (!HasValidGun()) return;

        ProjectileGun gun = gunList[gunListPos];

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        timeBetweenShooting = gun.shootRate;

        magazineSize = gun.magazineSize;
        bulletsPerTap = gun.bulletsPerTap;
        allowButtonHold = gun.allowButtonHold;

       
        bulletsShot = 0;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (!HasValidGun()) return;
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
                gunListPos++;
                changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel")< 0 && gunListPos> 0) 
            {
            gunListPos--;
            changeGun();
            }
    }

    public void updateplayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashRed()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

}
