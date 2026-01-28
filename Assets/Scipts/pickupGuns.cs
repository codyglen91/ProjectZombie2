using UnityEngine;
//Script to pickup guns in the game world, attached to the pickup gun prefab.
public class pickupGuns : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] gunStats gun;
    void Start()
    {

    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        IPickup pik = other.GetComponent<IPickup>();   
        if (pik != null)
        {
            pik.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
