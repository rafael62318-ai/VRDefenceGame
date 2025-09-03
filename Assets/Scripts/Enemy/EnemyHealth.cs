using UnityEngine;
using System.Collections;
using System;

// 이 컴포넌트는 Enemy 스크립트가 반드시 필요함을 명시합니다.
[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    // --- 능력치 변수 (Enemy.cs로부터 값을 전달받음) ---
    private int maxHP;
    private int currentHP;
    private int defense;
    private int dropGold;

    // --- 인스펙터에서 설정하는 변수들 ---
    [Header("UI 설정")]
    [Tooltip("적 머리 위에 표시될 체력 UI 프리팹")]
    public GameObject healthUiPrefab;
    [Tooltip("체력 UI가 표시될 Y축 높이")]
    public float healthUiYOffset = 2f;

    [Header("골드 드롭 효과")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject goldTextPrefab;

    // --- 이벤트 ---
    // 체력이 변경될 때마다 외부(주로 EnemyHPUI)에 알려주는 신호
    public event Action<int, int> OnHealthChanged;

    // --- 내부 참조 변수 ---
    private Enemy enemy;
    private Coroutine slowCoroutine;
    private float baseSpeed; // 둔화 효과 계산을 위한 원래 속도 저장 변수

    /// <summary>
    /// Enemy.cs가 호출하여 이 적의 핵심 능력치를 설정해주는 함수입니다.
    /// </summary>
    /// <param name="hp">최대 체력</param>
    /// <param name="def">방어력</param>
    /// <param name="gold">사망 시 드롭할 골드</param>
    public void Initialize(int hp, int def, int gold)
    {
        maxHP = hp;
        defense = def;
        dropGold = gold;
        currentHP = maxHP;
    }

    // 컴포넌트 참조는 Awake에서 처리하는 것이 안전합니다.
    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    // 다른 컴포넌트들의 Awake가 끝난 후 초기화를 진행합니다.
    void Start()
    {
        // Enemy.cs의 Awake에서 moveSpeed가 설정된 후, 그 값을 baseSpeed로 저장합니다.
        if (enemy != null && enemy.TryGetComponent<Enemy>(out var enemyComponent))
        {
            baseSpeed = enemyComponent.MoveSpeed;
        }

        // 체력 바 UI를 생성합니다.
        if (healthUiPrefab != null)
        {
            GameObject healthUiObject = Instantiate(healthUiPrefab, transform.position + new Vector3(0, healthUiYOffset, 0), Quaternion.identity, transform);
            healthUiObject.GetComponent<EnemyHPUI>()?.Initialize(this);
        }
        
        // UI에 현재 체력을 처음으로 표시하기 위해 이벤트를 호출합니다.
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    
    /// <summary>
    /// 현재 체력을 반환하는 함수입니다.
    /// </summary>
    public int GetCurrentHP()
    {
        return currentHP;
    }
    
    /// <summary>
    /// 외부로부터 피해를 받는 함수입니다.
    /// </summary>
    /// <param name="dmg">받은 피해량</param>
    public void TakeDamage(int dmg)
    {
        if (currentHP <= 0) return;

        // 방어력을 적용하여 최종 피해량을 계산합니다. (최소 1의 피해는 받음)
        int finalDamage = Mathf.Max(1, dmg - defense);
        currentHP -= finalDamage;
        
        // 디버깅을 위해 콘솔에 현재 체력을 출력합니다.
        Debug.Log($"{gameObject.name}이(가) {finalDamage}의 피해를 입었습니다. 현재 체력: {currentHP} / {maxHP}");
        
        // 체력 바 UI를 업데이트하기 위해 이벤트를 호출합니다.
        OnHealthChanged?.Invoke(currentHP, maxHP);

        // 체력이 0 이하로 떨어지면 죽음 처리 함수를 호출합니다.
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 둔화(슬로우) 효과를 적용하는 함수입니다.
    /// </summary>
    /// <param name="slowFactor">감속 비율 (0.0 ~ 1.0)</param>
    /// <param name="duration">지속 시간(초)</param>
    public void ApplySlow(float slowFactor, float duration)
    {
        if (enemy == null) return;

        // 이전에 적용된 둔화 효과가 있다면 중지하고 새로 시작합니다.
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowProcess(slowFactor, duration));
    }
    
    // 둔화 효과를 실제로 처리하는 코루틴입니다.
    private IEnumerator SlowProcess(float slowFactor, float duration)
    {
        // 원래 속도(baseSpeed)를 기준으로 속도를 감소시킵니다.
        enemy.MoveSpeed = baseSpeed * (1 - slowFactor);

        // 지정된 시간만큼 기다립니다.
        yield return new WaitForSeconds(duration);

        // 지속 시간이 끝나면 원래 속도로 되돌립니다.
        enemy.MoveSpeed = baseSpeed;
        
        slowCoroutine = null;
    }
    
    // 적이 죽었을 때 호출되는 함수입니다.
    private void Die()
    {
        Debug.Log(gameObject.name + "의 체력이 0이 되어 파괴됩니다.");

        // 리소스 매니저에 골드를 추가합니다.
        if (ResourceManager.Instance != null && dropGold > 0)
            ResourceManager.Instance.AddGold(dropGold);

        // 골드 획득 관련 시각 효과를 생성합니다.
        if (coinPrefab != null)
            Instantiate(coinPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        if (goldTextPrefab != null)
        {
            var obj = Instantiate(goldTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            obj.GetComponent<FloatingText>()?.SetText($"+{dropGold}");
        }

        // 자기 자신을 씬에서 파괴합니다.
        Destroy(gameObject);
    }
}