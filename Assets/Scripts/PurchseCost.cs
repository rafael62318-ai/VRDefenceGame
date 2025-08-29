using UnityEngine;

[DisallowMultipleComponent]
public class PurchaseCost : MonoBehaviour
{
    [Header("설치 비용")]
    [SerializeField, Min(0)] private int buildCost = 8;

    [Header("업그레이드 비용 (단계별 누적)")]
    [SerializeField, Min(0)] private int[] upgradeCosts = new int[] { 10, 15, 20 };

    public int CurrentLevel { get; private set; } = 0;
    public int MaxLevel => upgradeCosts != null ? upgradeCosts.Length : 0;

    public int GetBuildCost() => buildCost;

    public bool CanUpgrade() => CurrentLevel < MaxLevel;

    public int GetNextUpgradeCost()
    {
        if (!CanUpgrade()) return -1;
        return upgradeCosts[CurrentLevel];
    }

    public bool TryIncreaseLevel()
    {
        if (!CanUpgrade()) return false;
        CurrentLevel++;
        return true;
    }
}
