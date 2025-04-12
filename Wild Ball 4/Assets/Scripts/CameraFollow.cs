using UnityEngine;

public class CameraFollow : MonoBehaviour // ���� ������ ����� �������� �� Main Camera
{
    public Transform target; // ���� ���������� ������ ������ (BouncingBall)

    // ��������� ������ ������ ����� ��������� �� �����.
    // ������� �������� = ����� �������/��������� ����������.
    public float smoothSpeed = 0.125f;

    // �������� ������ ������������ ����.
    // ��� ���������� � ���� ����� ������������� ��������� ��� ������.
    public Vector3 offset;

    // --- �����������: ����������� �������� ������ ---
    // public bool limitBounds = false;
    // public Vector3 minCameraPos;
    // public Vector3 maxCameraPos;
    // ---------------------------------------------

    void Start()
    {
        // ���������, ��������� �� ����
        if (target == null)
        {
            Debug.LogError("���� (target) ��� ������ �� ���������!");
            enabled = false; // ��������� ������, ���� ���� ���
            return;
        }

        // ��������� ��������� �������� ������ ������������ ����.
        // �����: ��������� ������ � ��������� ���, ��� �� ������ ������ ����
        // ����� ��������, ��� �������� ����� ���������.
        offset = transform.position - target.position;
    }

    // LateUpdate ���������� ����� ���� Update � FixedUpdate.
    // ��� ��������� ����� ��� ���������� ������, ��� ��� �����
    // ��� ������ ��� ��������� ���� �������� � ���� �����.
    void LateUpdate()
    {
        // ���� ���� ������� (��������, ����������), �� ��������� ���
        if (target == null) return;

        // ������������ �������� ������� ������: ������� ���� + ��������
        Vector3 desiredPosition = target.position + offset;

        // --- �����������: ���������� ����������� ---
        // if (limitBounds)
        // {
        //     desiredPosition.x = Mathf.Clamp(desiredPosition.x, minCameraPos.x, maxCameraPos.x);
        //     desiredPosition.y = Mathf.Clamp(desiredPosition.y, minCameraPos.y, maxCameraPos.y);
        //     desiredPosition.z = Mathf.Clamp(desiredPosition.z, minCameraPos.z, maxCameraPos.z);
        // }
        // -----------------------------------------

        // ������ ���������� ������ �� ������� ������� � ��������
        // Vector3.Lerp - �������� ������������ ����� ����� �������
        // ������ �������� (t) ����������, ��������� ������ � B �� ������������ �� A.
        // ������������� smoothSpeed * Time.deltaTime ������������ ���������,
        // �� ��������� (��� ����� �� ���������) �� ������� ������.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // ��������� ����������� ������� � ������
        transform.position = smoothedPosition;

        // �����������: ������ �������� �� ���� (��������������, ���� �����)
        // ��� ����� ���� �������, ���� ���� ���������, �� ������ ������ ������
        // �������� � � �����, ��������� ���������� ������������ ���� ������.
        // ��� �������� ���� ��� ������ �� �����, ���� offset ��������� ���������.
        // transform.LookAt(target);
    }
}
