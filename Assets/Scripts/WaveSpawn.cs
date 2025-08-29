using UnityEngine;
using System.Collections;

public class WaveSpawn : MonoBehaviour
{
    // 한 웨이브에 생성할 적의 수
    public int waveSize = 5;

    // 생성할 적 유닛 프리팹
    public GameObject enemyPrefab;

    // 적 생성 간격
    public float enemyInterval = 2f;

    // 적이 생성될 위치 (웨이포인트 0)
    public Transform spawnPoint;

    // 적 유닛이 따라갈 웨이포인트 배열
    public Transform[] waypoints;

    // 게임 시작 후 첫 웨이브가 시작될 시간
    public float startTime = 1f;

    // 현재까지 생성된 적의 수
    private int enemyCount = 0;

    void Start()
    {
        // InvokeRepeating을 사용하여 startTime 이후 enemyInterval마다 SpawnEnemy 함수 반복 호출
        InvokeRepeating("SpawnEnemy", startTime, enemyInterval);
    }

    void Update()
    {
        // 생성된 적의 수가 웨이브 크기와 같아지면 적 생성을 멈춤
        if (enemyCount >= waveSize)
        {
            CancelInvoke("SpawnEnemy");
        }
    }

    void SpawnEnemy()
    {
        // 적 수 증가
        enemyCount++;
        
        // 적 유닛 생성
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity) as GameObject;

        // 생성된 적에게 웨이포인트 정보 전달
        enemy.GetComponent<Enemy>().waypoints = waypoints;
    }
}