using UnityEngine;
using UnityEngine.SceneManagement; // Необходимо для управления сценами

public class MenuManager : MonoBehaviour
{
    // Ссылки на панели, которые будем включать/выключать
    // Перетащите соответствующие панели из Hierarchy в инспектор
    public GameObject mainPanel;
    public GameObject levelSelectPanel;

    // Метод, который будет вызываться при нажатии кнопки Play
    public void ShowLevelSelect()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false); // Выключаем главную панель

        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(true); // Включаем панель выбора уровней
    }

    // Метод для кнопки "Назад"
    public void HideLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false); // Выключаем панель выбора уровней

        if (mainPanel != null)
            mainPanel.SetActive(true); // Включаем главную панель
    }

    // Метод для загрузки уровня по его Build Index
    // Этот метод будет использоваться кнопками уровней
    public void LoadLevel(int sceneBuildIndex)
    {
        // Проверка, что индекс корректный (больше или равен 0 и меньше количества сцен в Build Settings)
        if (sceneBuildIndex >= 0 && sceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
        else
        {
            Debug.LogError($"Неверный индекс сцены: {sceneBuildIndex}. Проверьте Build Settings.");
        }
    }

    // (Опционально) Метод для выхода из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры..."); // Работает в билде, в редакторе просто выводит сообщение
        Application.Quit();
    }
}
