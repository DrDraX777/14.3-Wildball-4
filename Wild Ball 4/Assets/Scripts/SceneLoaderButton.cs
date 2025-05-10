using UnityEngine;
using UnityEngine.SceneManagement; // ����������� ��� ������ �� �������

public class SceneLoaderButton : MonoBehaviour
{
    // ��������� �����, ������� ����� ���������� ��� ������� �� ������
    public void LoadSceneByIndex(int sceneIndex)
    {
        // ��������, ���������� �� ����� � ����� �������� � Build Settings
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"���������� ��������� ����� � �������� {sceneIndex}. " +
                             $"������ ��� ����������� ��������� (0 �� {SceneManager.sceneCountInBuildSettings - 1}). " +
                             "���������, ��� ����� ��������� � Build Settings.");
            return;
        }

        Debug.Log($"�������� ����� � ��������: {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);

        // �����������: ���� ���� ���� �� �����, ����� �� ����� ��������� ����� �����
        // Time.timeScale = 1f;
    }

    // ������������������ ����� ��� �������� ����� � �������� 0 (������� ����/������ �������)
    // ���� ����� ������� ������������, ���� ������ ������ ������ ��������� ������ ����� 0
    public void LoadFirstScene()
    {
        Debug.Log("�������� ������ ����� (������ 0)...");
        // ����� ����� ����� ������������ ������ 0 ��� ������� ����� �����
        LoadSceneByIndex(0);
    }

    // ����� ��� ����������� ������� �����
    public void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"������������ ������� �����: {SceneManager.GetActiveScene().name} (������ {currentSceneIndex})");
        LoadSceneByIndex(currentSceneIndex);
    }
}