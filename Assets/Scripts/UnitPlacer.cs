using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField] private LayerMask placeableLayers;
    [SerializeField] private float maxPlaceDistance = 20f;

    /// <summary>
    /// 지정된 위치에 터렛 설치를 시도합니다.
    /// </summary>
    /// <param name="turretPrefab">설치할 터렛의 프리팹</param>
    /// <param name="rayOrigin">레이캐스트 시작 위치</param>
    /// <param name="rayDir">레이캐스트 방향</param>
    /// <returns>설치 성공 여부</returns>
    public bool TryPlaceAtPoint(GameObject turretPrefab, Vector3 point)
    {
        // 1. 프리팹이 유효한지 확인
        if (turretPrefab == null) return false;

        // 2. 프리팹에서 비용 정보를 가져옴
        var costComponent = turretPrefab.GetComponent<PurchaseCost>();
        if (costComponent == null) return false;
        int buildCost = costComponent.buildCost;
        

        // 3. 비용을 지불할 수 있는지 확인
        if (ResourceManager.Instance == null || !ResourceManager.Instance.CanAfford(buildCost))
        {
            return false;
        }
            
        
        // 5. 터렛 생성 및 비용 차감
        Instantiate(turretPrefab, point, Quaternion.identity);
        ResourceManager.Instance.TrySpend(buildCost);
        return true;
    }


}