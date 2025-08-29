using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    public int health = 100; // 적의 초기 체력

    // 데미지를 입는 함수
    public void Dmg(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + "이(가) " + damage + "만큼의 데미지를 입었습니다. 남은 체력: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    // 적이 파괴되는 함수
    void Die()
    {
        Debug.Log(gameObject.name + "이(가) 파괴되었습니다.");
        Destroy(gameObject); // 이 스크립트가 붙은 오브젝트를 파괴
    }
}