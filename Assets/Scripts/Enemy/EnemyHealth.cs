using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int maxHP = 100;   // 최대 체력
    private int currentHP;

    [Header("골드 드롭")]
    [SerializeField] private int dropGold = 4;
    [SerializeField] private GameObject coinPrefab;      // 코인 프리팹
    [SerializeField] private GameObject goldTextPrefab;  // World Space Canvas (TMP Text 포함)

    void Awake()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 데미지를 입었을 때 호출
    /// </summary>
    public void TakeDamage(int dmg)
    {
        if (currentHP <= 0) return;

        currentHP -= dmg;
        Debug.Log($"{gameObject.name}이(가) {dmg} 만큼 피해를 입음. 남은 HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 적 사망 처리
    /// </summary>
    void Die()
    {
        Debug.Log($"{gameObject.name}이(가) 파괴되었습니다.");

        // 골드 획득 처리
        if (ResourceManager.Instance != null && dropGold > 0)
            ResourceManager.Instance.AddGold(dropGold);

        // 코인 드롭
        if (coinPrefab != null)
            Instantiate(coinPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        // +골드 텍스트 드롭
        if (goldTextPrefab != null)
        {
            var obj = Instantiate(goldTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            var floating = obj.GetComponent<FloatingText>();
            if (floating != null)
                floating.SetText($"+{dropGold}");
        }

        Destroy(gameObject);
    }
}
