using UnityEngine;

[CreateAssetMenu]

public class gunStatsNew : ScriptableObject
{
    public GameObject gunModel;



    [Range(1, 10)] public int shootDamage;
    [Range(3, 1000)] public int shootDist;
    [Range(.01f, 3)] public float shootRate;

    public int ammoCur;
    [Range(5, 50)] public int ammoMaz;

    //public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;

}
