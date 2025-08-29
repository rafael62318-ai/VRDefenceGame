using UnityEngine;

public class TurretUIController : MonoBehaviour
{
    [SerializeField] private UnitPlacer placer;        // 씬에 있는 UnitPlacer 연결
    [SerializeField] private GameObject turretPrefab;  // 설치할 포탑 프리팹
    private Upgradeable selectedTurret;                // 현재 선택된 포탑

    // 버튼: 포탑 설치
    public void PlaceTurret()
    {
        if (placer != null && turretPrefab != null)
        {
            bool placed = placer.TryPlaceAt(
                turretPrefab,
                Camera.main.transform.position,
                Camera.main.transform.forward
            );
            Debug.Log(placed ? "포탑 설치 성공" : "설치 실패 (골드 부족 or 위치 불가)");
        }
    }

    // 포탑 선택
    public void SelectTurret(Upgradeable turret)
    {
        selectedTurret = turret;
        Debug.Log($"[TurretUI] 포탑 선택됨: {turret.gameObject.name}");
    }

    // 버튼: 포탑 업그레이드
    public void UpgradeTurret()
    {
        if (selectedTurret != null)
        {
            bool upgraded = selectedTurret.TryUpgrade();
            Debug.Log(upgraded ? "업그레이드 성공" : "업그레이드 실패 (골드 부족 or 최대 레벨)");
        }
        else
        {
            Debug.Log("업그레이드할 포탑이 선택되지 않았습니다.");
        }
    }
}
