// TeleportZone.cs
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    // public GameObject linkedTeleport; // ��� ���������� �� ������������ � ������� ������ TeleportManager
    private TeleportManager teleportManager;

    void Start()
    {
        teleportManager = FindObjectOfType<TeleportManager>();
        if (teleportManager == null)
        {
            Debug.LogError($"TeleportZone �� ������� {gameObject.name} �� ���� ����� TeleportManager � �����!", this);
            enabled = false; // ���������, ���� ��� ���������
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (teleportManager != null)
            {
                teleportManager.OnPlayerEnteredZone(gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (teleportManager != null)
            {
                teleportManager.OnPlayerExitedZone(gameObject);
            }
        }
    }
}