using UnityEngine;
using TMPro; // ��� UnityEngine.UI

public class InvisHint : MonoBehaviour
{
    [Header("UI ���������")]
    [Tooltip("UI ������� (GameObject) ��� ��������� ��������������")]
    public GameObject interactionPromptUI; // ���� ��������� ��� Text/Image GameObject

    // ������: ������ �� ������� ������ ������, �.�. ����� ���� � ������ �������
    // [Header("������� ������")]
    // [Tooltip("������, ������������ � ������� ��������� ���������")]
    // public GameObject targetObjectToMonitor; // ��� ���� ������ �� ����� �����

    private bool targetActivated = false; // ����: ��� �� ����������� ������� ������?

    void Start()
    {
        // ������ ��������� ��� ������
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Interaction Prompt UI �� �������� � ����������!", this);
        }
    }

    // �����������, ����� ������ ��������� ������ � ���������� ���� ����� �������
    private void OnTriggerEnter(Collider other)
    {
        // ��������� ��� ������
        if (other.CompareTag("Player"))
        {
            // ���������� ��������� ������ ���� ������� ������ ��� �� ��� �����������
            if (!targetActivated && interactionPromptUI != null)
            {
                Debug.Log("����� ����� � ���� ���������, ���� �� ������������ - ���������� ���������.");
                interactionPromptUI.SetActive(true);
            }
            else if (targetActivated)
            {
                Debug.Log("����� ����� � ���� ���������, �� ���� ��� ������������ - ��������� �� �����.");
            }
        }
    }

    // �����������, ����� ������ ��������� ������� �� ���������� ���� ����� �������
    private void OnTriggerExit(Collider other)
    {
        // ��������� ��� ������
        if (other.CompareTag("Player"))
        {
            // ������ �������� ��������� ��� ������, ���� ��� ���� �����
            if (interactionPromptUI != null && interactionPromptUI.activeSelf)
            {
                Debug.Log("����� ����� �� ���� ��������� - �������� ���������.");
                interactionPromptUI.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ��������� �����, ������� ����� ������ �� TargetObjectCollider,
    /// ����� ����� ���������� � ������� ��������.
    /// </summary>
    public void NotifyTargetActivated()
    {
        Debug.Log($"InvisHint ������� �����������: ���� ({this.name}'s target) ���� ������������.");
        targetActivated = true;

        // �����: ���� ����� ��� ��� ��������� � ���������� ���� � ������ ��������� ����,
        // ����� ���������� ������ ���������.
        if (interactionPromptUI != null && interactionPromptUI.activeSelf)
        {
            Debug.Log("���� ������������, ���� ����� � ���� - ���������� �������� ���������.");
            interactionPromptUI.SetActive(false);
        }
    }
}
