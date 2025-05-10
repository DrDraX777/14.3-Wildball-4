using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Ссылка на панель паузы внутри этого Canvas
    public GameObject pausePanel;

    private bool isPaused = false;

    // Этот метод можно вызывать извне (например, по нажатию кнопки Esc) или с кнопки Pause
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Метод для постановки игры на паузу
    void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true); // Показываем панель паузы

        Time.timeScale = 0f; // Останавливаем время в игре
        isPaused = true;
        // Здесь можно добавить логику разблокировки курсора, если нужно
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    // Метод для снятия игры с паузы
    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // Скрываем панель паузы

        Time.timeScale = 1f; // Возобновляем время
        isPaused = false;
        // Здесь можно добавить логику блокировки курсора, если нужно
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // Метод для перезапуска текущего уровня
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Важно сбросить Time.timeScale перед загрузкой новой сцены!
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Метод для выхода в главное меню
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Важно сбросить Time.timeScale!
        SceneManager.LoadScene(0); // Загружаем сцену с индексом 0 (MainMenu)
    }

   

    // Убедимся, что панель скрыта при старте уровня
    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; // На всякий случай, если вышли из паузы через главное меню
    }
}
