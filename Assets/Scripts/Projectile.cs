using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 10;
    public Transform target;

    void Update()
    {
        // 타겟이 존재할 경우에만 이동
        if (target != null)
        {
            // 타겟을 향하는 방향 벡터를 계산
            Vector3 direction = (target.position - transform.position).normalized;
            // 해당 방향으로 이동
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        else
        {
            // 타겟이 사라지면 총알도 파괴
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
        EnemyHp enemyHp = other.GetComponent<EnemyHp>();
        if (enemyHp != null)
        {
            enemyHp.Dmg(damage);
        }
        Debug.Log(target.name + "에게 " + damage + " 데미지를 입혔습니다!");
        Destroy(gameObject);
        }
    }
}