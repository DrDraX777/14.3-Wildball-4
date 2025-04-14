using UnityEngine;
using TMPro; // ���� ��������� - TextMeshPro
// using UnityEngine.UI; // ���� ��������� - ������� Text/Image

public class GateController : MonoBehaviour
{
    [Header("������ �� ������� � ���������")]
    [Tooltip("�������� ����� ������� �����")]
    public Animator leftDoorAnimator;
    [Tooltip("�������� ������ ������� �����")]
    public Animator rightDoorAnimator;

    [Header("������ �� UI")]
    [Tooltip("UI ������� (GameObject) � ������� '����������� ��� ������...'")]
    public GameObject activationHintUI;

    [Header("��������� ��������")]
    [Tooltip("��� �������� � ���������� ������� ��� ��������")]
    public string openAnimationTrigger = "Open";

    [Header("��������� ��������������")]
    [Tooltip("������� ����� ������� ������ ���� ������������")]
    public int requiredLevers = 2; // ����� ��������, ���� ������� ����� ������/������
    [Tooltip("��� ������")]
    public string playerTag = "Player";

    // ���������
    private int activatedLeverCount = 0; // ������� �������������� �������
    private bool isOpen = false;         // ������ ��� �������?
    private bool playerInZone = false;   // ����� � ���������� ���� �����?

    void Start()
    {
        // ��������
        if (leftDoorAnimator == null || rightDoorAnimator == null)
            Debug.LogError("�� ��������� ��������� ������� �����!", this);
        if (activationHintUI == null)
            Debug.LogWarning("�� �������� UI ������� ��������� ��� �����.", this);
        else
            activationHintUI.SetActive(false); // ������ ��������� ��� ������

        // ��������, ��� ���� ���������-�������
        Collider col = GetComponent<Collider>();
        if (col == null || !col.isTrigger)
            Debug.LogError("�� ������������ ������� ����� ����������� Collider ��� �� �� �������� ���������!", this);
    }

    // ��������� �����, ���������� �� LeverController
    public void NotifyLeverActivated(int leverID)
    {
        // ���������, �� ������� �� ������ ��� � �� �������� �� �� ��� ������ �� ����� ������
        // (��� �������� ������ ������ ����������� �������, �� ����� �������� �������� ID,
        // ����� ���� ����� �� ���������� ������, ���� ���-�� ������ �� ���)
        if (!isOpen)
        {
            activatedLeverCount++;
            Debug.Log($"�������� ����������� �� ������ {leverID}. ����� ������������: {activatedLeverCount}/{requiredLevers}");

            // ���������, ���������� �� ������� ������������
            if (activatedLeverCount >= requiredLevers)
            {
                OpenGate();
            }
        }
    }

    // ������ �������� �����
    private void OpenGate()
    {
        if (isOpen) return; // ��� �������

        isOpen = true;
        Debug.Log("��� ������ ������������! ��������� ������...");

        // 1. ��������� �������� �������
        if (leftDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            leftDoorAnimator.SetTrigger(openAnimationTrigger);
        if (rightDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            rightDoorAnimator.SetTrigger(openAnimationTrigger);

        // 2. �������������� �������� UI ���������
        if (activationHintUI != null)
            activationHintUI.SetActive(false);

        // �����������: ��������� ������� �����, ����� ��������� ������ �� ����������
        // Collider col = GetComponent<Collider>();
        // if (col != null) col.enabled = false;
        // ��� ����� ������ ���������� �� ���� isOpen � OnTriggerEnter/Exit
    }

    // ����� ����� ������ � ������� �����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = true;
            // ���������� ��������� ������ ���� ������ ��� �� �������
            if (!isOpen && activationHintUI != null)
            {
                activationHintUI.SetActive(true);
                Debug.Log("����� ����� � ���� �������� ����� - ���������� ���������.");
            }
        }
    }

    // ����� ����� ������� �� �������� �����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = false;
            // �������� ���������, ���� ��� ���� �����
            if (activationHintUI != null && activationHintUI.activeSelf)
            {
                activationHintUI.SetActive(false);
                Debug.Log("����� ����� �� ���� ����� - �������� ���������.");
            }
        }
    }
}
