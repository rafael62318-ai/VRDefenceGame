using UnityEngine;
using UnityEngine.InputSystem; // 새로운 유니티 입력 시스템을 사용하기 위해 필요합니다.

public class ControllerHandsManager : MonoBehaviour
{
    // 인스펙터에서 Input Action Reference를 할당할 공개 변수입니다.
    // 이 참조들은 Input Action Asset에 정의된 특정 액션(예: "트리거 누르기")에 연결됩니다.
    public InputActionReference triggerActionReference;
    public InputActionReference gripActionReference;

    // 손 모델에 있는 Animator 컴포넌트의 참조입니다.
    // 이 Animator는 입력에 따라 손 애니메이션을 위한 블렌드 트리 또는 상태를 제어합니다.
    public Animator handAnimator;

    private void Awake()
    {
        // 이 스크립트가 붙어 있는 GameObject에 있는 Animator 컴포넌트를 가져옵니다.
        // 일반적으로 손 모델과 Animator를 포함하는 GameObject입니다.
        handAnimator = GetComponent<Animator>();

        // 입력 액션 구독을 설정하는 메서드를 호출합니다.
        SetupInputActions();
    }

    /// <summary>
    /// 트리거 및 그립 입력 액션에 대한 리스너를 설정합니다.
    /// 이 액션들이 수행될 때, 손 애니메이션을 업데이트합니다.
    /// </summary>
    void SetupInputActions()
    {
        // 두 입력 액션 참조가 인스펙터에 할당되었는지 확인합니다.
        if (triggerActionReference != null && gripActionReference != null)
        {
            // 트리거 액션의 'performed' 이벤트에 구독합니다.
            // 트리거가 눌리거나 해제될 때 (액션 수행 시), UpdateHandAnimation을 호출합니다.
            // 'ctx.ReadValue<float>()'는 액션의 현재 float 값(예: 트리거 당김 시 0에서 1)을 가져옵니다.
            // performed 이벤트: 이러한 입력 액션이 완전히 완료되거나, 특정 기준을 충족했을 때 한 번 발생합니다.
            triggerActionReference.action.performed += ctx => UpdateHandAnimation("Trigger", ctx.ReadValue<float>());

            // 그립 액션의 'performed' 이벤트에 구독합니다.
            // 트리거와 유사하게, "Grip" 애니메이션 파라미터를 업데이트합니다.
            triggerActionReference.action.canceled += ctx => UpdateHandAnimation("Trigger", 0);
            
            // 지속적인 액션의 경우, 입력이 해제될 때 값을 명시적으로 재설정해야 한다면
            // 'canceled' 이벤트에도 구독할 수 있습니다.
            gripActionReference.action.performed += ctx => UpdateHandAnimation("Grip", ctx.ReadValue<float>());
            gripActionReference.action.canceled += ctx => UpdateHandAnimation("Grip", 0);
        }
        else
        {
            // 입력 액션 참조가 설정되지 않았다면 경고를 기록합니다.
            // 이렇게 되면 손 애니메이션이 작동하지 않습니다.
            Debug.LogWarning("Input Action References are not set in the Inspector");
        }
    }

    /// <summary>
    /// 손 Animator의 float 파라미터를 업데이트합니다.
    /// </summary>
    /// <param name="parameterName">Animator의 float 파라미터 이름 (예: "Trigger", "Grip").</param>
    /// <param name="value">파라미터에 설정할 float 값 (예: 0.0에서 1.0).</param>
    void UpdateHandAnimation(string parameterName, float value)
    {
        // Animator 컴포넌트가 유효한지 확인한 후 파라미터를 설정하려고 시도합니다.
        if (handAnimator != null)
        {
            // Animator의 float 파라미터를 설정합니다.
            // 이 파라미터는 일반적으로 Animator Controller의 블렌드 트리의 일부여야 합니다.
            // 이 블렌드 트리는 다양한 손 포즈(예: 열린 손, 부분적으로 닫힌 손, 완전히 닫힌 손)를 블렌딩합니다.
            handAnimator.SetFloat(parameterName, value);
        }
    }

    // --- 입력 액션 관리를 위한 생명 주기 메서드 ---
    // 오류를 방지하고 성능을 최적화하기 위해 입력 액션을 올바르게 활성화 및 비활성화하는 것이 중요합니다.
    private void OnEnable()
    {
        // 이 GameObject가 활성화될 때 액션을 활성화합니다.
        // 이렇게 하면 입력 감지를 시작합니다.
        /*
        "?" 연산자는 Null 조건부 연산자입니다.
        if (triggerActionReference != null)
        {
            triggerActionReference.action.Enable();
        }
        위 내용을 줄여서 쓰면 아래와 같습니다.
        */
        triggerActionReference?.action.Enable();
        gripActionReference?.action.Enable();
    }

    private void OnDisable()
    {
        // 이 GameObject가 비활성화되거나 파괴될 때 액션을 비활성화합니다.
        // 이렇게 하면 입력 감지를 중지하고 메모리 누수를 방지합니다.
        triggerActionReference?.action.Disable();
        gripActionReference?.action.Disable();
    }
}
