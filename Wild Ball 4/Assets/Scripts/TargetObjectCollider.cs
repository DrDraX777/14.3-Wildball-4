using UnityEngine;

[RequireComponent(typeof(Collider))] // Гарантируем наличие коллайдера
[RequireComponent(typeof(MeshRenderer))] // Гарантируем наличие рендерера
public class TargetObjectCollider : MonoBehaviour
{
    [Tooltip("Ссылка на скрипт InvisHint, который нужно уведомить")]
    public InvisHint hintController; // Сюда в инспекторе перетащим объект с InvisHint

    [Tooltip("Тег объекта, который активирует этот объект (игрок)")]
    public string playerTag = "Player";

    private MeshRenderer meshRenderer;
    private bool alreadyTriggered = false;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // Важно: Убедимся, что объект изначально НЕВИДИМ
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("MeshRenderer не найден на объекте!", this);
        }

        if (hintController == null)
        {
            Debug.LogError("Ссылка 'Hint Controller' (InvisHint) не назначена в инспекторе!", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, не было ли уже столкновения и тег игрока
        if (!alreadyTriggered && collision.gameObject.CompareTag(playerTag))
        {
            alreadyTriggered = true;
            Debug.Log($"Игрок столкнулся с {gameObject.name}. Делаем видимым и уведомляем InvisHint.");

            // 1. Включаем MeshRenderer этого объекта
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }

            // 2. Уведомляем скрипт InvisHint, что цель была активирована
            if (hintController != null)
            {
                hintController.NotifyTargetActivated(); // Вызываем публичный метод у InvisHint
            }
            else
            {
                Debug.LogWarning("Не удалось уведомить InvisHint, т.к. ссылка не назначена.", this);
            }

            // Опционально: можно отключить коллайдер после активации,
            // если больше не нужно отслеживать столкновения с ним
            // Collider col = GetComponent<Collider>();
            // if (col != null) col.enabled = false;
        }
    }
}
