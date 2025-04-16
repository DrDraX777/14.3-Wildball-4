using UnityEngine;
using TMPro; // Если используешь TextMeshPro для подсказки
// using UnityEngine.UI; // Если используешь обычный UI Text/Image

[RequireComponent(typeof(Collider))] // Нужен коллайдер-триггер для зоны
public class HatchOpener : MonoBehaviour
{
    [Header("Настройки Люка")]
    [Tooltip("Количество монет, необходимое для открытия")]
    public int coinsRequired = 10;
    [Tooltip("Аниматор люка")]
    public Animator hatchAnimator;
    [Tooltip("Имя триггера в аниматоре для анимации открытия")]
    public string openAnimationTrigger = "OpenHatch"; // Замени на имя твоего триггера

    [Header("Взаимодействие")]
    [Tooltip("UI элемент (GameObject) для подсказки 'Нажмите Е...'")]
    public GameObject interactPromptUI;
    [Tooltip("Текст, отображаемый в подсказке")]
    public string promptTextFormat = "Нажмите [E], чтобы открыть. Цена - ({0} монет)"; // {0} заменится на coinsRequired
    [Tooltip("Тег объекта игрока")]
    public string playerTag = "Player";

    // Состояние
    private bool playerInRange = false;
    private bool isOpen = false;
    private PlayerInteractionHandler playerInteractionHandler = null; // Ссылка на скрипт игрока

    void Start()
    {
        // Проверка аниматора
        if (hatchAnimator == null)
        {
            Debug.LogError($"Аниматор (hatchAnimator) не назначен для люка '{gameObject.name}'!", this);
        }

        // Настройка коллайдера (убедимся, что он триггер)
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"Коллайдер на люке '{gameObject.name}' не является триггером. Устанавливаю isTrigger = true.", this);
            col.isTrigger = true;
        }

        // Скрыть подсказку при старте
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"UI подсказка (interactPromptUI) не назначена для люка '{gameObject.name}'.", this);
        }

        isOpen = false; // Убедимся, что люк закрыт при старте
    }

    void Update()
    {
        // Проверяем нажатие 'E' только если игрок в зоне, люк закрыт, и ссылка на игрока есть
        if (playerInRange && !isOpen && playerInteractionHandler != null && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenHatch();
        }
    }

    // Когда игрок ВХОДИТ в триггерную зону люка
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок и люк еще ЗАКРЫТ
        if (!isOpen && other.CompareTag(playerTag))
        {
            // Пытаемся получить компонент PlayerInteractionHandler
            playerInteractionHandler = other.GetComponent<PlayerInteractionHandler>();

            if (playerInteractionHandler != null)
            {
                playerInRange = true;
                // Показываем подсказку, если она есть
                if (interactPromptUI != null)
                {
                    // Обновляем текст подсказки, если там есть TextMeshPro или Text
                    UpdatePromptText();
                    interactPromptUI.SetActive(true);
                }
                Debug.Log($"Игрок вошел в зону люка '{gameObject.name}'.");
            }
            else
            {
                // Если на объекте с тегом Player нет нужного скрипта
                Debug.LogWarning($"Объект с тегом '{playerTag}' вошел в зону, но не имеет скрипта PlayerInteractionHandler.", other);
            }
        }
    }

    // Когда игрок ВЫХОДИТ из триггерной зоны люка
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerInteractionHandler = null; // Сбрасываем ссылку на игрока

            // Скрываем подсказку, если она есть
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
            Debug.Log($"Игрок вышел из зоны люка '{gameObject.name}'.");
        }
    }

    // Пытаемся открыть люк
    private void TryOpenHatch()
    {
        if (isOpen || playerInteractionHandler == null) return; // Если уже открыт или нет игрока

        Debug.Log($"Попытка открыть люк. Требуется: {coinsRequired} монет.");

        // Проверяем, достаточно ли у игрока монет И тратим их
        if (playerInteractionHandler.SpendCoins(coinsRequired)) // SpendCoins вернет true, если монеты были успешно потрачены
        {
            // Монеты потрачены, открываем люк
            OpenHatch();
        }
        else
        {
            // Недостаточно монет
            Debug.Log("Недостаточно монет для открытия люка!");
            // Опционально: проиграть звук "неудачи" или показать сообщение
            // Например, можно временно изменить текст подсказки на "Недостаточно монет"
        }
    }

    // Выполняет действия по открытию люка
    private void OpenHatch()
    {
        isOpen = true; // Помечаем люк как открытый
        Debug.Log($"Люк '{gameObject.name}' открыт!");

        // Скрываем подсказку навсегда для этого люка
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }

        // Запускаем анимацию
        if (hatchAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
        {
            hatchAnimator.SetTrigger(openAnimationTrigger);
        }

        // Опционально: можно отключить коллайдер или этот скрипт,
        // чтобы больше не проверять взаимодействие
        // GetComponent<Collider>().enabled = false;
        // this.enabled = false;
    }

    // Обновляет текст в UI подсказке
    private void UpdatePromptText()
    {
        if (interactPromptUI == null) return;

        // Пытаемся найти компонент TextMeshProUGUI или Text
        TMP_Text tmpText = interactPromptUI.GetComponent<TMP_Text>(); // TMP_Text - базовый класс для TextMeshProUGUI
        if (tmpText != null)
        {
            tmpText.text = string.Format(promptTextFormat, coinsRequired);
            return; // Нашли TextMeshPro, выходим
        }

        UnityEngine.UI.Text uiText = interactPromptUI.GetComponent<UnityEngine.UI.Text>();
        if (uiText != null)
        {
            uiText.text = string.Format(promptTextFormat, coinsRequired);
            return; // Нашли обычный UI Text
        }

        // Если не нашли ни тот, ни другой компонент текста
        Debug.LogWarning($"Не найден компонент TMP_Text или UI.Text на объекте подсказки '{interactPromptUI.name}' для люка '{gameObject.name}'.", interactPromptUI);
    }
}
