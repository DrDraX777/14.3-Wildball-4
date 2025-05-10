using UnityEngine;
using UnityEngine.SceneManagement; // Обязательно для работы со сценами

public class SceneLoaderButton : MonoBehaviour
{
    // Публичный метод, который будет вызываться при нажатии на кнопку
    public void LoadSceneByIndex(int sceneIndex)
    {
        // Проверка, существует ли сцена с таким индексом в Build Settings
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"Невозможно загрузить сцену с индексом {sceneIndex}. " +
                             $"Индекс вне допустимого диапазона (0 до {SceneManager.sceneCountInBuildSettings - 1}). " +
                             "Убедитесь, что сцена добавлена в Build Settings.");
            return;
        }

        Debug.Log($"Загрузка сцены с индексом: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);

        // Опционально: Если игра была на паузе, снять ее перед загрузкой новой сцены
        // Time.timeScale = 1f;
    }

    // Специализированный метод для загрузки сцены с индексом 0 (главное меню/первый уровень)
    // Этот метод удобнее использовать, если кнопка всегда должна загружать именно сцену 0
    public void LoadFirstScene()
    {
        Debug.Log("Загрузка первой сцены (индекс 0)...");
        // Здесь можно сразу использовать индекс 0 или вызвать общий метод
        LoadSceneByIndex(0);
    }

    // Метод для перезапуска текущей сцены
    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"Перезагрузка текущей сцены: {SceneManager.GetActiveScene().name} (индекс {currentSceneIndex})");
        LoadSceneByIndex(currentSceneIndex);
    }
}