using TMPro;
using UnityEngine;

// ���� ������ �������� �� ������ ���������-�������
public class PlatformNotifier : MonoBehaviour
{
    // ������ �� GameManager (���������� ������ GameManager ���� � ����������)
    public GameManager gameManager;


    [Header("��������� ���������")]
    [Tooltip("������� TextMeshProUGUI �� UI Canvas, ��� ����� ������������ ���������")]
    public TextMeshProUGUI hintTextElement;

    [Tooltip("����� ���������, ������� ����� ������� ������")]
    [TextArea(3, 5)]
    public string hintMessage = "������� � ����� ��������� ������.";





    void Start()
    {
        // ��������� �������� ��� ������
        if (gameManager == null)
        {
            Debug.LogError("GameManager �� �������� ��� ���������: " + gameObject.name + "! ����������� �� ����� ��������.", this);
        }

        // �������� ������� �������-���������� (�������������)
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
        {
            Debug.LogWarning("�� ��������� " + gameObject.name + " ����������� Collider � ���������� 'Is Trigger'. ������ PlatformNotifier ����� �� ��������.", this);
        }
    }

    // ����������, ����� ����� ������ � ������� ���������
    void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
        {
            // ���� ����������� ��� ������, ������ �� ������ (��������� �� �����)
            Debug.Log($"[{this.GetType().Name}] ����� ({other.name}) �����, �� ����������� ��� ������. ��������� �� ��������.");
            return; // ������� �� ������
        }




        // �������� GameManager'�, ������ ���� ����� ����� � GameManager ��������
        if (other.CompareTag("Player") && gameManager != null)
        {

            hintTextElement.text = hintMessage;
            hintTextElement.gameObject.SetActive(true);
            // �������� ������ �� ���� ������ (�.�. �� ��� ���������� ���������)
            gameManager.PlayerEnteredPlatform(this);
        }
    }

    // ����������, ����� ����� ������� �� �������� ���������
    void OnTriggerExit(Collider other)
    {
        // �������� GameManager'�, ������ ���� ����� ����� � GameManager ��������
        if (other.CompareTag("Player") && gameManager != null)
        {

            if (hintTextElement.gameObject.activeSelf) // �������� ������ ���� �������
            {
                
                hintTextElement.gameObject.SetActive(false);
            }

            // �������� ������ �� ���� ������
            gameManager.PlayerExitedPlatform(this);
        }
    }
}