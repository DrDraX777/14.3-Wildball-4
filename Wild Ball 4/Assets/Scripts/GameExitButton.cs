using UnityEngine;

public class GameExitButton : MonoBehaviour
{
    // ��������� �����, ������� ����� ���������� ��� ������� �� ������
    public void QuitGame()
    {
        Debug.Log("������� ������ �� ����...");

        // ���� ��� �������� ��-������� � ����������� �� ����, ��� �������� ����:

        // ���� ���� �������� � ��������� Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("����� �� ������ Play � ��������� Unity.");
#else
        // ���� ���� �������� ��� ���������������� ���� (.exe)
        Application.Quit();
        Debug.Log("������� Application.Quit() ������� (�������� ������ � �����).");
#endif
    }
}