// Checkpoint.cs
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Новая точка респауна, которая будет активирована при прохождении этого чекпоинта. Оставьте пустым, если это просто триггер события без изменения респауна.")]
    public Transform newRespawnPoint;

    [Tooltip("Отключить чекпоинт после первого срабатывания?")]
    public bool deactivateOnTrigger = true;

    [Header("Визуализация (Только для редактора)")]
    [Tooltip("Цвет гизмо для точки респауна в редакторе")]
    public Color gizmoColor = new Color(0, 1, 0, 0.5f); // Зеленый, полупрозрачный
    [Tooltip("Размер сферы гизмо для точки респауна")]
    public float gizmoRadius = 0.5f;

    private bool _isActivated = false;

    public bool IsActivated => _isActivated; // Свойство только для чтения

    void OnTriggerEnter(Collider other)
    {
        if (_isActivated && deactivateOnTrigger) return; // Если уже активирован и должен отключаться

        if (other.CompareTag("Player"))
        {
            PlayerInteractionHandler playerInteraction = other.GetComponent<PlayerInteractionHandler>();
            if (playerInteraction != null)
            {
                if (newRespawnPoint != null)
                {
                    playerInteraction.SetNewRespawnPoint(newRespawnPoint.position);
                    Debug.Log($"Чекпоинт '{gameObject.name}' активирован. Новая точка респауна: {newRespawnPoint.position}");
                }
                else
                {
                    // Если newRespawnPoint не назначен, можно просто логировать или вызвать другое событие
                    Debug.Log($"Чекпоинт '{gameObject.name}' пройден, но новая точка респауна не указана.");
                }

                _isActivated = true;

                // Опционально: деактивировать сам коллайдер чекпоинта, чтобы он не срабатывал повторно
                if (deactivateOnTrigger)
                {
                    // GetComponent<Collider>().enabled = false; // Можно так, или деактивировать весь объект
                    // gameObject.SetActive(false); // Деактивировать весь объект чекпоинта
                }
                // Здесь можно добавить визуальный/звуковой эффект срабатывания чекпоинта
            }
        }
    }

    // Рисуем гизмо в редакторе для удобства настройки
    void OnDrawGizmos()
    {
        if (newRespawnPoint != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(newRespawnPoint.position, gizmoRadius);
            Gizmos.DrawLine(transform.position, newRespawnPoint.position); // Линия от чекпоинта к точке респауна
        }
    }

    // Можно использовать OnDrawGizmosSelected, чтобы гизмо рисовался только при выделении чекпоинта
    // void OnDrawGizmosSelected()
    // {
    //     if (newRespawnPoint != null)
    //     {
    //         Gizmos.color = gizmoColor;
    //         Gizmos.DrawSphere(newRespawnPoint.position, gizmoRadius);
    //     }
    // }
}