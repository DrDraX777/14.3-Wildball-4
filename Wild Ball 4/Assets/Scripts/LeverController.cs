using UnityEngine;
using TMPro; // ���� ��������� - TextMeshPro
// using UnityEngine.UI; // ���� ��������� - ������� Text/Image

public class LeverController : MonoBehaviour
{
    [Header("������")]
    [Tooltip("������ �����, ������� ��������� ���� �����")]
    public GateController targetGate; // ���� ���������� ������ �����

    [Tooltip("�������� ������ ������")]
    public Animator leverAnimator; // ���� ���������� �������� ����� ������

    [Tooltip("UI �������, �������������� ������ � (�����������)")]
    public GameObject interactPromptUI; // ����� "������� �" ��� ����� ������

    [Header("���������")]
    [Tooltip("������������� ����� ������ (1 ��� �������, 2 ��� ������� � �.�.)")]
    public int leverID = 1; // ���������� ID ��� ������� ������!

    [Tooltip("��� �������� � ��������� ������ ��� ������������")]
    public string animationTriggerName = "Activate";

    [Tooltip("��� ������")]
    public string playerTag = "Player";

    // ���������
    private bool isActivated = false; // ����� ��� �����������?
    private bool playerInRange = false; // ����� � ���� ��������?

    void Start()
    {
        // ��������
        if (targetGate == null)
            Debug.LogError($"����� '{gameObject.name}' �� ����� ������ �� GateController!", this);
        if (leverAnimator == null)
            Debug.LogWarning($"����� '{gameObject.name}' �� ����� ������ �� ���� Animator.", this);
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false); // ������ ��������� "�" ��� ������
        else
            Debug.LogWarning($"����� '{gameObject.name}' �� ����� ������ �� UI ��������� '������� �'.", this);

        // ��������, ��� ���� ���������-�������
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            Debug.LogWarning($"�� ������ '{gameObject.name}' ����������� Collider ��� �� �� �������� ���������!", this);
    }

    void Update()
    {
        // ��������� ������� � ������ ���� ����� ����� � ����� ��� �� �����������
        if (playerInRange && !isActivated && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
        }
    }

    // ����� ����� ������ � ������� ������
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag(playerTag))
        {
            playerInRange = true;
            // ���������� ��������� "������� �", ���� ��� ���� � ����� �� �������
            if (interactPromptUI != null)
                interactPromptUI.SetActive(true);
            Debug.Log($"����� ����� � ���� ������ {leverID}.");
        }
    }

    // ����� ����� ������� �� �������� ������
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // �������� ��������� "������� �", ���� ��� ����
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            Debug.Log($"����� ����� �� ���� ������ {leverID}.");
        }
    }

    // ������ ��������� ������
    private void ActivateLever()
    {
        if (isActivated) return; // ��� �����������, ������ �� ������

        isActivated = true;
        playerInRange = false; // ����������, ����� �� ��������� ��� ��� �� ������/�����
        Debug.Log($"����� {leverID} �����������!");

        // 1. ��������� �������� ������ (���� ����)
        if (leverAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            leverAnimator.SetTrigger(animationTriggerName);
        }

        // 2. �������� ��������� "������� �" (���� ����)
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }

        // 3. �������� ������� �� ���������
        if (targetGate != null)
        {
            targetGate.NotifyLeverActivated(leverID); // �������� ����� �� ������� �����
        }

        // �����������: ����� �������������� ��������� ����� �������������
        // GetComponent<Collider>().enabled = false;
        // this.enabled = false;
    }
}
