using UnityEngine;
using System.Collections; // Для корутины вращения
using TMPro; // или UnityEngine.UI

[RequireComponent(typeof(Collider))] // Убедимся, что есть коллайдер
public class RotatingPlate : MonoBehaviour
{
    [Header("Настройки Вращения")]
    [Tooltip("Скорость вращения (градусов в секунду)")]
    public float rotationSpeed = 180f;
    [Tooltip("Целевой угол вращения по Z (0, 90, 180, 270), к которому должна прийти плита")]
    [Range(0, 3)] // 0=0, 1=90, 2=180, 3=270
    public int targetRotationStep = 0; // ЗАДАТЬ В ИНСПЕКТОРЕ для каждой плиты! 0-3

    [Header("Ссылки")]
    [Tooltip("Контроллер ворот, который нужно уведомить")]
    public PuzzleGateOpener gateController; // Перетащить объект ворот сюда
    [Tooltip("UI элемент для подсказки 'Нажмите E'")]
    public GameObject interactPromptUI; // Перетащить UI текст/картинку сюда

    [Header("Взаимодействие")]
    [Tooltip("Тег объекта игрока")]
    public string playerTag = "Player";

    // Приватные переменные состояния
    private bool playerInRange = false;
    private bool isRotating = false;
    private int currentRotationStep = 0; // Текущая позиция (0=0, 1=90, 2=180, 3=270)
    private bool isCorrectlyRotated = false;
    private Quaternion initialRotation; // Запомним начальное вращение для расчета шагов

    void Start()
    {
        // Проверки ссылок
        if (gateController == null)
            Debug.LogError($"Плита '{gameObject.name}' не имеет ссылки на PuzzleGateOpener!", this);
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
        else
            Debug.LogWarning($"Плита '{gameObject.name}' не имеет ссылки на UI подсказку 'Нажмите Е'.", this);

        // Настройка коллайдера
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Коллайдер на плите '{gameObject.name}' не является триггером. Устанавливаю isTrigger = true.", this);
            col.isTrigger = true;
        }

        // Определяем начальный шаг по текущему вращению (упрощенно)
        initialRotation = transform.localRotation;
        // TODO: Если плиты изначально не на 0 градусов, нужно точнее определить currentRotationStep
        currentRotationStep = 0; // Пока предполагаем старт с 0 градусов

        CheckIfCorrect(); // Проверить начальное состояние
    }

    void Update()
    {
        // Проверяем нажатие E ТОЛЬКО если игрок рядом И плита не вращается сейчас
        if (playerInRange && !isRotating && Input.GetKeyDown(KeyCode.E))
        {
            StartRotation();
        }
    }

    // Когда игрок входит в триггер плиты
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            // Показываем подсказку "Нажмите Е", ТОЛЬКО если плита ЕЩЕ НЕ в правильном положении
            if (interactPromptUI != null && !isCorrectlyRotated)
                interactPromptUI.SetActive(true);
            Debug.Log($"Игрок вошел в зону плиты {gameObject.name}.");
        }
    }

    // Когда игрок выходит из триггера плиты
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // Скрываем подсказку "Нажмите Е"
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            Debug.Log($"Игрок вышел из зоны плиты {gameObject.name}.");
        }
    }

    // Запускает процесс вращения
    private void StartRotation()
    {
        if (isRotating) return; // Уже вращаемся

        isRotating = true;
        if (interactPromptUI != null) // Прячем подсказку во время вращения
            interactPromptUI.SetActive(false);

        // Вычисляем следующий шаг и целевой угол
        int nextStep = (currentRotationStep + 1) % 4; // Цикл 0 -> 1 -> 2 -> 3 -> 0
        float targetAngleZ = nextStep * 90.0f;

        // Создаем целевое вращение (относительно начального, если оно было не 0, ИЛИ просто по Z)
        // Проще работать с абсолютными углами 0, 90, 180, 270
        Quaternion targetRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, targetAngleZ);
        // Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, targetAngleZ); // Если вращаем относительно начального

        Debug.Log($"Плита {gameObject.name}: Вращаем с шага {currentRotationStep} на шаг {nextStep} (угол {targetAngleZ})");
        StartCoroutine(RotateCoroutine(targetRotation, nextStep));
    }

    // Корутина для плавного вращения
    IEnumerator RotateCoroutine(Quaternion targetLocalRotation, int nextStep)
    {
        Quaternion startLocalRotation = transform.localRotation;
        float time = 0;
        float duration = 90.0f / rotationSpeed; // Время на поворот на 90 градусов

        while (time < duration)
        {
            transform.localRotation = Quaternion.Slerp(startLocalRotation, targetLocalRotation, time / duration);
            time += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // Гарантируем точное конечное положение
        transform.localRotation = targetLocalRotation;
        currentRotationStep = nextStep;
        isRotating = false;

        // Проверяем, стала ли плита в правильное положение
        CheckIfCorrect();
        // Показать подсказку снова, если игрок все еще в зоне И плита все еще НЕ правильная
        if (playerInRange && interactPromptUI != null && !isCorrectlyRotated)
            interactPromptUI.SetActive(true);

        Debug.Log($"Плита {gameObject.name}: Вращение завершено на шаге {currentRotationStep}. Правильно: {isCorrectlyRotated}");
    }

    // Проверяет, находится ли плита в целевом положении и уведомляет контроллер ворот
    void CheckIfCorrect()
    {
        bool previouslyCorrect = isCorrectlyRotated;
        isCorrectlyRotated = (currentRotationStep == targetRotationStep);

        // Если состояние изменилось (была неправильной -> стала правильной, ИЛИ наоборот)
        if (isCorrectlyRotated != previouslyCorrect && gateController != null)
        {
            gateController.PlateStateChanged(isCorrectlyRotated);
        }

        // Если плита стала правильной, скрываем подсказку "Е" навсегда для этой плиты
        if (isCorrectlyRotated && interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
    }
}
