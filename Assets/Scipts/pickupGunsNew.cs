using NUnit.Framework.Interfaces;
using UnityEngine;

public class pickupGunsNew : MonoBehaviour
{
    [SerializeField] ProjectileGun gun;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();

        if (pik != null)
        {
            gun.bulletsLeft = gun.magazineSize;
            pik.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
