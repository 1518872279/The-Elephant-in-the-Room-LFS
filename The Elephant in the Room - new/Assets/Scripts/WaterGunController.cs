using UnityEngine;
using System.Collections.Generic;

public class WaterGunController : MonoBehaviour
{
    [Header("Water Gun Setup")]
    public GameObject waterGunParent;
    public ParticleSystem waterSpray;
    
    [Header("Effects")]
    public GameObject splashPrefab;
    public AudioSource waterGunAudio;
    public AudioClip spraySound;
    
    [Header("Controls")]
    public int sprayButton = 0;
    public float maxSprayDistance = 10f;
    public float stopDelay = 0.5f;
    
    [Header("Collision Detection")]
    public LayerMask stainLayerMask = -1;
    
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    private bool isSpraying = false;
    private bool isStopping = false;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        
        if (waterGunParent != null)
        {
            waterGunParent.SetActive(false);
        }
        
        if (waterSpray != null)
        {
            waterSpray.Stop();
        }
        
        if (waterGunAudio == null)
        {
            waterGunAudio = GetComponent<AudioSource>();
            if (waterGunAudio == null)
            {
                waterGunAudio = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    void Update()
    {
        HandleSprayInput();
    }

    private void HandleSprayInput()
    {
        bool sprayPressed = Input.GetMouseButtonDown(sprayButton);
        bool sprayHeld = Input.GetMouseButton(sprayButton);
        bool sprayReleased = Input.GetMouseButtonUp(sprayButton);
        
        if (sprayPressed)
        {
            StartSpray();
        }
        else if (sprayReleased)
        {
            StopSpray();
        }
        
        if (sprayHeld && isSpraying)
        {
            UpdateSprayDirection();
        }
    }

    private void StartSpray()
    {
        if (waterGunParent != null && !isSpraying)
        {
            waterGunParent.SetActive(true);
            isSpraying = true;
            
            ParticleSystem[] particleSystems = GetAllParticleSystems();
            foreach (ParticleSystem ps in particleSystems)
            {
                if (ps != null)
                {
                    ps.Play();
                }
            }
            
            if (waterGunAudio != null && spraySound != null)
            {
                waterGunAudio.PlayOneShot(spraySound);
            }
        }
    }

    private void StopSpray()
    {
        if (waterGunParent != null && isSpraying && !isStopping)
        {
            isStopping = true;
            isSpraying = false;
            StartCoroutine(StopSprayWithDelay());
        }
    }

    private void UpdateSprayDirection()
    {
        if (playerCamera == null || waterGunParent == null) return;
        
        Vector3 sprayDirection = playerCamera.transform.forward;
        transform.rotation = Quaternion.LookRotation(sprayDirection);
    }

    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("1");
        if (waterGunParent == null || !waterGunParent.activeInHierarchy) return;
        
        ParticleSystem[] particleSystems = GetAllParticleSystems();
        
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps == null) continue;
            
            int count = ps.GetCollisionEvents(other, collisionEvents);
            
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

    public void EnableWaterGun()
    {
        gameObject.SetActive(true);
    }

    public void DisableWaterGun()
    {
        StopSpray();
    }

    private System.Collections.IEnumerator StopSprayWithDelay()
    {
        yield return new WaitForSeconds(stopDelay);
        
        ParticleSystem[] particleSystems = GetAllParticleSystems();
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null && ps.isPlaying)
            {
                ps.Stop();
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (waterGunParent != null)
        {
            waterGunParent.SetActive(false);
        }
        
        isStopping = false;
    }

    private ParticleSystem[] GetAllParticleSystems()
    {
        if (waterGunParent == null) return new ParticleSystem[0];
        return waterGunParent.GetComponentsInChildren<ParticleSystem>();
    }
} 