using UnityEngine;
using System.Collections; // 코루틴(시간차 공격)을 사용하기 위해 필요합니다.

public class Enemy : MonoBehaviour
{
    // public으로 선언되어 인스펙터 창에서 선택할 수 있는 적의 종류 목록입니다.
    public enum EnemyType { Model, Rocket, Big, Boss }

    [Header("적 타입 설정")]
    [Tooltip("이 프리팹이 어떤 종류의 적인지 선택해주세요.")]
    public EnemyType enemyType;

    // private 변수들은 SetStats() 함수를 통해 내부적으로만 값이 결정됩니다.
    private float moveSpeed;    // 이동 속도
    private int maxHP;          // 최대 체력
    private int attackDamage;   // 공격력
    private int defense;        // 방어력
    private int dropGold;       // 드롭 골드

    // 외부에서 속도를 제어할 수 있도록 public 프로퍼티(통로)를 만듭니다. (주로 둔화 효과용)
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    private EnemyHealth enemyHealth; // 같은 오브젝트에 있는 EnemyHealth 스크립트를 담을 변수
    private bool hasReachedEnd = false; // 경로 끝에 도달했는지 확인하는 스위치

    [Header("경로 설정")]
    [Tooltip("적이 따라갈 웨이포인트(경로)들")]
    [HideInInspector]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    // 게임 오브젝트가 생성될 때 가장 먼저 실행되는 함수입니다.
    void Awake()
    {
        // 1. 자신의 타입에 맞는 모든 능력치를 설정합니다.
        SetStats();
        
        // 2. 같은 오브젝트에 붙어있는 체력 담당 스크립트(EnemyHealth)를 찾습니다.
        enemyHealth = GetComponent<EnemyHealth>();
        
        // 3. 체력 담당 스크립트에게 방금 설정한 능력치 정보를 넘겨줍니다.
        enemyHealth.Initialize(maxHP, defense, dropGold);
    }

    // Update 함수는 매 프레임마다 계속 실행됩니다.
    void Update()
    {
        // 만약 경로 끝에 도달했다면, 더 이상 아래의 이동 코드를 실행하지 않습니다.
        if (hasReachedEnd) 
            return;

        // 마지막 웨이포인트까지 통과했다면
        if (waypoints == null || currentWaypointIndex >= waypoints.Length)
        {
            hasReachedEnd = true; // 도달 스위치를 켭니다.
            StartCoroutine(AttackBase()); // 본체 공격 코루틴을 시작합니다.
            return; // 이동 로직을 중단합니다.
        }

        // 현재 목표 웨이포인트를 향해 이동합니다.
        // 현재 속도인 MoveSpeed를 사용합니다. (둔화 효과가 적용될 수 있음)
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, MoveSpeed * Time.deltaTime);

        // 현재 목표 웨이포인트에 거의 도착했다면
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            // 다음 웨이포인트를 목표로 설정합니다.
            currentWaypointIndex++;
            if (currentWaypointIndex < waypoints.Length)
            {
                transform.LookAt(waypoints[currentWaypointIndex]); // 다음 지점을 바라봅니다.
            }
        }
    }

    // 적 타입(enemyType)에 따라 능력치를 설정하는 함수입니다.
    void SetStats()
    {
        switch (enemyType)
        {
            case EnemyType.Model:
                maxHP = 100;
                moveSpeed = 5f;
                attackDamage = 10;
                defense = 0;
                dropGold = 4;
                break;
            case EnemyType.Rocket:
                maxHP = 75;
                moveSpeed = 8f;
                attackDamage = 5;
                defense = 0;
                dropGold = 3;
                break;
            case EnemyType.Big:
                maxHP = 300;
                moveSpeed = 3f;
                attackDamage = 20;
                defense = 5;
                dropGold = 10;
                break;
            case EnemyType.Boss:
                maxHP = 1000;
                moveSpeed = 2f;
                attackDamage = 50;
                defense = 10;
                dropGold = 50;
                break;
        }
    }

    // 본체를 반복적으로 공격하는 코루틴 함수입니다.
    IEnumerator AttackBase()
    {
        // 이 루프는 적이 파괴되기 전까지 무한 반복됩니다.
        while (true)
        {
            // MainBaseHealth 스크립트가 씬 어딘가에 존재한다면
            if (MainBaseHealth.Instance != null)
            {
                // 자신의 공격력만큼 본체에 피해를 입힙니다.
                MainBaseHealth.Instance.TakeDamage(attackDamage);
            }
            
            // 1초 동안 기다렸다가 다시 루프를 실행합니다.
            yield return new WaitForSeconds(1f);
        }
    }
}