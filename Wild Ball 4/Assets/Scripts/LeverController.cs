using UnityEngine;
using TMPro; // ��� using UnityEngine.UI;

public class LeverController : MonoBehaviour
{
    [Header("������")]
    [Tooltip("�������� ������ ������")]
    public Animator leverAnimator;
    [Tooltip("UI ����� ��������� ��� ������ (������� E)")]
    public GameObject leverHintUI;
    [Tooltip("������ �� ������ GateController �� �������� �����")]
    public GateController gateController; // ���� ���������� GateHintTrigger!

    [Header("���������")]
    [Tooltip("��� �������� ��� �������� ������")]
    public string pullTriggerName = "PullTrigger"; // ��� �� ���� 1
    [Tooltip("������� ��������������")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerIsNear = false;
    private bool isLeverPulled = false; // ����, ��� ����� ��� �����

    void Start()
    {
        // ������ ��������� ��� ������
        if (leverHintUI != null)
            leverHintUI.SetActive(false);

        // �������� ������� ������ �� ���������� �����
        if (gateController == null)
            Debug.LogError("GateController �� �������� � LeverController �� ������� " + gameObject.name);
    }

    void Update()
    {
        // ���� ����� �����, ����� ��� �� ����� � ����� ����� E
        if (playerIsNear && !isLeverPulled && Input.GetKeyDown(interactionKey))
        {
            ActivateLever();
        }
    }

    private void ActivateLever()
    {
        Debug.Log("����� ����������� �����!");
        isLeverPulled = true; // ��������, ��� ����� �����������

        // �������� ��������� � ������ ��������
        if (leverHintUI != null)
            leverHintUI.SetActive(false);

        // ��������� �������� ������
        if (leverAnimator != null)
            leverAnimator.SetTrigger(pullTriggerName);
        else Debug.LogError("�������� ������ �� �������� � LeverController!");

        // !!! �������� ����� �������� ����� �� GateController !!!
        if (gateController != null)
            gateController.OpenTheGate();
        else Debug.LogError("�� ���� ������� ������, GateController �� ��������!");

        // ����� �������������� ������� ������
        gameObject.GetComponent<Collider>().enabled = false;
        // gameObject.SetActive(false);
    }

    // --- ������ ������/������� ��������� ---
    private void OnTriggerEnter(Collider other)
    {
        // ���� ����� ��� �� ����� � ����� �����
        if (!isLeverPulled && other.CompareTag("Player"))
        {
            if (leverHintUI != null)
                leverHintUI.SetActive(true); // ���������� ��������� "������� E"
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���� ����� ����� (�������, ����� ����� ��� ���)
        if (other.CompareTag("Player"))
        {
            if (leverHintUI != null)
                leverHintUI.SetActive(false); // �������� ���������
            playerIsNear = false;
        }
    }
}
