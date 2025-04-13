using UnityEngine;
using TMPro; // или UnityEngine.UI

public class InvisHint : MonoBehaviour
{
    [Header("UI Подсказка")]
    [Tooltip("UI элемент (GameObject) для подсказки взаимодействия")]
    public GameObject interactionPromptUI; // Сюда перетащим наш Text/Image GameObject

    // УБРАЛИ: Ссылку на целевой объект отсюда, т.к. связь идет в другую сторону
    // [Header("Целевой Объект")]
    // [Tooltip("Объект, столкновение с которым отключает подсказку")]
    // public GameObject targetObjectToMonitor; // Это поле больше не нужно здесь

    private bool targetActivated = false; // Флаг: был ли активирован целевой объект?

    void Start()
    {
        // Скрыть подсказку при старте
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Interaction Prompt UI не назначен в инспекторе!", this);
        }
    }

    // Срабатывает, когда другой коллайдер ВХОДИТ в триггерную зону ЭТОГО объекта
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег игрока
        if (other.CompareTag("Player"))
        {
            // Показываем подсказку ТОЛЬКО ЕСЛИ целевой объект ЕЩЕ НЕ БЫЛ активирован
            if (!targetActivated && interactionPromptUI != null)
            {
                Debug.Log("Игрок вошел в зону подсказки, цель не активирована - показываем подсказку.");
                interactionPromptUI.SetActive(true);
            }
            else if (targetActivated)
            {
                Debug.Log("Игрок вошел в зону подсказки, но цель уже активирована - подсказка не нужна.");
            }
        }
    }

    // Срабатывает, когда другой коллайдер ВЫХОДИТ из триггерной зоны ЭТОГО объекта
    private void OnTriggerExit(Collider other)
    {
        // Проверяем тег игрока
        if (other.CompareTag("Player"))
        {
            // Всегда скрываем подсказку при выходе, если она была видна
            if (interactionPromptUI != null && interactionPromptUI.activeSelf)
            {
                Debug.Log("Игрок вышел из зоны подсказки - скрываем подсказку.");
                interactionPromptUI.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Публичный метод, который будет вызван из TargetObjectCollider,
    /// когда игрок столкнется с целевым объектом.
    /// </summary>
    public void NotifyTargetActivated()
    {
        Debug.Log($"InvisHint получил уведомление: Цель ({this.name}'s target) была активирована.");
        targetActivated = true;

        // Важно: Если игрок все еще находится в триггерной зоне В МОМЕНТ активации цели,
        // нужно немедленно скрыть подсказку.
        if (interactionPromptUI != null && interactionPromptUI.activeSelf)
        {
            Debug.Log("Цель активирована, пока игрок в зоне - немедленно скрываем подсказку.");
            interactionPromptUI.SetActive(false);
        }
    }
}
