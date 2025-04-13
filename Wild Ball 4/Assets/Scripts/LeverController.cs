using UnityEngine;
using TMPro; // или using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Аниматор самого рычага")]
    public Animator leverAnimator;
    [Tooltip("UI текст подсказки для рычага (Нажмите E)")]
    public GameObject leverHintUI;
    [Tooltip("Ссылка на скрипт GateController на триггере ворот")]
    public GateController gateController; // СЮДА ПЕРЕТАЩИТЬ GateHintTrigger!

    [Header("Параметры")]
    [Tooltip("Имя триггера для анимации рычага")]
    public string pullTriggerName = "PullTrigger"; // Имя из Шага 1
    [Tooltip("Клавиша взаимодействия")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerIsNear = false;
    private bool isLeverPulled = false; // Флаг, что рычаг уже нажат

    void Start()
    {
        // Скрыть подсказку при старте
        if (leverHintUI != null)
            leverHintUI.SetActive(false);

        // Проверка наличия ссылки на контроллер ворот
        if (gateController == null)
            Debug.LogError("GateController не назначен в LeverController на объекте " + gameObject.name);
    }

    void Update()
    {
        // Если игрок рядом, рычаг еще не нажат и игрок нажал E
        if (playerIsNear && !isLeverPulled && Input.GetKeyDown(interactionKey))
        {
            ActivateLever();
        }
    }

    private void ActivateLever()
    {
        Debug.Log("Игрок активировал рычаг!");
        isLeverPulled = true; // Помечаем, что рычаг активирован

        // Скрываем подсказку у рычага навсегда
        if (leverHintUI != null)
            leverHintUI.SetActive(false);

        // Запускаем анимацию рычага
        if (leverAnimator != null)
            leverAnimator.SetTrigger(pullTriggerName);
        else Debug.LogError("Аниматор рычага не назначен в LeverController!");

        // !!! Вызываем метод открытия ворот на GateController !!!
        if (gateController != null)
            gateController.OpenTheGate();
        else Debug.LogError("Не могу открыть ворота, GateController не назначен!");

        // Можно деактивировать триггер рычага
        gameObject.GetComponent<Collider>().enabled = false;
        // gameObject.SetActive(false);
    }

    // --- Логика показа/скрытия подсказки ---
    private void OnTriggerEnter(Collider other)
    {
        // Если рычаг ЕЩЕ НЕ нажат и вошел игрок
        if (!isLeverPulled && other.CompareTag("Player"))
        {
            if (leverHintUI != null)
                leverHintUI.SetActive(true); // Показываем подсказку "Нажмите E"
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Если вошел игрок (неважно, нажат рычаг или нет)
        if (other.CompareTag("Player"))
        {
            if (leverHintUI != null)
                leverHintUI.SetActive(false); // Скрываем подсказку
            playerIsNear = false;
        }
    }
}
