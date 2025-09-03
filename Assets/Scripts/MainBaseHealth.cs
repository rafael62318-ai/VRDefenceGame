using UnityEngine;

public class MainBaseHealth : MonoBehaviour
{
    public static MainBaseHealth Instance { get; private set; }

    [Header("본체 설정")]
    [Tooltip("본체의 최대 체력")]
    [SerializeField] private int maxHP = 1000;
    private int currentHP;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHP = maxHP;
        Debug.Log($"본체 체력 : {currentHP} / {maxHP}");
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;

        currentHP -= damage;
        Debug.Log($"본체가 {damage} 피해를 입었습니다! 남은 체력 : {currentHP}");

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.LogError("게임 오버!");
        //여기에 게임 오버 UI를 띄우거나, 게임을 멈추는 로직 추가
        Time.timeScale = 0f; //시간을 멈춰서 게임을 정지시킴
    }
}
