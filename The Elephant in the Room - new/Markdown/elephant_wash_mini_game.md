# Elephant Wash Mini-Game

A step-by-step guide and sample code for implementing the elephant washing mini-game in Unity.

## 1. Mini-Game Flow

1. **Enter Mini-Game**
   - Player interacts with a jacuzzi trigger (`OnTriggerEnter`).
   - Disable main controls, switch camera/UI to the wash view.
   - Call `ElephantWashManager.StartWash()`.
2. **Spawn Stains**
   - Randomly place a number of stain prefabs on the elephant’s surface.
   - Each stain has a health value.
3. **Spray Water**
   - Player holds left-mouse to fire the water-gun VFX.
   - `WaterGun` script detects collisions with stain objects and applies damage.
4. **Track Progress**
   - Decrement stain count as they are cleaned.
   - Update a UI bar or counter.
5. **End Condition**
   - When `remainingStains == 0`, call `ElephantWashManager.EndWash()` to reward the player and return to the main game.

---

## 2. Stain System

**Stain.cs**

```csharp
using UnityEngine;
using UnityEngine.Events;

public class Stain : MonoBehaviour
{
    [Tooltip("Number of hits to remove this stain.")]
    public int health = 3;

    public UnityEvent onCleaned;
    private bool isDead = false;

    /// <summary>
    /// Call this when the water hits the stain.
    /// </summary>
    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;
        health -= amount;
        if (health <= 0)
        {
            isDead = true;
            onCleaned?.Invoke();
            Destroy(gameObject);
        }
    }
}
```

- **onCleaned** fires when the stain is removed, letting the manager decrement its counter.

---

## 3. Spawning Stains on the Elephant

**ElephantWashManager.cs**

```csharp
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class ElephantWashManager : MonoBehaviour
{
    [Tooltip("Prefab with Stain.cs on it")]
    public GameObject stainPrefab;
    [Tooltip("How many stains to spawn")]
    public int stainCount = 20;

    private Mesh elephantMesh;
    private MeshCollider meshCollider;
    private int remainingStains;

    void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        elephantMesh = GetComponent<MeshFilter>().mesh;
    }

    public void StartWash()
    {
        remainingStains = stainCount;
        for (int i = 0; i < stainCount; i++)
        {
            Vector3 worldPos = RandomPointOnMeshSurface(elephantMesh, transform.localToWorldMatrix);
            GameObject stain = Instantiate(stainPrefab, worldPos, Quaternion.identity, transform);
            stain.transform.rotation = Quaternion.LookRotation(meshCollider.sharedMesh.normals[0]);
            Stain s = stain.GetComponent<Stain>();
            s.onCleaned.AddListener(OnStainCleaned);
        }
    }

    void OnStainCleaned()
    {
        remainingStains--;
        // Update UI here
        if (remainingStains <= 0)
            EndWash();
    }

    public void EndWash()
    {
        Debug.Log("All clean!");
        // Reward player and exit mini-game
    }

    Vector3 RandomPointOnMeshSurface(Mesh mesh, Matrix4x4 localToWorld)
    {
        var tris = mesh.triangles;
        var verts = mesh.vertices;
        float[] cumAreas = new float[tris.Length / 3];
        float total = 0;
        for (int i = 0; i < cumAreas.Length; i++)
        {
            Vector3 a = verts[tris[i*3+0]];
            Vector3 b = verts[tris[i*3+1]];
            Vector3 c = verts[tris[i*3+2]];
            total += Vector3.Cross(b - a, c - a).magnitude * 0.5f;
            cumAreas[i] = total;
        }

        float r = Random.value * total;
        int triIndex = System.Array.FindIndex(cumAreas, area => area >= r);
        Vector3 v0 = verts[tris[triIndex*3+0]];
        Vector3 v1 = verts[tris[triIndex*3+1]];
        Vector3 v2 = verts[tris[triIndex*3+2]];
        float u = Random.value;
        float v = Random.value;
        if (u + v > 1) { u = 1 - u; v = 1 - v; }
        Vector3 point = v0 + u*(v1-v0) + v*(v2-v0);
        return localToWorld.MultiplyPoint(point);
    }
}
```

---

## 4. Integrating Your Water-Gun VFX

Extend your existing `WaterGun` script:

```csharp
void OnParticleCollision(GameObject other)
{
    int count = spray.GetCollisionEvents(other, collisionEvents);
    for (int i = 0; i < count; i++)
    {
        var comp = collisionEvents[i].colliderComponent;
        if (comp != null && comp.TryGetComponent<Stain>(out var stain))
        {
            stain.TakeDamage(1);
        }
        Vector3 pos = collisionEvents[i].intersection;
        Instantiate(splashPrefab, pos, Quaternion.LookRotation(collisionEvents[i].normal));
    }
}
```

- Enable **Collision Module** on your Particle System and include stain layers.

---

## 5. UI & Feedback

- **Progress Bar / Counter**: Show "Stains left: X" or radial fill.
- **Audio & VFX**: Play a foam burst and sound when a stain is cleaned.
- **Completion Cue**: Camera pan, jingle, or fade-out when all stains are gone.

---

## 6. Tips & Extensions

- **Shader-Based Cleaning**: Use a splatmap RenderTexture to reveal clean areas.
- **Difficulty Scaling**: Adjust `stainCount` or `health` per stain.
- **Scoring**: Track time or water used and display a grade at the end.

