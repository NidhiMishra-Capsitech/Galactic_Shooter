using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    // How long the effect should live before being destroyed
    public float lifeTime = 2f;

    void Start()
    {
        // Destroy this GameObject after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }
}