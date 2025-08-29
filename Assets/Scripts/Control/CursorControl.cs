using UnityEngine;

public class CursorControl : MonoBehaviour
{
    bool isCursorVisible = false; // 마우스 가시성 상태 저장

    void Start()
    {
        HideCursor(); // 게임 시작 시 마우스 숨기기
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC 키를 눌러서 마우스 가시성 전환
        {
            isCursorVisible = !isCursorVisible;

            if (isCursorVisible)
            {
                ShowCursor();
            }
            else
            {
                HideCursor();
            }
        }
    }

    void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스를 화면 중앙으로 고정
        Cursor.visible = false; // 마우스 숨기기
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None; // 마우스 고정 해제
        Cursor.visible = true; // 마우스 표시
    }
}