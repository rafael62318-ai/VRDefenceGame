using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField, Min(0)] private int startGold = 0;   // 시작 골드
    public int Gold { get; private set; }                 // 현재 골드

    public event Action<int> OnGoldChanged;   // HUD 업데이트용 이벤트
    public event Action<int> OnGoldGained;    // 골드 획득 이벤트 (옵션)
    public event Action<int> OnGoldSpent;     // 골드 소비 이벤트 (옵션)

    [Header("Persist Across Scenes?")]
    [SerializeField] private bool dontDestroyOnLoad = true;

public bool CanAfford(int amount)
{
    return amount <= Gold;
}
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        Gold = Mathf.Max(0, startGold);
        OnGoldChanged?.Invoke(Gold);  // HUD 초기화
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        Gold += amount;
        Debug.Log($"[ResourceManager] 골드 획득: {amount}, 총합: {Gold}");

        OnGoldGained?.Invoke(amount);
        OnGoldChanged?.Invoke(Gold);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || amount > Gold) return false;

        Gold -= amount;
        Debug.Log($"[ResourceManager] 골드 소비: {amount}, 총합: {Gold}");

        OnGoldSpent?.Invoke(amount);
        OnGoldChanged?.Invoke(Gold);
        return true;
    }
}
