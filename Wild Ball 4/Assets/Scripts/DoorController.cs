using UnityEngine;
using TMPro; // Используй это, если у тебя TextMeshPro
// using UnityEngine.UI; // Используй это, если у тебя старый Text

public class DoorController : MonoBehaviour
{
    [Header("Ссылки на компоненты")]
    [Tooltip("Аниматор двери, которую нужно открыть")]
    public Animator doorAnimator; // Сюда перетащим Дверь

    [Tooltip("UI Текст для подсказки взаимодействия")]
    public GameObject interactionPromptUI; // Сюда перетащим наш Text объект

    [Header("Параметры Анимации")]
    [Tooltip("Имя триггер-параметра в аниматоре для открытия двери")]
    public string openTriggerName = "OpenTrigger"; // Имя триггера из Шага 3

    [Header("Параметры Взаимодействия")]
    [Tooltip("Клавиша для взаимодействия")]
    public KeyCode interactionKey = KeyCode.E;

    // Приватная переменная для отслеживания, находится ли игрок в зоне
    private bool playerIsNear = false;
    // Чтобы дверь не открывалась повторно, если она уже открыта
    private bool isDoorOpen = false;

    void Start()
    {
        // Скрыть подсказку при старте
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // Проверяем, находится ли игрок рядом, нажата ли клавиша И дверь еще не открыта
        if (playerIsNear && Input.GetKeyDown(interactionKey) && !isDoorOpen)
        {
            OpenTheDoor();
        }
    }

    // Метод для открытия двери
    private void OpenTheDoor()
    {
        if (doorAnimator != null)
        {
            // Запускаем триггер анимации
            doorAnimator.SetTrigger(openTriggerName);

            // Помечаем, что дверь открыта
            isDoorOpen = true;

            // Скрываем подсказку после открытия
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
            Debug.Log("Дверь открыта!");
        }
        else
        {
            Debug.LogError("Аниматор двери не назначен в DoorController!");
        }
    }

    // Срабатывает, когда другой коллайдер ВХОДИТ в триггерную зону
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, вошел ли объект с тегом "Player"
        if (other.CompareTag("Player"))
        {
            // Если дверь еще не открыта, показываем подсказку
            if (!isDoorOpen && interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
            playerIsNear = true; // Помечаем, что игрок рядом
            Debug.Log("Игрок вошел в зону двери.");
        }
    }

    // Срабатывает, когда другой коллайдер ВЫХОДИТ из триггерной зоны
    private void OnTriggerExit(Collider other)
    {
        // Проверяем, вышел ли объект с тегом "Player"
        if (other.CompareTag("Player"))
        {
            // Скрываем подсказку, если она была видна
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
            playerIsNear = false; // Помечаем, что игрок ушел
            Debug.Log("Игрок покинул зону двери.");
        }
    }
}
