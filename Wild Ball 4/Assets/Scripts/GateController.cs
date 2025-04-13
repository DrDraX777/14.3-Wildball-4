using UnityEngine;
using TMPro; // ��� using UnityEngine.UI;

public class GateController : MonoBehaviour
{
    [Header("������ �� �������")]
    [Tooltip("�������� ����� ������� �����")]
    public Animator leftDoorAnimator;
    [Tooltip("�������� ������ ������� �����")]
    public Animator rightDoorAnimator;

    [Header("UI � ��������")]
    [Tooltip("UI ����� ��������� ��� ����� (����� �����)")]
    public GameObject gateHintUI;
    [Tooltip("��� �������� ��� �������� �������� �������")]
    public string openTriggerName = "OpenTrigger"; // ��� �� ���� 1

    private bool isGateOpen = false; // ����, ��� ������ ��� �������

    void Start()
    {
        // ��������, ��� ��������� ������ ��� ������
        if (gateHintUI != null)
            gateHintUI.SetActive(false);
    }

    // ��������� �����, ������� ����� ���������� �������
    public void OpenTheGate()
    {
        if (!isGateOpen)
        {
            Debug.Log("�������� ������� �� �������� �����!");
            isGateOpen = true;

            // ��������� �������� ����� �������
            if (leftDoorAnimator != null)
                leftDoorAnimator.SetTrigger(openTriggerName);
            else Debug.LogError("�������� ����� ������� �� �������� � GateController!");

            if (rightDoorAnimator != null)
                rightDoorAnimator.SetTrigger(openTriggerName);
            else Debug.LogError("�������� ������ ������� �� �������� � GateController!");

            // �������� ��������� � ����� ��������
            if (gateHintUI != null)
                gateHintUI.SetActive(false);

            // ����� �������������� ��� �������, ����� �� ������ �� ����������
            gameObject.GetComponent<Collider>().enabled = false;
            // ��� ��������� �������������� ������ ��������
            // gameObject.SetActive(false);
        }
    }

    // --- ������ ������/������� ��������� ---
    private void OnTriggerEnter(Collider other)
    {
        // ���� ������ ��� �� ������� � ����� �����
        if (!isGateOpen && other.CompareTag("Player"))
        {
            if (gateHintUI != null)
                gateHintUI.SetActive(true); // ���������� ��������� "����� �����"
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���� ����� ����� (�������, ������� ������ ��� ���, ��������� ������ �������� ��� ������)
        if (other.CompareTag("Player"))
        {
            if (gateHintUI != null)
                gateHintUI.SetActive(false); // �������� ���������
        }
    }
}
