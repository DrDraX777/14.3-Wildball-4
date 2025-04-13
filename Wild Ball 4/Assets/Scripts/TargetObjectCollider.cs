using UnityEngine;

[RequireComponent(typeof(Collider))] // ����������� ������� ����������
[RequireComponent(typeof(MeshRenderer))] // ����������� ������� ���������
public class TargetObjectCollider : MonoBehaviour
{
    [Tooltip("������ �� ������ InvisHint, ������� ����� ���������")]
    public InvisHint hintController; // ���� � ���������� ��������� ������ � InvisHint

    [Tooltip("��� �������, ������� ���������� ���� ������ (�����)")]
    public string playerTag = "Player";

    private MeshRenderer meshRenderer;
    private bool alreadyTriggered = false;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // �����: ��������, ��� ������ ���������� �������
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("MeshRenderer �� ������ �� �������!", this);
        }

        if (hintController == null)
        {
            Debug.LogError("������ 'Hint Controller' (InvisHint) �� ��������� � ����������!", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���������, �� ���� �� ��� ������������ � ��� ������
        if (!alreadyTriggered && collision.gameObject.CompareTag(playerTag))
        {
            alreadyTriggered = true;
            Debug.Log($"����� ���������� � {gameObject.name}. ������ ������� � ���������� InvisHint.");

            // 1. �������� MeshRenderer ����� �������
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }

            // 2. ���������� ������ InvisHint, ��� ���� ���� ������������
            if (hintController != null)
            {
                hintController.NotifyTargetActivated(); // �������� ��������� ����� � InvisHint
            }
            else
            {
                Debug.LogWarning("�� ������� ��������� InvisHint, �.�. ������ �� ���������.", this);
            }

            // �����������: ����� ��������� ��������� ����� ���������,
            // ���� ������ �� ����� ����������� ������������ � ���
            // Collider col = GetComponent<Collider>();
            // if (col != null) col.enabled = false;
        }
    }
}
