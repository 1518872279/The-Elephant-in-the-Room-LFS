using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class ExaminableObject : MonoBehaviour
{
    void Reset()
    {
        gameObject.layer = LayerMask.NameToLayer("Examinable");
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }
} 