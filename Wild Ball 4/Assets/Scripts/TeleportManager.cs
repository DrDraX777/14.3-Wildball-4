// TeleportManager.cs
using UnityEngine;
using TMPro;
using System.Collections; // Для корутины, если понадобится

public class TeleportManager : MonoBehaviour
{
    [Header("Ссылки на Объекты")]
    public GameObject teleportA;
    public GameObject teleportB;
    public TextMeshProUGUI teleportHint;

    [Header("Тексты Подсказок")]
    public string hintTextEnterToTeleport = "Нажмите E чтобы телепортироваться";
    public string hintTextActivateTeleport = "Нажмите E чтобы активировать телепорт";
    public string hintTextFindSecondTeleport = "Найдите второй телепорт для активации";

    [Header("Настройки Телепортации")]
    [Tooltip("Высота, на которую игрок будет поднят над точкой телепорта")]
    public float teleportHeightOffset = 1.0f; // Изменил на 1.0f, 2.0f может быть многовато для некоторых персонажей
    [Tooltip("Задержка в секундах между возможностями телепортироваться")]
    public float teleportCooldown = 0.75f; // Немного увеличил для надежности

    // Внутренние переменные состояния
    private bool playerIsNearTeleportA = false;
    private bool playerIsNearTeleportB = false;
    private bool teleportsAreActive = false;
    private float lastTeleportTimestamp = -Mathf.Infinity; // Время последней телепортации
    private Rigidbody playerRigidbody; // Кэшированная ссылка на Rigidbody игрока

    void Start()
    {
        if (teleportHint != null)
            teleportHint.gameObject.SetActive(false);
        else
            Debug.LogWarning("TeleportManager: UI подсказка (teleportHint) не назначена.");

        if (teleportA == null || teleportB == null)
        {
            Debug.LogError("TeleportManager: Один или оба телепорта (teleportA, teleportB) не назначены! Скрипт будет отключен.");
            enabled = false;
            return;
        }
        // Деактивируем сами объекты телепортов, если на них есть что-то, что не должно работать до активации
        // (например, визуальные эффекты). Это опционально.
    }

    void Update()
    {
        // 1. Проверка кулдауна
        bool isOnCooldown = Time.time < lastTeleportTimestamp + teleportCooldown;

        // 2. Обработка нажатия клавиши E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOnCooldown)
            {
                Debug.Log("TeleportManager: Попытка телепортации во время кулдауна.");
                return; // Ничего не делаем, если кулдаун активен
            }

            // Ищем игрока каждый раз или кэшируем его, если он не меняется
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("TeleportManager: Игрок с тегом 'Player' не найден в сцене!");
                return;
            }

            // Получаем Rigidbody игрока (лучше кэшировать в Start или при первом нахождении)
            if (playerRigidbody == null)
            {
                playerRigidbody = playerObject.GetComponent<Rigidbody>();
                if (playerRigidbody == null)
                {
                    Debug.LogError("TeleportManager: На объекте игрока отсутствует компонент Rigidbody! Телепортация через MovePosition невозможна.");
                    return;
                }
            }

            Vector3? targetWorldPosition = null; // Nullable Vector3 для хранения целевой позиции

            // Сценарий 1: Активация телепортов (игрок у второго телепорта B, телепорты неактивны)
            if (playerIsNearTeleportB && !teleportsAreActive)
            {
                ActivateTeleports(); // teleportsAreActive становится true
                targetWorldPosition = teleportA.transform.position + Vector3.up * teleportHeightOffset;

                // Сразу устанавливаем ожидаемое состояние ПОСЛЕ телепортации
                playerIsNearTeleportA = true;
                playerIsNearTeleportB = false; // Игрок покинет зону B
                Debug.Log("TeleportManager: Телепорты активированы. Игрок телепортируется из B в A.");
            }
            // Сценарий 2: Использование уже активных телепортов
            else if (teleportsAreActive)
            {
                if (playerIsNearTeleportA)
                {
                    targetWorldPosition = teleportB.transform.position + Vector3.up * teleportHeightOffset;
                    playerIsNearTeleportB = true;
                    playerIsNearTeleportA = false;
                    Debug.Log("TeleportManager: Игрок телепортируется из A в B.");
                }
                else if (playerIsNearTeleportB)
                {
                    targetWorldPosition = teleportA.transform.position + Vector3.up * teleportHeightOffset;
                    playerIsNearTeleportA = true;
                    playerIsNearTeleportB = false;
                    Debug.Log("TeleportManager: Игрок телепортируется из B в A.");
                }
            }

            // Если была определена целевая позиция, производим телепортацию
            if (targetWorldPosition.HasValue)
            {
                // Используем MovePosition для физически корректного перемещения
                playerRigidbody.velocity = Vector3.zero; // Обнуляем скорость перед телепортацией, чтобы избежать "вылетания"
                playerRigidbody.angularVelocity = Vector3.zero; // Обнуляем угловую скорость
                playerRigidbody.MovePosition(targetWorldPosition.Value);

                lastTeleportTimestamp = Time.time; // Устанавливаем кулдаун

                // Обновляем подсказку для нового местоположения
                // Делаем это здесь, так как флаги playerIsNear... уже установлены в ожидаемое состояние
                UpdateTeleportHint();
                Debug.Log($"TeleportManager: Игрок перемещен в {targetWorldPosition.Value}. Кулдаун активирован.");
            }
            else
            {
                // Если игрок нажал E, но условия для телепортации не выполнены
                Debug.Log("TeleportManager: Нажата клавиша E, но условия для телепортации не выполнены.");
            }
        }

        // 3. Обновление подсказки, если не было телепортации в этом кадре
        // Это нужно, чтобы подсказка обновлялась, когда игрок просто входит/выходит из зоны пешком
        // и не находится на кулдауне от предыдущей телепортации.
        if (!Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
        {
            //UpdateTeleportHint(); // Раскомментируй, если нужно, чтобы подсказка обновлялась постоянно.
            // Но лучше, чтобы PlayerEntered/Exited вызывали UpdateTeleportHint.
        }
    }

    // Вызывается из TeleportZone.cs, когда игрок входит в триггер
    public void OnPlayerEnteredZone(GameObject enteredTeleportZone)
    {
        if (Time.time < lastTeleportTimestamp + teleportCooldown)
        {
            // Если мы на кулдауне от телепортации, не меняем флаги playerIsNear...
            // так как они были установлены принудительно в Update.
            // Но мы можем обновить подсказку, если она не соответствует текущему состоянию.
            UpdateTeleportHint();
            return;
        }

        if (enteredTeleportZone == teleportA)
        {
            playerIsNearTeleportA = true;
        }
        else if (enteredTeleportZone == teleportB)
        {
            playerIsNearTeleportB = true;
        }
        UpdateTeleportHint();
        Debug.Log($"TeleportManager: Игрок вошел в зону {(enteredTeleportZone == teleportA ? "A" : "B")}. Флаги: A={playerIsNearTeleportA}, B={playerIsNearTeleportB}");
    }

    // Вызывается из TeleportZone.cs, когда игрок выходит из триггера
    public void OnPlayerExitedZone(GameObject exitedTeleportZone)
    {
        // Если мы вышли из зоны СРАЗУ ПОСЛЕ телепортации (т.е. на кулдауне),
        // флаги playerIsNear... уже должны были быть установлены в Update в состояние
        // "у целевого телепорта". Если мы вышли из исходного телепорта - это нормально.
        // Если мы вышли из целевого телепорта во время кулдауна - значит игрок быстро движется.

        bool updateHintAfterExit = true;

        if (exitedTeleportZone == teleportA)
        {
            if (playerIsNearTeleportA) playerIsNearTeleportA = false;
        }
        else if (exitedTeleportZone == teleportB)
        {
            if (playerIsNearTeleportB) playerIsNearTeleportB = false;
        }

        // Обновляем подсказку, только если не на кулдауне, или если вышли из всех зон
        if (Time.time >= lastTeleportTimestamp + teleportCooldown || (!playerIsNearTeleportA && !playerIsNearTeleportB))
        {
            UpdateTeleportHint();
        }
        Debug.Log($"TeleportManager: Игрок покинул зону {(exitedTeleportZone == teleportA ? "A" : "B")}. Флаги: A={playerIsNearTeleportA}, B={playerIsNearTeleportB}");
    }

    private void ActivateTeleports()
    {
        teleportsAreActive = true;
        // Визуальные эффекты активации телепортов можно добавить здесь
        Debug.Log("TeleportManager: Телепорты активированы!");
        // Подсказка будет обновлена в Update после телепортации или в UpdateTeleportHint
    }

    private void UpdateTeleportHint()
    {
        if (teleportHint == null) return;

        string messageToShow = null;

        if (playerIsNearTeleportA)
        {
            if (!teleportsAreActive) messageToShow = hintTextFindSecondTeleport;
            else messageToShow = hintTextEnterToTeleport;
        }
        else if (playerIsNearTeleportB)
        {
            if (!teleportsAreActive) messageToShow = hintTextActivateTeleport; // Приоритет активации
            else messageToShow = hintTextEnterToTeleport;
        }

        if (string.IsNullOrEmpty(messageToShow))
        {
            teleportHint.gameObject.SetActive(false);
        }
        else
        {
            teleportHint.text = messageToShow;
            teleportHint.gameObject.SetActive(true);
        }
    }

    // Публичный метод для проверки состояния (если нужен другим скриптам)
    public bool AreTeleportsActive()
    {
        return teleportsAreActive;
    }
}