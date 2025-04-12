using UnityEngine;
using TMPro; // Используй это, если у тебя TextMeshPro
// using UnityEngine.UI; // Используй это, если у тебя старый Text

public class InvisHint : MonoBehaviour
{
   

    [Tooltip("UI Текст для подсказки взаимодействия")]
    public GameObject interactionPromptUI; // Сюда перетащим наш Text объект

      

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
        
    }

    

    // Срабатывает, когда другой коллайдер ВХОДИТ в триггерную зону
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, вошел ли объект с тегом "Player"
        if (other.CompareTag("Player"))
        {
            // Если дверь еще не открыта, показываем подсказку
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
            
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
            
        }
    }
}
