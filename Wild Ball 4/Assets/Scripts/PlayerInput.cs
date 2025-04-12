using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // ��������� �������� (read-only) ��� �������� �������� ����� ������ ��������
    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    // ����� �������� ������ �����, ��������, ��� ������, �������������� � �.�.
    // public bool JumpInputDown { get; private set; }
    // public bool InteractInputDown { get; private set; }

    void Update()
    {
        // ������ ��� ��������
        HorizontalInput = Input.GetAxis("Horizontal"); // A/D ��� ������� �����/������
        VerticalInput = Input.GetAxis("Vertical");     // W/S ��� ������� �����/����

        // ������ ������ ������ (���� ����� ��� ������� �������)
        // JumpInputDown = Input.GetButtonDown("Jump"); // ������ ������
        // InteractInputDown = Input.GetKeyDown(KeyCode.E);
    }
}
