using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{   
    //인스펙터 창에서 선택할 수 있는 적의 종류 목록
    public enum EnemyType { Model, Rocket, Big, Boss }

    [Header("적 타입 설정")]
    public EnemyType enemyType;

    //SetStats() 함수를 통해 내부적으로 값이 결정되는 변수들
     public float MoveSpeed
    {
        get { return moveSpeed; } // moveSpeed 값을 읽어갈 때 사용
        set { moveSpeed = value; } // moveSpeed 값을 변경할 때 사용
    }
    private float moveSpeed;
    private int maxHP;
    private int attackDamage;
    private int defense;

    private EnemyHealth enemyHealth;
    private bool hasReachedEnd = false; //경로 끝에 도달했는지 확인하는 스위치

    // 적이 따라갈 경로
    [HideInInspector]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    //게임오브젝트가 생성될 때 가장 먼저 실행되는 함수
    void Awake()
    {
        //자신의 타입에 맞는 능력치 설정
        SetStats();
        //같은 오브젝트에 붙어있는 체력 담당 스크립트 찾기
        enemyHealth = GetComponent<EnemyHealth>();
        //체력 스크립트에게 최대 체력과 방어력 정보 넘겨주기
        enemyHealth.Initialize(maxHP, defense);
    }

    //매 프레임마다 계속 실행
    void Update()
    {
        //경로 끝에 도달 시 이동 코드 실행 끝
        if (hasReachedEnd) return;

        //마지막 웨이포인트까지 통과했다면 
        if (waypoints == null || currentWaypointIndex >= waypoints.Length)
        {
            hasReachedEnd = true;//도달 스위치를 킴
            Debug.Log(gameObject.name + "이(가) 경로 끝에 도달하여 AttackBase 코루틴을 시작합니다!");
            StartCoroutine(AttackBase());//본체 공격 코루틴 시작
            GetComponent<Collider>().enabled = false; //다른 오브젝트와 충돌하지 않도록 비활성화
            return;//이동로직 중단
        }
        //현재 목표 웨이포인트를 향해 이동
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);
        //현재 목표 웨이포인트에 거의 도착했다면
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {   
            //다음 웨이포인트를 목표로 설정함
            currentWaypointIndex++;
            if (currentWaypointIndex < waypoints.Length)
            {
                transform.LookAt(waypoints[currentWaypointIndex]); //다음 지점을 바라봄
            }
        }
    }

    //적 타입에 따라 능력치를 설정하는 함수
    void SetStats()
    {
        switch (enemyType)
        {
            case EnemyType.Model:
                maxHP = 100;
                moveSpeed = 5f;
                attackDamage = 10;
                defense = 0;
                break;
            case EnemyType.Rocket:
                maxHP = 75;
                moveSpeed = 8f;
                attackDamage = 5;
                defense = 0;
                break;
            case EnemyType.Big:
                maxHP = 300;
                moveSpeed = 3f;
                attackDamage = 20;
                defense = 5;
                break;
            case EnemyType.Boss:
                maxHP = 1000;
                moveSpeed = 2f;
                attackDamage = 50;
                defense = 10;
                break;
        }
    }

    //본체를 반복적으로 공격하는 코루틴 함수입니다.
    IEnumerator AttackBase()
    {   
        //이 루프는 적이 파괴되기 전까지 무한 반복된다.
        while (true)
        {
            //MainBaseHealth 스크립트가 씬 어딘가에 존재한다면
            if (MainBaseHealth.Instance != null)
            {
                Debug.Log(gameObject.name + "이(가) 본체를 공격!");
                //자신의 공격력만큼 본체에 피해를 입힌다.
                MainBaseHealth.Instance.TakeDamage(attackDamage);
            }
            //1초 동안 기다렸다가 다시 루프를 실행
            yield return new WaitForSeconds(1f);
        }
    }
}