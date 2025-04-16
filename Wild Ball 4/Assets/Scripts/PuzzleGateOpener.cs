using UnityEngine;

public class PuzzleGateOpener : MonoBehaviour
{
    [Header("Ссылки на Аниматоры Створок")]
    [Tooltip("Аниматор левой створки ворот")]
    public Animator leftDoorAnimator;
    [Tooltip("Аниматор правой створки ворот")]
    public Animator rightDoorAnimator;

    [Header("Настройки Головоломки")]
    [Tooltip("Сколько всего плит должны быть в правильном положении")]
    public int requiredCorrectPlates = 4; // Должно совпадать с количеством плит
    [Tooltip("Имя триггера в аниматорах створок для открытия")]
    public string openAnimationTrigger = "Open"; // Имя триггера в Animator'ах створок
    [Tooltip("UI элемент для подсказки ворот (если есть отдельная)")] // Уточнил Tooltip
    public GameObject interactPromptUI; // Подсказка для самих ворот (может и не быть)
    [Tooltip("Тег объекта игрока")]
    public string playerTag = "Player";

    // Приватные переменные состояния
    private int currentCorrectPlates = 0; // Счетчик правильно повернутых плит
    private bool isOpen = false;          // Ворота уже открыты?

    /// <summary>
    /// Возвращает true, если головоломка решена и ворота открыты (или открываются).
    /// </summary>
    public bool IsOpen => isOpen;

    void Start()
    {
        // Проверки
        if (leftDoorAnimator == null || rightDoorAnimator == null)
            Debug.LogError("Не назначены аниматоры створок ворот на PuzzleGateOpener!", this);
        if (interactPromptUI != null) interactPromptUI.SetActive(false); // Скрыть подсказку ворот при старте
        currentCorrectPlates = 0;
        isOpen = false;
    }

    /// <summary>
    /// Вызывается из скрипта RotatingPlate, когда состояние плиты (правильное/неправильное) меняется.
    /// </summary>
    /// <param name="plateBecameCorrect">True - если плита стала правильной, False - если стала неправильной.</param>
    public void PlateStateChanged(bool plateBecameCorrect)
    {
        if (IsOpen) return; // Если ворота уже открыты, ничего не делаем

        if (plateBecameCorrect)
        {
            currentCorrectPlates++;
            Debug.Log($"Правильная плита! Всего правильных: {currentCorrectPlates}/{requiredCorrectPlates}");
        }
        else
        {
            currentCorrectPlates--;
            Debug.Log($"Плита стала неправильной! Всего правильных: {currentCorrectPlates}/{requiredCorrectPlates}");
        }

        // Clamp на всякий случай
        currentCorrectPlates = Mathf.Clamp(currentCorrectPlates, 0, requiredCorrectPlates);

        // Проверяем, достигнуто ли нужное количество
        if (currentCorrectPlates >= requiredCorrectPlates)
        {
            OpenGate();
        }
    }

    // Метод открытия ворот
    private void OpenGate()
    {
        if (IsOpen) return; // Защита от повторного открытия

        isOpen = true; // Устанавливаем флаг ПЕРЕД действиями
        Debug.Log("ГОЛОВОЛОМКА РЕШЕНА! Открываем ворота...");

        // Скрываем подсказку самих ворот, если она была
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);

        // Запускаем анимацию на обеих створках
        if (leftDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            leftDoorAnimator.SetTrigger(openAnimationTrigger);
        if (rightDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            rightDoorAnimator.SetTrigger(openAnimationTrigger);

        // Опционально: тут можно проиграть звук открытия, включить свет и т.д.
    }

    // Триггеры для показа/скрытия подсказки САМИХ ворот (если она есть)
    private void OnTriggerEnter(Collider other)
    {
        // Показываем подсказку ворот, только если они закрыты
        if (interactPromptUI != null && other.CompareTag(playerTag) && !IsOpen)
        {
            interactPromptUI.SetActive(true);
            Debug.Log($"Игрок вошел в зону ЗАКРЫТЫХ врат {gameObject.name}.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Всегда скрываем подсказку ворот при выходе
        if (interactPromptUI != null && other.CompareTag(playerTag))
        {
            interactPromptUI.SetActive(false);
            Debug.Log($"Игрок вышел из зоны врат {gameObject.name}.");
        }
    }
}
