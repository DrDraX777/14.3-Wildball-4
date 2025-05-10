using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // ������ �� ������ ����� ������ ����� Canvas
    public GameObject pausePanel;

    private bool isPaused = false;

    // ���� ����� ����� �������� ����� (��������, �� ������� ������ Esc) ��� � ������ Pause
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

    // ����� ��� ���������� ���� �� �����
    void PauseGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(true); // ���������� ������ �����

        Time.timeScale = 0f; // ������������� ����� � ����
        isPaused = true;
        // ����� ����� �������� ������ ������������� �������, ���� �����
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
    }

    // ����� ��� ������ ���� � �����
    public void ResumeGame()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false); // �������� ������ �����

        Time.timeScale = 1f; // ������������ �����
        isPaused = false;
        // ����� ����� �������� ������ ���������� �������, ���� �����
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // ����� ��� ����������� �������� ������
    public void RestartLevel()
    {
        Time.timeScale = 1f; // ����� �������� Time.timeScale ����� ��������� ����� �����!
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // ����� ��� ������ � ������� ����
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // ����� �������� Time.timeScale!
        SceneManager.LoadScene(0); // ��������� ����� � �������� 0 (MainMenu)
    }

   

    // ��������, ��� ������ ������ ��� ������ ������
    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; // �� ������ ������, ���� ����� �� ����� ����� ������� ����
    }
}
