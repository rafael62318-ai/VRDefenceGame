using UnityEngine;

public class UnitPlacer : MonoBehaviour
{
    [SerializeField] private LayerMask placeableLayers;
    [SerializeField] private float maxPlaceDistance = 20f;

    // 설치 시도: 레이캐스트 히트 지점에 프리팹 배치
    public bool TryPlaceAt(GameObject unitPrefab, Vector3 rayOrigin, Vector3 rayDir)
    {
        if (unitPrefab == null) return false;

        var cost = unitPrefab.GetComponent<PurchaseCost>();
        int buildCost = cost != null ? cost.GetBuildCost() : 0;

        if (ResourceManager.Instance == null || !ResourceManager.Instance.CanAfford(buildCost))
            return false;

        if (Physics.Raycast(rayOrigin, rayDir, out var hit, maxPlaceDistance, placeableLayers))
        {
            // 필요 시 스냅/정렬 로직 추가
            var obj = Instantiate(unitPrefab, hit.point, Quaternion.identity);

            // 비용 차감
            ResourceManager.Instance.TrySpend(buildCost);
            return true;
        }
        return false;
    }
}
