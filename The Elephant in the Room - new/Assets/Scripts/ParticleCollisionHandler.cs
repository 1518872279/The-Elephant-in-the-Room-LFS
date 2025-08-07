using UnityEngine;
using System.Collections.Generic;

public class ParticleCollisionHandler : MonoBehaviour
{
    [Header("Collision Settings")]
    public GameObject splashPrefab;
    public LayerMask stainLayerMask = -1;
    
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError("ParticleCollisionHandler: No ParticleSystem found on this GameObject!");
        }
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("1");
        
        if (particleSystem == null) return;
        
        int count = particleSystem.GetCollisionEvents(other, collisionEvents);
        
        for (int i = 0; i < count; i++)
        {
            var collisionEvent = collisionEvents[i];
            var comp = collisionEvent.colliderComponent;
            
            if (comp != null)
            {
                if (comp.TryGetComponent<Stain>(out var stain))
                {
                    stain.TakeDamage(1);
                    
                    if (splashPrefab != null)
                    {
                        Vector3 splashPos = collisionEvent.intersection;
                        Quaternion splashRot = Quaternion.LookRotation(collisionEvent.normal);
                        Instantiate(splashPrefab, splashPos, splashRot);
                    }
                }
            }
        }
    }
} 