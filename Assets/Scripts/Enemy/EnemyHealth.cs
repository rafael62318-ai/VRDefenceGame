using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Enemy))]
public class EnemyHealth : MonoBehaviour
{
    private int maxHP;
    private int defense;
    private int dropGold = 4;
    private int currentHP;

    [Header("UI 설정")]
    [Tooltip("적 머리 위에 표시될 체력 UI 프리팹")]
    public GameObject healthUiPrefab;
    [Tooltip("체력 UI가 표시될 Y축 높이")]
    public float healthUiYOffset = 2f;

    [Header("골드 드롭")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject goldTextPrefab;

    public event Action<int, int> OnHealthChanged;

    private Enemy enemy;
    private Coroutine slowCoroutine;
    private float baseSpeed; // 적의 '진짜' 원래 속도를 저장할 변수

    public void Initialize(int hp, int def)
    {
        maxHP = hp;
        defense = def;
        currentHP = maxHP;
    }

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    void Start()
    {
        if (enemy != null)
        {
            baseSpeed = enemy.gameObject.GetComponent<Enemy>().MoveSpeed;
        }


        if (healthUiPrefab != null)
            {
                GameObject healthUiObject = Instantiate(healthUiPrefab, transform.position + new Vector3(0, healthUiYOffset, 0), Quaternion.identity, transform);
                healthUiObject.GetComponent<EnemyHPUI>()?.Initialize(this);
            }
        
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    
    public int GetCurrentHP()
    {
        return currentHP;
    }
    
    public void TakeDamage(int dmg)
    {
        if (currentHP <= 0) return;

        int finalDamage = Mathf.Max(1, dmg - defense);
        currentHP -= finalDamage;
        
        Debug.Log($"{gameObject.name}이(가) 피해를 입었습니다. 현재 체력: {currentHP} / {maxHP}");
        
        OnHealthChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    public void ApplySlow(float slowFactor, float duration)
    {
        if (enemy == null) return;

        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowProcess(slowFactor, duration));
    }
    
    private IEnumerator SlowProcess(float slowFactor, float duration)
    {
    
        // 현재 속도가 아닌, 처음에 저장해둔 baseSpeed를 기준으로 속도를 감소시킵니다.
        enemy.MoveSpeed = baseSpeed * (1 - slowFactor);

        yield return new WaitForSeconds(duration);

        // 둔화가 끝나면 현재 속도가 아닌, 원래 속도(baseSpeed)로 되돌립니다.
        enemy.MoveSpeed = baseSpeed;
     
        
        slowCoroutine = null;
    }
    
    private void Die()
    {
        Debug.Log(gameObject.name + "의 체력이 0이 되어 파괴됩니다.");
        if (ResourceManager.Instance != null && dropGold > 0)
            ResourceManager.Instance.AddGold(dropGold);

        if (coinPrefab != null)
            Instantiate(coinPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        if (goldTextPrefab != null)
        {
            var obj = Instantiate(goldTextPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);
            obj.GetComponent<FloatingText>()?.SetText($"+{dropGold}");
        }

        Destroy(gameObject);
    }
}