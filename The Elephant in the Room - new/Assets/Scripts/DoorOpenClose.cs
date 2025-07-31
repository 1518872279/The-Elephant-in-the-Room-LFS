using UnityEngine;
using System.Collections;

public class DoorOpenClose : MonoBehaviour
{
    [Header("Door Objects")]
    public GameObject doorObject;        // The door GameObject that will rotate
    public Transform hinge;              // Pivot for rotation (the actual door hinge)
    
    [Header("Rotation Settings")]
    public float openAngle = 90f;        // Degrees to open
    public float openSpeed = 3f;         // Lerp speed
    public string playerTag = "Player"; // Tag used for detecting player entry/exit

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        // Cache the start rotation of the door object
        if (doorObject == null)
        {
            Debug.LogError("Door GameObject not set on DoorOpenClose.");
            return;
        }
        
        if (hinge == null)
        {
            Debug.LogError("Hinge Transform not set on DoorOpenClose.");
            return;
        }
        
        closedRot = doorObject.transform.localRotation;
        openRot   = closedRot * Quaternion.Euler(0f, openAngle, 0f);
        //Debug.Log($"DoorOpenClose initialized - Door: {doorObject.name}, Hinge: {hinge.name}, Closed Rot: {closedRot}, Open Rot: {openRot}");
    }

    // Trigger events now fire on this trigger zone object
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger Enter: {other.name} with tag {other.tag}");
        if (other.CompareTag(playerTag) && !isOpen)
        {
            Debug.Log("Opening door...");
            StopAllCoroutines();
            StartCoroutine(RotateDoor(openRot));
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"Trigger Exit: {other.name} with tag {other.tag}");
        if (other.CompareTag(playerTag) && isOpen)
        {
            Debug.Log("Closing door...");
            StopAllCoroutines();
            StartCoroutine(RotateDoor(closedRot));
            isOpen = false;
        }
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        Debug.Log($"Starting door rotation to: {target}");
        // Smoothly slerp door rotation towards target
        while (Quaternion.Angle(doorObject.transform.localRotation, target) > 0.1f)
        {
            doorObject.transform.localRotation = Quaternion.Slerp(
                doorObject.transform.localRotation,
                target,
                Time.deltaTime * openSpeed);
            yield return null;
        }
        doorObject.transform.localRotation = target;
        Debug.Log($"Door rotation complete. Final rotation: {doorObject.transform.localRotation}");
    }
} 