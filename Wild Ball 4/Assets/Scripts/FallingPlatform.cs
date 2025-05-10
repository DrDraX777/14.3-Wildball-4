using UnityEngine;
using System.Collections; // Необходимо для использования корутин (IEnumerator)

// Добавляем требование иметь компонент Rigidbody на том же объекте,
// чтобы избежать ошибок, если его забудут добавить вручную.
[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("Настройки Падения")]
    [Tooltip("Тег объекта игрока, который активирует падение")]
    public string playerTag = "Player";

    [Tooltip("Время вибрации перед началом падения (в секундах)")]
    public float vibrationDuration = 0.5f;

    [Tooltip("Сила (амплитуда) вибрации - насколько сильно платформа смещается")]
    public float vibrationMagnitude = 0.05f;

    [Tooltip("Задержка перед уничтожением объекта после начала падения (в секундах)")]
    public float destroyDelay = 2.0f;

    // --- Внутренние переменные ---
    private Rigidbody rb;                  // Ссылка на компонент Rigidbody
    private Vector3 originalPosition;      // Исходная позиция платформы для вибрации
    private bool isTriggered = false;      // Флаг, была ли платформа уже активирована
    private Coroutine vibrationCoroutine = null; // Ссылка на активную корутину вибрации

    // Вызывается один раз при инициализации объекта
    void Awake()
    {
        // Получаем компонент Rigidbody
        rb = GetComponent<Rigidbody>();

        // Проверяем, найден ли Rigidbody (хотя RequireComponent должен это гарантировать)
        if (rb == null)
        {
            Debug.LogError($"На объекте {gameObject.name} отсутствует компонент Rigidbody!", this);
            enabled = false; // Отключаем скрипт, если нет Rigidbody
            return;
        }

        // Сохраняем начальную позицию платформы
        originalPosition = transform.position;

        // Убедимся, что платформа изначально статична и не подвержена гравитации.
        // Мы сделаем ее kinematic, чтобы вибрация (изменение transform.position)
        // работала корректно и не конфликтовала с физикой до начала падения.
        rb.isKinematic = true;
        rb.useGravity = false; // Гравитация выключена, пока платформа kinematic
    }

    // Вызывается, когда другой Collider входит в контакт с Collider'ом этого объекта
    void OnCollisionEnter(Collision collision)
    {
        // Проверяем:
        // 1. Не была ли платформа уже активирована (isTriggered == false)
        // 2. Столкнулся ли объект с нужным тегом (playerTag)
        if (!isTriggered && collision.gameObject.CompareTag(playerTag))
        {
            // Дополнительная проверка: Убедимся, что игрок наступил СВЕРХУ.
            // Это предотвратит срабатывание, если игрок ударится сбоку или снизу.
            // Мы проверяем нормаль точки контакта. Если она направлена в основном вниз,
            // значит, столкновение произошло сверху платформы.
            ContactPoint contact = collision.GetContact(0); // Берем первую точку контакта
            // Vector3.down - это (0, -1, 0). Скалярное произведение > 0 означает, что угол < 90 градусов.
            // Используем небольшое пороговое значение (например, 0.7), чтобы быть уверенными,
            // что контакт произошел достаточно близко к вертикальному направлению сверху.
            if (Vector3.Dot(contact.normal, Vector3.down) > 0.7f)
            {
                Debug.Log($"Платформа {gameObject.name} активирована игроком {collision.gameObject.name}");
                isTriggered = true; // Устанавливаем флаг, чтобы предотвратить повторное срабатывание

                // Запускаем корутину, которая выполнит всю последовательность действий
                StartCoroutine(TriggerFallingSequence());
            }
        }
    }

    // Корутина, управляющая последовательностью: вибрация -> падение -> уничтожение
    private IEnumerator TriggerFallingSequence()
    {
        // --- Фаза 1: Вибрация ---
        // Запускаем корутину вибрации и ждем ее завершения
        vibrationCoroutine = StartCoroutine(VibratePlatform());
        yield return vibrationCoroutine; // Ждем, пока VibratePlatform() не закончится
        vibrationCoroutine = null; // Сбрасываем ссылку на корутину

        // --- Фаза 2: Падение ---
        // Делаем платформу НЕ kinematic, чтобы на нее действовала физика (гравитация)
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true; // Включаем гравитацию
        }
        Debug.Log($"Платформа {gameObject.name} начала падать.");

        // --- Фаза 3: Уничтожение ---
        // Запланировать уничтожение этого игрового объекта через destroyDelay секунд
        // после начала падения.
        Destroy(gameObject, destroyDelay);
    }

    // Корутина для эффекта вибрации
    private IEnumerator VibratePlatform()
    {
        float elapsedTime = 0f; // Счетчик прошедшего времени вибрации
        Debug.Log($"Платформа {gameObject.name} вибрирует...");

        // Цикл выполняется, пока не пройдет время vibrationDuration
        while (elapsedTime < vibrationDuration)
        {
            // Генерируем случайное небольшое смещение по горизонтали (оси X и Z)
            float offsetX = Random.Range(-1f, 1f) * vibrationMagnitude;
            float offsetY = 0f; // Не вибрируем по вертикали
            float offsetZ = Random.Range(-1f, 1f) * vibrationMagnitude;

            // Применяем смещение к ИСХОДНОЙ позиции, чтобы платформа не "уезжала" далеко
            transform.position = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            // Увеличиваем счетчик времени
            elapsedTime += Time.deltaTime;

            // Уступаем управление до следующего кадра
            yield return null;
        }

        // После завершения цикла вибрации возвращаем платформу точно в исходное положение.
        // Это важно, чтобы падение началось из правильной точки.
        transform.position = originalPosition;
        Debug.Log($"Платформа {gameObject.name} закончила вибрировать.");
    }

    // Опционально: Если объект отключается или уничтожается до завершения вибрации,
    // останавливаем корутину и возвращаем позицию.
    void OnDisable()
    {
        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
            transform.position = originalPosition; // Вернуть на место на всякий случай
            vibrationCoroutine = null;
        }
    }
}