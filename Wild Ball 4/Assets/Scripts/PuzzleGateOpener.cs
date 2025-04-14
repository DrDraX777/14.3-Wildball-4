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

    // Приватные переменные состояния
    private int currentCorrectPlates = 0; // Счетчик правильно повернутых плит
    private bool isOpen = false;          // Ворота уже открыты?

    void Start()
    {
        // Проверки
        if (leftDoorAnimator == null || rightDoorAnimator == null)
            Debug.LogError("Не назначены аниматоры створок ворот на PuzzleGateOpener!", this);
        currentCorrectPlates = 0;
        isOpen = false;
    }

    /// <summary>
    /// Вызывается из скрипта RotatingPlate, когда состояние плиты (правильное/неправильное) меняется.
    /// </summary>
    /// <param name="plateBecameCorrect">True - если плита стала правильной, False - если стала неправильной.</param>
    public void PlateStateChanged(bool plateBecameCorrect)
    {
        if (isOpen) return; // Если ворота уже открыты, ничего не делаем

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

        // Clamp на всякий случай (хотя не должно выходить за пределы)
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
        if (isOpen) return; // Защита от повторного открытия

        isOpen = true;
        Debug.Log("ГОЛОВОЛОМКА РЕШЕНА! Открываем ворота...");

        // Запускаем анимацию на обеих створках
        if (leftDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            leftDoorAnimator.SetTrigger(openAnimationTrigger);
        if (rightDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            rightDoorAnimator.SetTrigger(openAnimationTrigger);

        // Опционально: тут можно проиграть звук открытия, включить свет и т.д.
    }
}

