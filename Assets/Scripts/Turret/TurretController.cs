using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{
    // 타겟팅 및 회전 관련 변수
    public Transform lookAtObj; // 회전할 오브젝트
    public string targetTag = "Enemy";
    public float rotationSpeed = 5f;
    public float range = 10f;
    public Transform target; // 현재 타겟
    private float homeY; // 초기 Y축 회전값

    // 발사 관련 변수
    public GameObject projectilePrefab;
    public Transform shootElement; // 발사 위치
    public float fireRate = 1f;
    private bool isShooting; // 발사 중인지 확인

    // 추가 기능 관련 변수
    public bool isCatcherType = false; // Catcher 타입 여부
    public Animator anim; // 애니메이터 컴포넌트
    public int damage = 10;

    // 코루틴 딜레이 계산용
    private float shootDelay;

    void Start()
    {
        if (lookAtObj != null)
            homeY = lookAtObj.localRotation.eulerAngles.y;

        if (isCatcherType && anim == null)
            anim = GetComponent<Animator>();

        // 0으로 나눔 방지
        shootDelay = 1f / Mathf.Max(0.0001f, fireRate);
    }

    void Update()
    {
        // 1) 타겟 탐색
        FindTarget();

        // 2) 타겟 있을 때
        if (target != null)
        {
            AimAtTarget();

            if (!isShooting)
            {
                if (!isCatcherType)
                    StartCoroutine(ShootCoroutine());
                else
                    StartCoroutine(CatcherAttackCoroutine());
            }
        }
        // 3) 타겟 없을 때
        else
        {
            ReturnToHomeRotation();
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.transform;
                }
            }
        }
        target = closestTarget;
    }

    void AimAtTarget()
    {
        if (lookAtObj == null || target == null) return;

        Vector3 direction = target.position - lookAtObj.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookAtObj.rotation = Quaternion.Slerp(lookAtObj.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void ReturnToHomeRotation()
    {
        if (lookAtObj == null) return;

        Quaternion home = Quaternion.Euler(
            lookAtObj.localRotation.eulerAngles.x,
            homeY,
            lookAtObj.localRotation.eulerAngles.z
        );
        lookAtObj.rotation = Quaternion.Slerp(lookAtObj.rotation, home, Time.deltaTime * rotationSpeed);
    }

    // ★중복 제거·정리된 일반 발사 코루틴
    IEnumerator ShootCoroutine()
    {
        isShooting = true;

        while (target != null)
        {
            // 총알 생성
            if (projectilePrefab != null && shootElement != null)
            {
                GameObject newBullet = Instantiate(projectilePrefab, shootElement.position, shootElement.rotation);

                var bulletScript = newBullet.GetComponent<Projectile>();
                if (bulletScript != null)
                {
                    bulletScript.target = target;
                    bulletScript.damage = damage;
                }
            }

            yield return new WaitForSeconds(shootDelay);
        }

        isShooting = false;
    }

    // Catcher 타입 공격 코루틴
    IEnumerator CatcherAttackCoroutine()
    {
        isShooting = true;

        while (target != null)
        {
            if (anim != null)
            {
                anim.SetBool("Attack", true);
                // 현재 레이어 길이만큼 대기(애니메이션 길이 확보가 안되면 한 프레임 대기로 대체됨)
                float wait = anim.GetCurrentAnimatorStateInfo(0).length;
                if (wait <= 0f) wait = Time.deltaTime;
                yield return new WaitForSeconds(wait);
                anim.SetBool("Attack", false);
            }

            // 데미지 처리
            if (target != null)
            {
                var enemyHp = target.GetComponent<EnemyHp>();
                if (enemyHp != null)
                    enemyHp.Dmg(damage);
            }

            yield return new WaitForSeconds(shootDelay);
        }

        isShooting = false;

        // 초기 애니메이션 복귀
        if (anim != null)
        {
            anim.SetBool("Attack", false);
            anim.SetBool("T_pose", true);
        }
    }

    // 개발 편의를 위한 시각화
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
