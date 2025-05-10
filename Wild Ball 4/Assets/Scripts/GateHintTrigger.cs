using UnityEngine;
using TMPro; // Необходимо для работы с TextMeshPro

public class GateHintTrigger : MonoBehaviour
{
    [Header("Настройки Подсказки")]
    [Tooltip("Элемент TextMeshProUGUI на UI Canvas, где будет отображаться подсказка")]
    public TextMeshProUGUI hintTextElement;

    [Tooltip("Текст подсказки, который будет показан игроку")]
    [TextArea(3, 5)]
    public string hintMessage = "Чтобы открыть врата - решите головоломку на уровне.";

    [Header("Настройки Триггера")]
    [Tooltip("Тег объекта игрока, который должен активировать подсказку")]
    public string playerTag = "Player";

    // --- Внутренние переменные ---
    private bool isPlayerInside = false;
    // private GameManager gameManagerInstance; // Ссылка на GameManager (получим через Singleton)

    void Start()
    {
        // Проверка наличия TextMeshPro элемента (остается как есть)
        if (hintTextElement == null)
        {
            Debug.LogError($"[{this.GetType().Name}] Не назначен элемент TextMeshProUGUI в инспекторе на объекте {gameObject.name}!", this);
            this.enabled = false;
            return;
        }

        // // Получаем ссылку на GameManager через Singleton
        // // Делаем это в Start, предполагая, что GameManager уже прошел свой Awake
        // gameManagerInstance = GameManager.Instance;
        // if (gameManagerInstance == null)
        // {
        //     Debug.LogError($"[{this.GetType().Name}] Не удалось найти экземпляр GameManager! Убедитесь, что объект GameManager существует в сцене и активен.", this);
        //     this.enabled = false; // Отключаемся, если GameManager не найден
        //     return;
        // }

        // Скрываем текст подсказки при запуске
        hintTextElement.gameObject.SetActive(false);
        isPlayerInside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем тег вошедшего объекта
        if (other.CompareTag(playerTag) && !isPlayerInside)
        {
            // <<< НОВАЯ ПРОВЕРКА: Прежде чем показать подсказку, убедимся, что головоломка НЕ решена >>>
            // Используем GameManager.Instance для доступа к свойству IsPuzzleSolved
            if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
            {
                // Если головоломка уже решена, ничего не делаем (подсказка не нужна)
                Debug.Log($"[{this.GetType().Name}] Игрок ({other.name}) вошел, но головоломка уже решена. Подсказка не показана.");
                return; // Выходим из метода
            }

            // Если головоломка не решена И текстовый элемент назначен
            if (hintTextElement != null)
            {
                isPlayerInside = true;
                Debug.Log($"[{this.GetType().Name}] Игрок ({other.name}) вошел в зону подсказки ворот (головоломка не решена).");
                hintTextElement.text = hintMessage;
                hintTextElement.gameObject.SetActive(true);
            }
            // Добавим проверку на случай, если GameManager.Instance был null (хотя в Start это должно было отсечься)
            else if (GameManager.Instance == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Не удалось получить доступ к GameManager.Instance при входе игрока!", this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Логика выхода остается прежней - просто скрываем подсказку, если она была показана
        if (other.CompareTag(playerTag))
        {
            if (hintTextElement != null)
            {
                // Не важно, была ли она реально показана (могли войти, когда пазл решен),
                // просто гарантируем, что она будет скрыта при выходе.
                isPlayerInside = false; // Сбрасываем флаг в любом случае
                if (hintTextElement.gameObject.activeSelf) // Скрываем только если активна
                {
                    Debug.Log($"[{this.GetType().Name}] Игрок ({other.name}) покинул зону подсказки ворот.");
                    hintTextElement.gameObject.SetActive(false);
                }
            }
        }
    }

    // <<< Опционально: Проверка в Update >>>
    // Если нужно, чтобы подсказка исчезла СРАЗУ ЖЕ, как только головоломка решается,
    // даже если игрок все еще стоит в триггере.
    void Update()
    {
        // Если игрок внутри, подсказка видима, НО головоломка только что была решена...
        if (isPlayerInside && hintTextElement != null && hintTextElement.gameObject.activeSelf)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
            {
                // ...немедленно скрываем подсказку.
                Debug.Log($"[{this.GetType().Name}] Головоломка решена, пока игрок внутри триггера. Скрываем подсказку.");
                hintTextElement.gameObject.SetActive(false);
                // Можно также установить isPlayerInside = false, чтобы OnTriggerExit не пытался скрыть ее снова,
                // но это не обязательно, так как мы проверяем activeSelf перед скрытием.
            }
            else if (GameManager.Instance == null)
            {
                Debug.LogError($"[{this.GetType().Name}] Не удалось получить доступ к GameManager.Instance в Update!", this);
            }
        }
    }

    // OnDisable/OnDestroy остаются без изменений (опционально)
}