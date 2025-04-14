using UnityEngine;
using TMPro; // Если подсказка - TextMeshPro
// using UnityEngine.UI; // Если подсказка - обычный Text/Image

public class LeverController : MonoBehaviour
{
    [Header("Ссылки")]
    [Tooltip("Объект ворот, которым управляет этот рычаг")]
    public GateController targetGate; // Сюда перетащить объект ворот

    [Tooltip("Аниматор самого рычага")]
    public Animator leverAnimator; // Сюда перетащить аниматор ЭТОГО рычага

    [Tooltip("UI элемент, подсказывающий нажать Е (опционально)")]
    public GameObject interactPromptUI; // Текст "Нажмите Е" для этого рычага

    [Header("Настройки")]
    [Tooltip("Идентификатор этого рычага (1 для первого, 2 для второго и т.д.)")]
    public int leverID = 1; // УНИКАЛЬНЫЙ ID для каждого рычага!

    [Tooltip("Имя триггера в аниматоре рычага для переключения")]
    public string animationTriggerName = "Activate";

    [Tooltip("Тег игрока")]
    public string playerTag = "Player";

    // Состояние
    private bool isActivated = false; // Рычаг уже активирован?
    private bool playerInRange = false; // Игрок в зоне действия?

    void Start()
    {
        // Проверки
        if (targetGate == null)
            Debug.LogError($"Рычаг '{gameObject.name}' не имеет ссылки на GateController!", this);
        if (leverAnimator == null)
            Debug.LogWarning($"Рычаг '{gameObject.name}' не имеет ссылки на свой Animator.", this);
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false); // Скрыть подсказку "Е" при старте
        else
            Debug.LogWarning($"Рычаг '{gameObject.name}' не имеет ссылки на UI подсказку 'Нажмите Е'.", this);

        // Убедимся, что есть коллайдер-триггер
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            Debug.LogWarning($"На рычаге '{gameObject.name}' отсутствует Collider или он не является триггером!", this);
    }

    void Update()
    {
        // Проверяем нажатие Е ТОЛЬКО если игрок рядом И рычаг еще НЕ активирован
        if (playerInRange && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }
    }

    // Когда игрок входит в триггер рычага
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag(playerTag))
        {
            playerInRange = true;
            // Показываем подсказку "Нажмите Е", если она есть и рычаг не активен
            if (interactPromptUI != null)
                interactPromptUI.SetActive(true);
            Debug.Log($"Игрок вошел в зону рычага {leverID}.");
        }
    }

    // Когда игрок выходит из триггера рычага
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // Скрываем подсказку "Нажмите Е", если она есть
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            Debug.Log($"Игрок вышел из зоны рычага {leverID}.");
        }
    }

    // Логика активации рычага
    private void ActivateLever()
    {
        if (isActivated) return; // Уже активирован, ничего не делаем

        isActivated = true;
        playerInRange = false; // Сбрасываем, чтобы не сработало еще раз до выхода/входа
        Debug.Log($"Рычаг {leverID} активирован!");

        // 1. Запускаем анимацию рычага (если есть)
        if (leverAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            leverAnimator.SetTrigger(animationTriggerName);
        }

        // 2. Скрываем подсказку "Нажмите Е" (если есть)
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }

        // 3. Сообщаем воротам об активации
        if (targetGate != null)
        {
            targetGate.NotifyLeverActivated(leverID); // Вызываем метод на скрипте ворот
        }

        // Опционально: можно деактивировать компонент после использования
        // GetComponent<Collider>().enabled = false;
        // this.enabled = false;
    }
}
