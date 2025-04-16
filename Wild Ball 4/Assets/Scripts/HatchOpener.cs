using UnityEngine;
using TMPro; // ���� ����������� TextMeshPro ��� ���������
// using UnityEngine.UI; // ���� ����������� ������� UI Text/Image

[RequireComponent(typeof(Collider))] // ����� ���������-������� ��� ����
public class HatchOpener : MonoBehaviour
{
    [Header("��������� ����")]
    [Tooltip("���������� �����, ����������� ��� ��������")]
    public int coinsRequired = 10;
    [Tooltip("�������� ����")]
    public Animator hatchAnimator;
    [Tooltip("��� �������� � ��������� ��� �������� ��������")]
    public string openAnimationTrigger = "OpenHatch"; // ������ �� ��� ������ ��������

    [Header("��������������")]
    [Tooltip("UI ������� (GameObject) ��� ��������� '������� �...'")]
    public GameObject interactPromptUI;
    [Tooltip("�����, ������������ � ���������")]
    public string promptTextFormat = "������� [E], ����� �������. ���� - ({0} �����)"; // {0} ��������� �� coinsRequired
    [Tooltip("��� ������� ������")]
    public string playerTag = "Player";

    // ���������
    private bool playerInRange = false;
    private bool isOpen = false;
    private PlayerInteractionHandler playerInteractionHandler = null; // ������ �� ������ ������

    void Start()
    {
        // �������� ���������
        if (hatchAnimator == null)
        {
            Debug.LogError($"�������� (hatchAnimator) �� �������� ��� ���� '{gameObject.name}'!", this);
        }

        // ��������� ���������� (��������, ��� �� �������)
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"��������� �� ���� '{gameObject.name}' �� �������� ���������. ������������ isTrigger = true.", this);
            col.isTrigger = true;
        }

        // ������ ��������� ��� ������
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"UI ��������� (interactPromptUI) �� ��������� ��� ���� '{gameObject.name}'.", this);
        }

        isOpen = false; // ��������, ��� ��� ������ ��� ������
    }

    void Update()
    {
        // ��������� ������� 'E' ������ ���� ����� � ����, ��� ������, � ������ �� ������ ����
        if (playerInRange && !isOpen && playerInteractionHandler != null && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenHatch();
        }
    }

    // ����� ����� ������ � ���������� ���� ����
    private void OnTriggerEnter(Collider other)
    {
        // ���������, ��� ��� ����� � ��� ��� ������
        if (!isOpen && other.CompareTag(playerTag))
        {
            // �������� �������� ��������� PlayerInteractionHandler
            playerInteractionHandler = other.GetComponent<PlayerInteractionHandler>();

            if (playerInteractionHandler != null)
            {
                playerInRange = true;
                // ���������� ���������, ���� ��� ����
                if (interactPromptUI != null)
                {
                    // ��������� ����� ���������, ���� ��� ���� TextMeshPro ��� Text
                    UpdatePromptText();
                    interactPromptUI.SetActive(true);
                }
                Debug.Log($"����� ����� � ���� ���� '{gameObject.name}'.");
            }
            else
            {
                // ���� �� ������� � ����� Player ��� ������� �������
                Debug.LogWarning($"������ � ����� '{playerTag}' ����� � ����, �� �� ����� ������� PlayerInteractionHandler.", other);
            }
        }
    }

    // ����� ����� ������� �� ���������� ���� ����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            playerInteractionHandler = null; // ���������� ������ �� ������

            // �������� ���������, ���� ��� ����
            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
            Debug.Log($"����� ����� �� ���� ���� '{gameObject.name}'.");
        }
    }

    // �������� ������� ���
    private void TryOpenHatch()
    {
        if (isOpen || playerInteractionHandler == null) return; // ���� ��� ������ ��� ��� ������

        Debug.Log($"������� ������� ���. ���������: {coinsRequired} �����.");

        // ���������, ���������� �� � ������ ����� � ������ ��
        if (playerInteractionHandler.SpendCoins(coinsRequired)) // SpendCoins ������ true, ���� ������ ���� ������� ���������
        {
            // ������ ���������, ��������� ���
            OpenHatch();
        }
        else
        {
            // ������������ �����
            Debug.Log("������������ ����� ��� �������� ����!");
            // �����������: ��������� ���� "�������" ��� �������� ���������
            // ��������, ����� �������� �������� ����� ��������� �� "������������ �����"
        }
    }

    // ��������� �������� �� �������� ����
    private void OpenHatch()
    {
        isOpen = true; // �������� ��� ��� ��������
        Debug.Log($"��� '{gameObject.name}' ������!");

        // �������� ��������� �������� ��� ����� ����
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }

        // ��������� ��������
        if (hatchAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
        {
            hatchAnimator.SetTrigger(openAnimationTrigger);
        }

        // �����������: ����� ��������� ��������� ��� ���� ������,
        // ����� ������ �� ��������� ��������������
        // GetComponent<Collider>().enabled = false;
        // this.enabled = false;
    }

    // ��������� ����� � UI ���������
    private void UpdatePromptText()
    {
        if (interactPromptUI == null) return;

        // �������� ����� ��������� TextMeshProUGUI ��� Text
        TMP_Text tmpText = interactPromptUI.GetComponent<TMP_Text>(); // TMP_Text - ������� ����� ��� TextMeshProUGUI
        if (tmpText != null)
        {
            tmpText.text = string.Format(promptTextFormat, coinsRequired);
            return; // ����� TextMeshPro, �������
        }

        UnityEngine.UI.Text uiText = interactPromptUI.GetComponent<UnityEngine.UI.Text>();
        if (uiText != null)
        {
            uiText.text = string.Format(promptTextFormat, coinsRequired);
            return; // ����� ������� UI Text
        }

        // ���� �� ����� �� ���, �� ������ ��������� ������
        Debug.LogWarning($"�� ������ ��������� TMP_Text ��� UI.Text �� ������� ��������� '{interactPromptUI.name}' ��� ���� '{gameObject.name}'.", interactPromptUI);
    }
}
