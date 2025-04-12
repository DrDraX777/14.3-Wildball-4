using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    [Tooltip("������ �������, ������� ����� ������ ��� �������")]
    public GameObject pickupEffectPrefab;

    [Tooltip("��� �������, ������� ����� ��������� ����� (�����)")]
    public string playerTag = "Player";

    private bool isPickedUp = false;

    void Start()
    {
        if (pickupEffectPrefab == null)
        {
            Debug.LogError("������ ������� (pickupEffectPrefab) �� ��������!", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPickedUp && other.CompareTag(playerTag))
        {
            // ���������� ������� ������ ������, � �� ������, ��� �������
            Pickup(transform.position);
        }
    }

    // ��������� �������, ��� ����� ������� ������
    private void Pickup(Vector3 pickupPosition)
    {
        Debug.Log("����� " + gameObject.name + " ��������! ���������� ��������...");
        isPickedUp = true;

        // 1. ������� ������ �� ������� � ����� ������
        if (pickupEffectPrefab != null)
        {
            Instantiate(pickupEffectPrefab, pickupPosition, Quaternion.identity);
            // �������, ��� � ������� ���� ParticleSystem � PlayOnAwake=true � StopAction=Destroy
        }

        // 2. ���������� ������������ ������ ������
        if (transform.parent != null) // ���������, ���� �� ��������
        {
            Destroy(transform.parent.gameObject); // ���������� GameObject ��������
            // �����: ��� ����������� ��������, ��� ��� �������� ������� (������� ����)
            // ����� ����� ���������� �������������.
        }
        else
        {
            // ���� �������� ���, ���������� ��� �������� ������
            Debug.LogWarning("� ������� " + gameObject.name + " ��� ��������. ���������� ��� ������.", this);
            Destroy(gameObject);
        }
    }
}
