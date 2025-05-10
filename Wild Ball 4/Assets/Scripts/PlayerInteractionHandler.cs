using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Основные Настройки")]
    private Vector3 startPosition; // Теперь это будет нашей текущей точкой респауна
    private Rigidbody playerRigidbody;
    private Renderer playerRenderer;
    private Collider playerCollider;

    [Header("Настройки Респауна")]
    public int flashCount = 4;
    public float flashDuration = 0.1f;
    private bool isRespawning = false;

    [Header("Эффекты")]
    public GameObject deathEffectPrefab;

    [Header("Интерфейс")]
    [Tooltip("Ссылка на TextMeshPro Text для отображения счета монет")]
    public TextMeshProUGUI coinText;
    private int coinCount = 0;

    [Header("Панели Интерфейса")] // Можешь добавить новый заголовок или к существующему "Интерфейс"
    [Tooltip("Ссылка на GameObject панели окончания игры (EndGamePanel)")]
    public GameObject endGamePanel; // <--- ДОБАВЬ ЭТУ СТРОКУ

    void Awake()
    {
        // Изначально startPosition - это позиция игрока при старте сцены
        startPosition = transform.position;
        playerRigidbody = GetComponent<Rigidbody>();
        playerRenderer = GetComponent<Renderer>();
        playerCollider = GetComponent<Collider>();

        if (playerRenderer == null) playerRenderer = GetComponentInChildren<Renderer>();

        // Проверки
        if (playerRigidbody == null) Debug.LogError("Rigidbody не найден!", this);
        if (playerRenderer == null) Debug.LogError("Renderer не найден!", this);
        if (playerCollider == null) Debug.LogError("Collider не найден!", this);
        if (deathEffectPrefab == null) Debug.LogError("Префаб 'Death Effect Prefab' не назначен!", this);
        if (coinText == null)
        {
            Debug.LogError("Ссылка на 'Coin Text' (TextMeshProUGUI) не назначена в инспекторе!", this);
        }
        UpdateCoinUI();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isRespawning && collision.gameObject.CompareTag("RespawnHazard"))
        {
            StartRespawnWithDeathEffect();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            LoadNextLevel();
        }

        if (other.gameObject.CompareTag("Bonus"))
        {
            // Вместо прямого вызова AddCoins, теперь можно сделать монету отдельным скриптом
            // и пусть монета сама себя уничтожает и вызывает AddCoins у игрока
            // Например: other.GetComponent<Coin>()?.Collect();
            // А пока оставим так для простоты, если нет отдельного скрипта для монеты:
            AddCoins(); // Предполагаем, что монета сама удалится или у нее есть свой скрипт
            Destroy(other.gameObject); // Пример, если монета должна удаляться при сборе
        }

        // Обработка чекпоинта теперь происходит в скрипте Checkpoint.cs
        // Нам не нужно здесь ничего добавлять для чекпоинтов, так как
        // Checkpoint.cs сам найдет PlayerInteractionHandler и вызовет SetNewRespawnPoint.
        // --- НАЧАЛО НОВОГО КОДА ДЛЯ ENDGAME ---
        if (other.gameObject.CompareTag("EndGame"))
        {
            Debug.Log("Игрок вошел в триггер EndGame.");
            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true); // Активируем панель
                Debug.Log("Панель EndGamePanel активирована.");

                // Опционально: здесь можно добавить другие действия, например:
                // Time.timeScale = 0f; // Поставить игру на паузу
                // GetComponent<PlayerMovementScript>()?.enabled = false; // Отключить скрипт движения игрока, если он есть
            }
            else
            {
                Debug.LogWarning("Панель EndGamePanel не назначена в инспекторе PlayerInteractionHandler, не могу ее активировать.");
            }
        }
        // --- КОНЕЦ НОВОГО КОДА ДЛЯ ENDGAME ---
    }

    // НОВЫЙ ПУБЛИЧНЫЙ МЕТОД для обновления точки респауна
    public void SetNewRespawnPoint(Vector3 newPosition)
    {
        startPosition = newPosition;
        Debug.Log($"Новая точка респауна установлена: {startPosition}");
    }

    public bool HasEnoughCoins(int amount)
    {
        return coinCount >= amount;
    }

    public bool SpendCoins(int amount)
    {
        if (HasEnoughCoins(amount))
        {
            coinCount -= amount;
            UpdateCoinUI();
            Debug.Log($"Потрачено {amount} монет. Осталось: {coinCount}");
            return true;
        }
        else
        {
            Debug.Log($"Недостаточно монет для траты {amount}. Текущий баланс: {coinCount}");
            return false;
        }
    }

    public void AddCoins(int amount = 1) // Добавим параметр amount, по умолчанию 1
    {
        coinCount += amount;
        Debug.Log($"Добавлено {amount} монет. Всего: {coinCount}");
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinCount;
        }
    }

    void LoadNextLevel()
    {
        if (isRespawning) return;
        Debug.Log("Уровень пройден! Загрузка следующего...");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(nextSceneIndex);
        else { Debug.Log("Все уровни пройдены!"); SceneManager.LoadScene(0); }
    }

    void StartRespawnWithDeathEffect()
    {
        if (isRespawning) return;
        isRespawning = true;
        Debug.Log("Столкновение с RespawnHazard! Отключаем игрока, показываем эффект...");

        if (playerCollider != null) playerCollider.enabled = false;
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
        }
        if (playerRenderer != null) playerRenderer.enabled = false;

        ParticleSystem deathEffectInstance = null;
        if (deathEffectPrefab != null)
        {
            GameObject effectGO = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            deathEffectInstance = effectGO.GetComponent<ParticleSystem>();
            if (deathEffectInstance == null) Debug.LogError("Префаб эффекта не содержит ParticleSystem!", effectGO);
        }
        else Debug.LogWarning("Префаб эффекта смерти не назначен!");

        StartCoroutine(WaitForEffectAndStartRespawn(deathEffectInstance));
    }

    IEnumerator WaitForEffectAndStartRespawn(ParticleSystem effectSystem)
    {
        if (effectSystem != null)
        {
            Debug.Log("Начинаем ожидание завершения эффекта...");
            // Ждем, пока система частиц жива ИЛИ пока она не будет уничтожена (effectSystem != null)
            // IsAlive(true) - проверяет, живы ли еще какие-либо частицы, включая дочерние.
            float startTime = Time.time;
            float maxWaitTime = 5f; // Максимальное время ожидания эффекта, на всякий случай

            while (effectSystem != null && effectSystem.IsAlive(true) && (Time.time - startTime) < maxWaitTime)
            {
                yield return null;
            }
            if (effectSystem != null && (Time.time - startTime) >= maxWaitTime)
            {
                Debug.LogWarning("Эффект смерти не завершился за максимальное время, продолжаем респаун.");
            }
            else if (effectSystem == null)
            {
                Debug.Log("Эффект смерти был уничтожен до завершения.");
            }
            else
            {
                Debug.Log("Эффект смерти завершился.");
            }
        }
        else Debug.LogWarning("Эффект для ожидания не найден.");

        Debug.Log("Начинаем респаун (перемещение и мигание)...");
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        // 1. Перемещение (игрок все еще невидим и коллайдер выключен)
        // ВАЖНО: используем startPosition, которая теперь может быть обновлена чекпоинтом
        transform.position = startPosition;
        if (playerRigidbody != null) // Сброс вращения, если есть
        {
            transform.rotation = Quaternion.identity; // или какое-то начальное вращение
            playerRigidbody.angularVelocity = Vector3.zero;
        }
        yield return null; // Дать один кадр на применение позиции

        // 2. Мигание (делаем игрока видимым)
        if (playerRenderer != null)
        {
            bool finalState = true;
            for (int i = 0; i < flashCount * 2; i++)
            {
                playerRenderer.enabled = finalState;
                yield return new WaitForSeconds(flashDuration);
                finalState = !finalState;
            }
            playerRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(flashCount * flashDuration * 2);
        }

        // 3. Восстановление взаимодействия
        if (playerCollider != null) playerCollider.enabled = true;
        if (playerRigidbody != null) playerRigidbody.isKinematic = false;

        isRespawning = false;
        Debug.Log("Респаун завершен.");
    }
}