using UnityEngine;
using UnityEngine.SceneManagement; // ���������� ��� ���������� �������

public class MenuManager : MonoBehaviour
{
    // ������ �� ������, ������� ����� ��������/���������
    // ���������� ��������������� ������ �� Hierarchy � ���������
    public GameObject mainPanel;
    public GameObject levelSelectPanel;

    // �����, ������� ����� ���������� ��� ������� ������ Play
    public void ShowLevelSelect()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false); // ��������� ������� ������

        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(true); // �������� ������ ������ �������
    }

    // ����� ��� ������ "�����"
    public void HideLevelSelect()
    {
        if (levelSelectPanel != null)
            levelSelectPanel.SetActive(false); // ��������� ������ ������ �������

        if (mainPanel != null)
            mainPanel.SetActive(true); // �������� ������� ������
    }

    // ����� ��� �������� ������ �� ��� Build Index
    // ���� ����� ����� �������������� �������� �������
    public void LoadLevel(int sceneBuildIndex)
    {
        // ��������, ��� ������ ���������� (������ ��� ����� 0 � ������ ���������� ���� � Build Settings)
        if (sceneBuildIndex >= 0 && sceneBuildIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
        else
        {
            Debug.LogError($"�������� ������ �����: {sceneBuildIndex}. ��������� Build Settings.");
        }
    }

    // (�����������) ����� ��� ������ �� ����
    public void QuitGame()
    {
        Debug.Log("����� �� ����..."); // �������� � �����, � ��������� ������ ������� ���������
        Application.Quit();
    }
}
