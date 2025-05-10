using UnityEngine;

public class GameExitButton : MonoBehaviour
{
    // Публичный метод, который будет вызываться при нажатии на кнопку
    public void QuitGame()
    {
        Debug.Log("Попытка выхода из игры...");

        // Этот код работает по-разному в зависимости от того, где запущена игра:

        // Если игра запущена в редакторе Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Выход из режима Play в редакторе Unity.");
#else
        // Если игра запущена как скомпилированный билд (.exe)
        Application.Quit();
        Debug.Log("Команда Application.Quit() вызвана (работает только в билде).");
#endif
    }
}