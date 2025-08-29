using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // 이동 속도 설정
    public float moveSpeed = 5f;

    // 외부에서 웨이포인트 배열을 받을 변수
    public Transform[] waypoints;

    // 현재 목표 웨이포인트의 인덱스
    private int currentWaypointIndex = 0;

    void Start()
    {
        // 웨이포인트가 없으면 경고 메시지 출력
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("웨이포인트가 할당되지 않았습니다!");
            return;
        }

        // 첫 번째 웨이포인트로 방향 설정
        transform.LookAt(waypoints[currentWaypointIndex]);
    }

    void Update()
    {
        // 웨이포인트 배열의 마지막에 도달하면
        if (currentWaypointIndex >= waypoints.Length)
        {
            // 목표(본진)에 도착한 것으로 간주
            // 예: 본진 체력 감소 로직 호출
            Destroy(gameObject); // 적 유닛 제거
            return;
        }

        // 현재 웨이포인트로 이동
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);

        // 목표 웨이포인트에 충분히 가까워지면
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            // 다음 웨이포인트로 목표 변경
            currentWaypointIndex++;

            // 다음 웨이포인트가 있으면 그쪽으로 방향 설정
            if (currentWaypointIndex < waypoints.Length)
            {
                transform.LookAt(waypoints[currentWaypointIndex]);
            }
        }
    }
}