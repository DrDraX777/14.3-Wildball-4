using UnityEngine;
using System.Collections;
using TMPro; // или UnityEngine.UI

[RequireComponent(typeof(Collider))]
public class RotatingPlate : MonoBehaviour
{
    [Header("Настройки Вращения")]
    [Tooltip("Скорость вращения (градусов в секунду)")]
    public float rotationSpeed = 180f;
    [Tooltip("Целевой шаг вращения по Z (0=0°, 1=90°, 2=180°, 3=270°)")]
    [Range(0, 3)]
    public int targetRotationStep = 0; // Установить в инспекторе!

    [Header("Ссылки")]
    [Tooltip("Контроллер ворот, который нужно уведомить")]
    public PuzzleGateOpener gateController;
    [Tooltip("UI элемент для подсказки 'Нажмите E'")]
    public GameObject interactPromptUI;

    [Header("Взаимодействие")]
    [Tooltip("Тег объекта игрока")]
    public string playerTag = "Player";

    // Состояние
    private bool playerInRange = false;
    private bool isRotating = false;
    private int currentRotationStep = 0;
    private bool isCorrectlyRotated = false;

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

        // Определение начального шага
        float initialZ = Mathf.Repeat(transform.localRotation.eulerAngles.z, 360f); // Угол 0..360
        currentRotationStep = Mathf.RoundToInt(initialZ / 90.0f) % 4; // Округляем до ближайшего шага 0, 1, 2, 3
        Debug.Log($"Плита '{gameObject.name}': Начальный угол Z ~{initialZ:F1}°, определен начальный шаг: {currentRotationStep}");

        // Устанавливаем начальное состояние isCorrectlyRotated БЕЗ уведомления контроллера
        isCorrectlyRotated = (currentRotationStep == targetRotationStep);
    }

    void Update()
    {
        // Проверяем нажатие E ТОЛЬКО если игрок рядом, плита не вращается СЕЙЧАС, И ВОРОТА ЗАКРЫТЫ
        if (playerInRange && !isRotating && gateController != null && !gateController.IsOpen && Input.GetKeyDown(KeyCode.E))
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

            // Показываем подсказку, если: игрок в зоне, плита НЕ вращается, ВОРОТА ЗАКРЫТЫ и подсказка назначена
            if (interactPromptUI != null && !isRotating && gateController != null && !gateController.IsOpen)
            {
                interactPromptUI.SetActive(true);
            }
            Debug.Log($"Игрок вошел в зону плиты {gameObject.name}.");
        }
    }

    // Когда игрок выходит из триггера плиты
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // Всегда скрываем подсказку при выходе
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
            Debug.Log($"Игрок вышел из зоны плиты {gameObject.name}.");
        }
    }

    // Запускает процесс вращения
    private void StartRotation()
    {
        // Дополнительная проверка перед началом корутины
        if (isRotating || gateController == null || gateController.IsOpen) return;

        isRotating = true;

        // Прячем подсказку на время вращения
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);

        // Вычисляем следующий шаг (по часовой стрелке) и целевой угол
        int nextStep = (currentRotationStep + 1) % 4;
        float targetAngleZ = nextStep * 90.0f;

        // Создаем целевое вращение, сохраняя текущие X и Y
        Quaternion targetRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, targetAngleZ);

        Debug.Log($"Плита {gameObject.name}: Вращаем с шага {currentRotationStep} на шаг {nextStep} (угол {targetAngleZ})");
        StartCoroutine(RotateCoroutine(targetRotation, nextStep));
    }

    // Корутина для плавного вращения
    IEnumerator RotateCoroutine(Quaternion targetLocalRotation, int nextStep)
    {
        Quaternion startLocalRotation = transform.localRotation;
        float time = 0;
        // Длительность зависит только от 90 градусов и скорости
        float duration = Mathf.Abs(90.0f / rotationSpeed);
        if (duration <= 0) duration = 0.1f; // Защита от деления на ноль

        while (time < duration)
        {
            // Slerp для плавного сферического вращения
            transform.localRotation = Quaternion.Slerp(startLocalRotation, targetLocalRotation, time / duration);
            time += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // Гарантируем точное конечное положение
        transform.localRotation = targetLocalRotation;
        currentRotationStep = nextStep;
        isRotating = false; // Важно! Сбрасываем флаг вращения ДО проверок

        // Проверяем правильность положения (и уведомляем контроллер, ЕСЛИ ворота еще не открыты)
        CheckIfCorrect();

        // Показываем подсказку СНОВА, если: игрок все еще в зоне, подсказка есть, И ВОРОТА ЗАКРЫТЫ
        if (playerInRange && interactPromptUI != null && gateController != null && !gateController.IsOpen)
        {
            interactPromptUI.SetActive(true);
        }

        Debug.Log($"Плита {gameObject.name}: Вращение завершено на шаге {currentRotationStep}. Правильно: {isCorrectlyRotated}");
    }

    // Проверяет, находится ли плита в целевом положении и уведомляет контроллер ворот
    void CheckIfCorrect()
    {
        // Если ворота уже открыты, просто обновляем локальное состояние и выходим
        if (gateController != null && gateController.IsOpen)
        {
            isCorrectlyRotated = (currentRotationStep == targetRotationStep);
            return;
        }

        bool previouslyCorrect = isCorrectlyRotated;
        isCorrectlyRotated = (currentRotationStep == targetRotationStep);

        // Уведомляем контроллер только если состояние изменилось И контроллер существует
        if (isCorrectlyRotated != previouslyCorrect && gateController != null)
        {
            gateController.PlateStateChanged(isCorrectlyRotated);
        }
    }
}
