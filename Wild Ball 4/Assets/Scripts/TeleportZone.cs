// TeleportZone.cs
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    // public GameObject linkedTeleport; // Эта переменная не используется в текущей логике TeleportManager
    private TeleportManager teleportManager;

    void Start()
    {
        teleportManager = FindObjectOfType<TeleportManager>();
        if (teleportManager == null)
        {
            Debug.LogError($"TeleportZone на объекте {gameObject.name} не смог найти TeleportManager в сцене!", this);
            enabled = false; // Отключаем, если нет менеджера
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