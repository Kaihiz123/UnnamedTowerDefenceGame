using UnityEngine;

public class DestroyObjectSimple : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3f); // Used for ExplosionVFX effect
    }
}
