using UnityEngine;
using TMPro; // ��������� ���, ���� � ���� TextMeshPro
// using UnityEngine.UI; // ��������� ���, ���� � ���� ������ Text

public class InvisHint : MonoBehaviour
{
   

    [Tooltip("UI ����� ��� ��������� ��������������")]
    public GameObject interactionPromptUI; // ���� ��������� ��� Text ������

      

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
        
    }

    

    // �����������, ����� ������ ��������� ������ � ���������� ����
    private void OnTriggerEnter(Collider other)
    {
        // ���������, ����� �� ������ � ����� "Player"
        if (other.CompareTag("Player"))
        {
            // ���� ����� ��� �� �������, ���������� ���������
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
            
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
            
        }
    }
}
