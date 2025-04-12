using UnityEngine;
using TMPro; // ��������� ���, ���� � ���� TextMeshPro
// using UnityEngine.UI; // ��������� ���, ���� � ���� ������ Text

public class DoorController : MonoBehaviour
{
    [Header("������ �� ����������")]
    [Tooltip("�������� �����, ������� ����� �������")]
    public Animator doorAnimator; // ���� ��������� �����

    [Tooltip("UI ����� ��� ��������� ��������������")]
    public GameObject interactionPromptUI; // ���� ��������� ��� Text ������

    [Header("��������� ��������")]
    [Tooltip("��� �������-��������� � ��������� ��� �������� �����")]
    public string openTriggerName = "OpenTrigger"; // ��� �������� �� ���� 3

    [Header("��������� ��������������")]
    [Tooltip("������� ��� ��������������")]
    public KeyCode interactionKey = KeyCode.E;

    // ��������� ���������� ��� ������������, ��������� �� ����� � ����
    private bool playerIsNear = false;
    // ����� ����� �� ����������� ��������, ���� ��� ��� �������
    private bool isDoorOpen = false;

    void Start()
    {
        // ������ ��������� ��� ������
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        // ���������, ��������� �� ����� �����, ������ �� ������� � ����� ��� �� �������
        if (playerIsNear && Input.GetKeyDown(interactionKey) && !isDoorOpen)
        {
            OpenTheDoor();
        }
    }

    // ����� ��� �������� �����
    private void OpenTheDoor()
    {
        if (doorAnimator != null)
        {
            // ��������� ������� ��������
            doorAnimator.SetTrigger(openTriggerName);

            // ��������, ��� ����� �������
            isDoorOpen = true;

            // �������� ��������� ����� ��������
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
            Debug.Log("����� �������!");
        }
        else
        {
            Debug.LogError("�������� ����� �� �������� � DoorController!");
        }
    }

    // �����������, ����� ������ ��������� ������ � ���������� ����
    private void OnTriggerEnter(Collider other)
    {
        // ���������, ����� �� ������ � ����� "Player"
        if (other.CompareTag("Player"))
        {
            // ���� ����� ��� �� �������, ���������� ���������
            if (!isDoorOpen && interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
            playerIsNear = true; // ��������, ��� ����� �����
            Debug.Log("����� ����� � ���� �����.");
        }
    }

    // �����������, ����� ������ ��������� ������� �� ���������� ����
    private void OnTriggerExit(Collider other)
    {
        // ���������, ����� �� ������ � ����� "Player"
        if (other.CompareTag("Player"))
        {
            // �������� ���������, ���� ��� ���� �����
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
            playerIsNear = false; // ��������, ��� ����� ����
            Debug.Log("����� ������� ���� �����.");
        }
    }
}
