using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; // 총알의 속도
    public int damage = 10; // 총알이 입히는 데미지
    public Transform target; // 총알이 추적할 타겟

    void Update()
    {
        if (target != null)
        {
            // 타겟을 향해 이동
            Vector3 direction = target.position - transform.position;
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // 타겟이 사라지면 총알도 파괴
            Destroy(gameObject);
        }
    }

    // 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 타겟과 동일한지, 그리고 'Enemy' 태그를 가졌는지 확인
        if (other.transform == target)
        {
            // 'EnemyHp' 스크립트를 찾아 데미지 입히기
            EnemyHp enemyHp = other.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.Dmg(damage);
            }
            
            Destroy(gameObject); // 총알 파괴
        }
    }
}