using TMPro;
using UnityEngine;

// Этот скрипт вешается на КАЖДУЮ платформу-триггер
public class PlatformNotifier : MonoBehaviour
{
    // Ссылка на GameManager (перетащить объект GameManager сюда в инспекторе)
    public GameManager gameManager;


    [Header("Настройки Подсказки")]
    [Tooltip("Элемент TextMeshProUGUI на UI Canvas, где будет отображаться подсказка")]
    public TextMeshProUGUI hintTextElement;

    [Tooltip("Текст подсказки, который будет показан игроку")]
    [TextArea(3, 5)]
    public string hintMessage = "Нажмите Е чтобы повернуть обьект.";





    void Start()
    {
        // Небольшая проверка при старте
        if (gameManager == null)
        {
            Debug.LogError("GameManager не назначен для платформы: " + gameObject.name + "! Уведомления не будут работать.", this);
        }

        // Проверка наличия триггер-коллайдера (рекомендуется)
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogWarning("На платформе " + gameObject.name + " отсутствует Collider с включенным 'Is Trigger'. Скрипт PlatformNotifier может не работать.", this);
        }
    }

    // Вызывается, когда игрок входит в триггер платформы
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
        {
            // Если головоломка уже решена, ничего не делаем (подсказка не нужна)
            Debug.Log($"[{this.GetType().Name}] Игрок ({other.name}) вошел, но головоломка уже решена. Подсказка не показана.");
            return; // Выходим из метода
        }




        // Сообщаем GameManager'у, только если вошел игрок и GameManager назначен
        if (other.CompareTag("Player") && gameManager != null)
        {

            hintTextElement.text = hintMessage;
            hintTextElement.gameObject.SetActive(true);
            // Передаем ссылку на ЭТОТ скрипт (т.е. на эту конкретную платформу)
            gameManager.PlayerEnteredPlatform(this);
        }
    }

    // Вызывается, когда игрок выходит из триггера платформы
    void OnTriggerExit(Collider other)
    {
        // Сообщаем GameManager'у, только если вышел игрок и GameManager назначен
        if (other.CompareTag("Player") && gameManager != null)
        {

            if (hintTextElement.gameObject.activeSelf) // Скрываем только если активна
            {
                
                hintTextElement.gameObject.SetActive(false);
            }

            // Передаем ссылку на ЭТОТ скрипт
            gameManager.PlayerExitedPlatform(this);
        }
    }
}