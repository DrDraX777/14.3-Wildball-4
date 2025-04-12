using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicPusher : MonoBehaviour
{
    [Header("��������� ����� (�������)")]
    [Tooltip("�������� ��������, ������������ � ������ ��� �����. ������� �������! ������� � ������� �������� (50-500+)")]
    public float hitImpulse = 100f; // ������� ��� ��� �������, �� ������ �������� pushForce
    public string targetTag = "Player"; // ��� �������, ������� ����� �����������

    [Header("����� �����������")]
    [Tooltip("������������ ������� �������� ��� ����������� ����������� (������������� ��� ������� �����)")]
    public bool useContactNormal = true;
    [Tooltip("���� �� ���������� �������, ������� �� ������ ����� ������� (����� ����������� ��� �����)")]
    public bool pushAwayFromCenter = false; // ������ ��� ����� ����� ������������ �������

    private Rigidbody kinematicRb;

    void Start()
    {
        kinematicRb = GetComponent<Rigidbody>();
        if (!kinematicRb.isKinematic)
        {
            Debug.LogWarning("������ KinematicPusher �������� �� �� �������������� Rigidbody! " + gameObject.name);
        }
    }

    // ����������� ���� ��� � ������ ������ ������������
    void OnCollisionEnter(Collision collision)
    {
        // ��������� ��� �������������� �������
        if (collision.gameObject.CompareTag(targetTag))
        {
            // �������� Rigidbody ������
            Rigidbody targetRb = collision.rigidbody;

            // ���� � ������ ���� Rigidbody � �� �� ��������������
            if (targetRb != null && !targetRb.isKinematic)
            {
                // ���������� ����������� �����
                Vector3 hitDirection;

                if (useContactNormal && collision.contactCount > 0)
                {
                    // ������� �������� - ��������� ����������� ��� "������" �� �����
                    // ��� ���������� �� ����������� ����� � ������
                    hitDirection = collision.GetContact(0).normal;

                    // �����: ���� ����� ����� ������, � ����� �������,
                    // ������� ����� ���� ����� ��������� ����������� �������� �����.
                    // ��� ���������.
                }
                else if (pushAwayFromCenter)
                {
                    // ������������: �� ������ ����� � ������ ������
                    hitDirection = (collision.transform.position - transform.position).normalized;
                }
                else
                {
                    // ���� �� ������� �����������, ������ �� ������
                    Debug.LogWarning("����������� ����� �� ���������� � KinematicPusher.", this);
                    return;
                }

                // --- ���������� �������� ---
                if (hitDirection != Vector3.zero)
                {
                    // ��������� ���������� ������� � ������
                    targetRb.AddForce(hitDirection * hitImpulse, ForceMode.Impulse);

                    Debug.Log($"Applied Impulse! Target: {collision.gameObject.name}, Impulse: {hitImpulse}, Direction: {hitDirection}"); // ��� �������
                }
            }
        }
    }
}
