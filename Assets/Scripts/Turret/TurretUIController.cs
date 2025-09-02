using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TurretUIController : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    [SerializeField] private UnitPlacer placer; // 터렛 설치 로직을 담당하는 컴포넌트
    [SerializeField] private GameObject upgradePanel;

    [Header("VR 상호작용 설정")]
    [Tooltip("터렛 설치/UI 조작에 사용할 VR 컨트롤러의 Ray Interactor")]
    [SerializeField] private XRRayInteractor rayInteractor;

    // 현재 선택되어 업그레이드/판매 등을 할 수 있는 터렛
    private Upgradeable selectedTurret;
    private GameObject turretPrefabToPlace;

    void Start()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
    }

    public void PrepareToPlaceTurret(GameObject turretPrefab)
    {
        turretPrefabToPlace = turretPrefab;
        Debug.Log($"{turretPrefab.name} 설치 준비 완료. 지면을 가리켜주세요.");
    }

    public void ClearPlacement()
    {
        turretPrefabToPlace = null;
    }

    void Update()
    {
        if (turretPrefabToPlace != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            if (rayInteractor.xrController is ActionBasedController controller)
            {
                // 'Select' 액션(보통 트리거)이 '이번 프레임에 처음으로 눌렸는지' 확인합니다.
                if (controller.selectAction.action.WasPressedThisFrame())
                {
                    PlaceTurret(hit.point);
                }
            }
        }
    }

    /// <summary>
    /// UI 버튼에 연결하여 특정 터렛의 설치를 시도하는 함수
    /// </summary>
    /// <param name="turretPrefab">설치할 터렛의 프리팹</param>
    public void PlaceTurret(Vector3 position)
    {
        if (placer == null) return;
        
        bool placed = placer.TryPlaceAtPoint(turretPrefabToPlace, position);

        if (placed)
        {
            Debug.Log($"{turretPrefabToPlace.name} 포탑 설치 성공");
            ClearPlacement();
        }
        else
        {
            Debug.Log("포탑 설치 실패 (골드 부족 or 설치 불가 위치)");
        }
    }

    /// <summary>
    /// 이미 설치된 터렛을 클릭했을 때 호출되는 함수
    /// </summary>
    public void SelectTurret(Upgradeable turret)
    {
        selectedTurret = turret;

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }
    }

    /// <summary>
    /// 업그레이드 UI 버튼에 연결되는 함수
    /// </summary>
    public void UpgradeSelectedTurret()
    {
        if (selectedTurret != null)
        {
            selectedTurret.TryUpgrade();
        }
    }
}