using UnityEngine;

public class TurretTrigger : MonoBehaviour
{
    // The main turret controller script
    public TurretController turretController;

    // The currently locked-on target
    private Transform currentTarget;

    void OnTriggerEnter(Collider other)
    {
        // Check if a target is already acquired
        if (currentTarget != null)
        {
            return;
        }

        // If the entering object has the correct tag, lock onto it
        if (other.CompareTag(turretController.targetTag))
        {
            turretController.target = other.transform;
            currentTarget = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // If the object leaving the trigger is our current target, release the lock
        if (other.transform == currentTarget)
        {
            turretController.target = null;
            currentTarget = null;
        }
    }

    void Update()
    {
        // If the current target no longer exists (e.g., destroyed or removed)
        if (currentTarget == null)
        {
            turretController.target = null;
        }
    }
}