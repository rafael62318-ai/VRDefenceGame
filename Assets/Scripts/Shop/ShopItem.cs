using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class ShopItem : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TurretDefinition turret;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPointForCarry; // 비워두면 손 위치/자기 위치 사용

    [Header("UI (Optional)")]
    [SerializeField] private ShopHoverUI hoverUI;              // 상점 패널(이름/가격/보유골드/상태)
    [SerializeField] private GameObject toastNotEnoughGold;     // "골드 부족" 토스트(기본 비활성)

    [Header("SFX (Optional)")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip buyClip;
    [SerializeField] private AudioClip denyClip;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

        // Hover 시 UI On/Off
        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);

        // 선택(집기) 시 결제 → 운반용 생성
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDestroy()
    {
        if (!interactable) return;
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnEnable()
    {
        // 초기 UI 갱신
        if (hoverUI != null) hoverUI.Bind(turret);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (hoverUI != null)
        {
            hoverUI.Bind(turret);
            hoverUI.Show();
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (hoverUI != null) hoverUI.Hide();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // 결제 시도
        if (!ResourceManager.Instance.TrySpend(turret.price))
        {
            // 실패 피드백
            if (sfx && denyClip) sfx.PlayOneShot(denyClip);
            SendHaptic(args.interactorObject, 0.4f, 0.15f);
            ShowToastOnce(toastNotEnoughGold, 1.0f);

            // UI 재갱신(상태 빨간색 등)
            if (hoverUI != null) hoverUI.Refresh();
            return;
        }

        // 성공 피드백
        if (sfx && buyClip) sfx.PlayOneShot(buyClip);

        // 운반용 프리팹 생성 + 손에 바로 쥐어주기
        var interactor = args.interactorObject as UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor;

        Transform hand = null;
        if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor direct) hand = direct.transform;
        else if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor ray) hand = ray.transform;

        Vector3 pos = spawnPointForCarry ? spawnPointForCarry.position :
                      hand ? hand.position : (transform.position + Vector3.up * 0.1f);
        Quaternion rot = spawnPointForCarry ? spawnPointForCarry.rotation :
                         hand ? hand.rotation : Quaternion.identity;

        var proxy = Instantiate(turret.carriedPrefab, pos, rot);
        var carried = proxy.GetComponent<CarriedTurret>();
        if (carried == null) carried = proxy.AddComponent<CarriedTurret>();
        carried.InitFromShop(this, turret); // 환불/설치 연동

        // 손에 강제 그랩 (Unity 6 최신 시그니처)
        var grab = proxy.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null && interactor != null && grab.interactionManager != null)
        {
            grab.interactionManager.SelectEnter(
                interactor as UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor,
                grab as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable
            );
        }

        // Hover UI 최신화(보유골드 감소 반영)
        if (hoverUI != null) hoverUI.Refresh();
    }

    public void Refund()
    {
        // 환불은 Add/AddGold 중 편한 것 사용 (ResourceManager 최종본에 Add 래퍼 있음)
        ResourceManager.Instance.Add(turret.price);
        if (hoverUI != null) hoverUI.Refresh();
    }

    private async void ShowToastOnce(GameObject go, float secs)
    {
        if (!go) return;
        go.SetActive(true);
        await System.Threading.Tasks.Task.Delay(Mathf.RoundToInt(secs * 1000));
        if (go) go.SetActive(false);
    }

    private void SendHaptic(UnityEngine.XR.Interaction.Toolkit.Interactors.IXRInteractor interactor, float amplitude, float duration)
    {
        try
        {
            if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor di && di.xrController != null)
                di.xrController.SendHapticImpulse(amplitude, duration);
            else if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor ri && ri.xrController != null)
                ri.xrController.SendHapticImpulse(amplitude, duration);
        }
        catch { /* XRI 버전에 따라 미지원일 수 있음 */ }
    }
}
