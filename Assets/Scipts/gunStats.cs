using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject gunModel;

    [Range(1,10)] public int shootDamage;
    [Range(3, 1000)] public int shootDist;
    [Range(0.1f, 4)] public float shootRate;

    public int ammoCurrent;
    [Range(5, 50)] public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol; //Always between 0 and 1 for VOL


}
