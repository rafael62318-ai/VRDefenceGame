using UnityEngine;

[RequireComponent(typeof(Upgradeable))]
public class TurretSelector : MonoBehaviour
{
    private Upgradeable upgradeable;

    void Awake()
    {
        upgradeable = GetComponent<Upgradeable>();
    }

    void OnMouseDown()
    {
        var ui = FindObjectOfType<TurretUIController>();
        if (ui != null && upgradeable != null)
        {
            ui.SelectTurret(upgradeable);
        }
    }
}
