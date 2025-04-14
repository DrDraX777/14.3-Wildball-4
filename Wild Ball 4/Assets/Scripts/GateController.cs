using UnityEngine;
using TMPro; // Если подсказка - TextMeshPro
// using UnityEngine.UI; // Если подсказка - обычный Text/Image

public class GateController : MonoBehaviour
{
    [Header("Ссылки на Створки и Аниматоры")]
    [Tooltip("Аниматор левой створки ворот")]
    public Animator leftDoorAnimator;
    [Tooltip("Аниматор правой створки ворот")]
    public Animator rightDoorAnimator;

    [Header("Ссылки на UI")]
    [Tooltip("UI элемент (GameObject) с текстом 'Активируйте два рычага...'")]
    public GameObject activationHintUI;

    [Header("Настройки Анимации")]
    [Tooltip("Имя триггера в аниматорах створок для открытия")]
    public string openAnimationTrigger = "Open";

    [Header("Настройки Взаимодействия")]
    [Tooltip("Сколько всего рычагов должно быть активировано")]
    public int requiredLevers = 2; // Можно изменить, если рычагов будет больше/меньше
    [Tooltip("Тег игрока")]
    public string playerTag = "Player";

    // Состояние
    private int activatedLeverCount = 0; // Счетчик активированных рычагов
    private bool isOpen = false;         // Ворота уже открыты?
    private bool playerInZone = false;   // Игрок в триггерной зоне ворот?

    void Start()
    {
        // Проверки
        if (leftDoorAnimator == null || rightDoorAnimator == null)
            Debug.LogError("Не назначены аниматоры створок ворот!", this);
        if (activationHintUI == null)
            Debug.LogWarning("Не назначен UI элемент подсказки для ворот.", this);
        else
            activationHintUI.SetActive(false); // Скрыть подсказку при старте

        // Убедимся, что есть коллайдер-триггер
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            Debug.LogError("На родительском объекте ворот отсутствует Collider или он не является триггером!", this);
    }

    // Публичный метод, вызываемый из LeverController
    public void NotifyLeverActivated(int leverID)
    {
        // Проверяем, не открыты ли ворота уже и не получили ли мы уже сигнал от этого рычага
        // (Для простоты сейчас просто увеличиваем счетчик, но можно добавить проверку ID,
        // чтобы один рычаг не засчитался дважды, если что-то пойдет не так)
        if (!isOpen)
        {
            activatedLeverCount++;
            Debug.Log($"Получено уведомление от рычага {leverID}. Всего активировано: {activatedLeverCount}/{requiredLevers}");

            // Проверяем, достаточно ли рычагов активировано
            if (activatedLeverCount >= requiredLevers)
            {
                OpenGate();
            }
        }
    }

    // Логика открытия ворот
    private void OpenGate()
    {
        if (isOpen) return; // Уже открыты

        isOpen = true;
        Debug.Log("Все рычаги активированы! Открываем ворота...");

        // 1. Запускаем анимации створок
        if (leftDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            leftDoorAnimator.SetTrigger(openAnimationTrigger);
        if (rightDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            rightDoorAnimator.SetTrigger(openAnimationTrigger);

        // 2. Гарантированно скрываем UI подсказку
        if (activationHintUI != null)
            activationHintUI.SetActive(false);

        // Опционально: отключаем триггер ворот, чтобы подсказка больше не появлялась
        // Collider col = GetComponent<Collider>();
        // if (col != null) col.enabled = false;
        // Или можно просто положиться на флаг isOpen в OnTriggerEnter/Exit
    }

    // Когда игрок входит в триггер ворот
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = true;
            // Показываем подсказку ТОЛЬКО если ворота еще НЕ открыты
            if (!isOpen && activationHintUI != null)
            {
                activationHintUI.SetActive(true);
                Debug.Log("Игрок вошел в зону закрытых ворот - показываем подсказку.");
            }
        }
    }

    // Когда игрок выходит из триггера ворот
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = false;
            // Скрываем подсказку, если она была видна
            if (activationHintUI != null && activationHintUI.activeSelf)
            {
                activationHintUI.SetActive(false);
                Debug.Log("Игрок вышел из зоны ворот - скрываем подсказку.");
            }
        }
    }
}
