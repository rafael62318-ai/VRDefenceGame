using UnityEngine;

public class CursorControl : MonoBehaviour
{
    bool isCursorVisible = false; // ���콺 ���ü� ���� ����

    void Start()
    {
        HideCursor(); // ���� ���� �� ���콺 �����
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // ESC Ű�� ������ ���콺 ���ü� ��ȯ
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
        Cursor.lockState = CursorLockMode.Locked; // ���콺�� ȭ�� �߾����� ����
        Cursor.visible = false; // ���콺 �����
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None; // ���콺 ���� ����
        Cursor.visible = true; // ���콺 ǥ��
    }
}