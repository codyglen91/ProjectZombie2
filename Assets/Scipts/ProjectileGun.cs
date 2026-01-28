using UnityEngine;
using TMPro;


[CreateAssetMenu]


public class ProjectileGun : ScriptableObject
{
    [SerializeField] public GameObject gunModel;
    [SerializeField] public GameObject bullet;


    [Range(1, 10)] public int shootDamage;
    [Range(3, 1000)] public int shootDist;
    [Range(.01f, 3)] public float shootRate;


    public float shootForce;
    public float upwardForce;

    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;
    public float timeBetweenShots;

    public int magazineSize;
    public int bulletsLeft;
    public int bulletsPerTap;

   public bool allowButtonHold;

   

 

  

   
}
