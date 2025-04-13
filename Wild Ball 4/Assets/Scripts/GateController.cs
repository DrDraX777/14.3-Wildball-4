using UnityEngine;
using TMPro; // или using UnityEngine.UI;

public class GateController : MonoBehaviour
{
    [Header("Ссылки на Створки")]
    [Tooltip("Аниматор левой створки ворот")]
    public Animator leftDoorAnimator;
    [Tooltip("Аниматор правой створки ворот")]
    public Animator rightDoorAnimator;

    [Header("UI и Анимация")]
    [Tooltip("UI текст подсказки для ворот (Найти рычаг)")]
    public GameObject gateHintUI;
    [Tooltip("Имя триггера для анимации открытия створок")]
    public string openTriggerName = "OpenTrigger"; // Имя из Шага 1

    private bool isGateOpen = false; // Флаг, что ворота уже открыты

    void Start()
    {
        // Убедимся, что подсказка скрыта при старте
        if (gateHintUI != null)
            gateHintUI.SetActive(false);
    }

    // Публичный метод, который будет вызываться рычагом
    public void OpenTheGate()
    {
        if (!isGateOpen)
        {
            Debug.Log("Получена команда на открытие ворот!");
            isGateOpen = true;

            // Запускаем анимацию обеих створок
            if (leftDoorAnimator != null)
                leftDoorAnimator.SetTrigger(openTriggerName);
            else Debug.LogError("Аниматор левой створки не назначен в GateController!");

            if (rightDoorAnimator != null)
                rightDoorAnimator.SetTrigger(openTriggerName);
            else Debug.LogError("Аниматор правой створки не назначен в GateController!");

            // Скрываем подсказку у ворот навсегда
            if (gateHintUI != null)
                gateHintUI.SetActive(false);

            // Можно деактивировать сам триггер, чтобы он больше не срабатывал
            gameObject.GetComponent<Collider>().enabled = false;
            // Или полностью деактивировать объект триггера
            // gameObject.SetActive(false);
        }
    }

    // --- Логика показа/скрытия подсказки ---
    private void OnTriggerEnter(Collider other)
    {
        // Если ворота ЕЩЕ НЕ открыты и вошел игрок
        if (!isGateOpen && other.CompareTag("Player"))
        {
            if (gateHintUI != null)
                gateHintUI.SetActive(true); // Показываем подсказку "Найди рычаг"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Если вошел игрок (неважно, открыты ворота или нет, подсказка должна скрыться при выходе)
        if (other.CompareTag("Player"))
        {
            if (gateHintUI != null)
                gateHintUI.SetActive(false); // Скрываем подсказку
        }
    }
}
