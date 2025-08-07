using UnityEngine;
using UnityEngine.Events;

public class Stain : MonoBehaviour
{
    public int health = 3;
    public GameObject damageEffect;
    public AudioClip cleanSound;

    public UnityEvent onCleaned;
    private bool isDead = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;
        
        health -= amount;
        
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, transform.rotation);
        }
        
        if (health <= 0)
        {
            isDead = true;
            
            if (cleanSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(cleanSound);
            }
            
            onCleaned?.Invoke();
            Destroy(gameObject);
        }
    }
} 