using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KinematicPusher : MonoBehaviour
{
    [Header("Настройки Удара (Импульс)")]
    [Tooltip("Величина импульса, применяемого к шарику при ударе. Требует подбора! Начните с высоких значений (50-500+)")]
    public float hitImpulse = 100f; // Изменил имя для ясности, но можешь оставить pushForce
    public string targetTag = "Player"; // Тег объекта, который нужно отталкивать

    [Header("Опции Направления")]
    [Tooltip("Использовать нормаль контакта для определения направления (рекомендуется для эффекта удара)")]
    public bool useContactNormal = true;
    [Tooltip("Если не используем нормаль, толкать от центра этого объекта (менее реалистично для удара)")]
    public bool pushAwayFromCenter = false; // Обычно для удара лучше использовать нормаль

    private Rigidbody kinematicRb;

    void Start()
    {
        kinematicRb = GetComponent<Rigidbody>();
        if (!kinematicRb.isKinematic)
        {
            Debug.LogWarning("Скрипт KinematicPusher работает на НЕ кинематическом Rigidbody! " + gameObject.name);
        }
    }

    // Срабатывает один раз в момент начала столкновения
    void OnCollisionEnter(Collision collision)
    {
        // Проверяем тег столкнувшегося объекта
        if (collision.gameObject.CompareTag(targetTag))
        {
            // Получаем Rigidbody шарика
            Rigidbody targetRb = collision.rigidbody;

            // Если у шарика есть Rigidbody и он не кинематический
            if (targetRb != null && !targetRb.isKinematic)
            {
                // Определяем направление удара
                Vector3 hitDirection;

                if (useContactNormal && collision.contactCount > 0)
                {
                    // Нормаль контакта - идеальное направление для "отдачи" от удара
                    // Она направлена ОТ поверхности балки к шарику
                    hitDirection = collision.GetContact(0).normal;

                    // Важно: Если шарик очень легкий, а балка быстрая,
                    // нормаль может быть почти идентична направлению движения балки.
                    // Это нормально.
                }
                else if (pushAwayFromCenter)
                {
                    // Альтернатива: от центра балки к центру шарика
                    hitDirection = (collision.transform.position - transform.position).normalized;
                }
                else
                {
                    // Если не выбрано направление, ничего не делаем
                    Debug.LogWarning("Направление удара не определено в KinematicPusher.", this);
                    return;
                }

                // --- Применение ИМПУЛЬСА ---
                if (hitDirection != Vector3.zero)
                {
                    // Применяем мгновенный импульс к шарику
                    targetRb.AddForce(hitDirection * hitImpulse, ForceMode.Impulse);

                    Debug.Log($"Applied Impulse! Target: {collision.gameObject.name}, Impulse: {hitImpulse}, Direction: {hitDirection}"); // Для отладки
                }
            }
        }
    }
}
