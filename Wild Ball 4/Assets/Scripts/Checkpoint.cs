// Checkpoint.cs
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("����� ����� ��������, ������� ����� ������������ ��� ����������� ����� ���������. �������� ������, ���� ��� ������ ������� ������� ��� ��������� ��������.")]
    public Transform newRespawnPoint;

    [Tooltip("��������� �������� ����� ������� ������������?")]
    public bool deactivateOnTrigger = true;

    [Header("������������ (������ ��� ���������)")]
    [Tooltip("���� ����� ��� ����� �������� � ���������")]
    public Color gizmoColor = new Color(0, 1, 0, 0.5f); // �������, ��������������
    [Tooltip("������ ����� ����� ��� ����� ��������")]
    public float gizmoRadius = 0.5f;

    private bool _isActivated = false;

    public bool IsActivated => _isActivated; // �������� ������ ��� ������

    void OnTriggerEnter(Collider other)
    {
        if (_isActivated && deactivateOnTrigger) return; // ���� ��� ����������� � ������ �����������

        if (other.CompareTag("Player"))
        {
            PlayerInteractionHandler playerInteraction = other.GetComponent<PlayerInteractionHandler>();
            if (playerInteraction != null)
            {
                if (newRespawnPoint != null)
                {
                    playerInteraction.SetNewRespawnPoint(newRespawnPoint.position);
                    Debug.Log($"�������� '{gameObject.name}' �����������. ����� ����� ��������: {newRespawnPoint.position}");
                }
                else
                {
                    // ���� newRespawnPoint �� ��������, ����� ������ ���������� ��� ������� ������ �������
                    Debug.Log($"�������� '{gameObject.name}' �������, �� ����� ����� �������� �� �������.");
                }

                _isActivated = true;

                // �����������: �������������� ��� ��������� ���������, ����� �� �� ���������� ��������
                if (deactivateOnTrigger)
                {
                    // GetComponent<Collider>().enabled = false; // ����� ���, ��� �������������� ���� ������
                    // gameObject.SetActive(false); // �������������� ���� ������ ���������
                }
                // ����� ����� �������� ����������/�������� ������ ������������ ���������
            }
        }
    }

    // ������ ����� � ��������� ��� �������� ���������
    void OnDrawGizmos()
    {
        if (newRespawnPoint != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(newRespawnPoint.position, gizmoRadius);
            Gizmos.DrawLine(transform.position, newRespawnPoint.position); // ����� �� ��������� � ����� ��������
        }
    }

    // ����� ������������ OnDrawGizmosSelected, ����� ����� ��������� ������ ��� ��������� ���������
    // void OnDrawGizmosSelected()
    // {
    //     if (newRespawnPoint != null)
    //     {
    //         Gizmos.color = gizmoColor;
    //         Gizmos.DrawSphere(newRespawnPoint.position, gizmoRadius);
    //     }
    // }
}