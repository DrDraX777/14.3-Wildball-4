using UnityEngine;
using System.Collections; // ��� �������� ��������
using TMPro; // ��� UnityEngine.UI

[RequireComponent(typeof(Collider))] // ��������, ��� ���� ���������
public class RotatingPlate : MonoBehaviour
{
    [Header("��������� ��������")]
    [Tooltip("�������� �������� (�������� � �������)")]
    public float rotationSpeed = 180f;
    [Tooltip("������� ���� �������� �� Z (0, 90, 180, 270), � �������� ������ ������ �����")]
    [Range(0, 3)] // 0=0, 1=90, 2=180, 3=270
    public int targetRotationStep = 0; // ������ � ���������� ��� ������ �����! 0-3

    [Header("������")]
    [Tooltip("���������� �����, ������� ����� ���������")]
    public PuzzleGateOpener gateController; // ���������� ������ ����� ����
    [Tooltip("UI ������� ��� ��������� '������� E'")]
    public GameObject interactPromptUI; // ���������� UI �����/�������� ����

    [Header("��������������")]
    [Tooltip("��� ������� ������")]
    public string playerTag = "Player";

    // ��������� ���������� ���������
    private bool playerInRange = false;
    private bool isRotating = false;
    private int currentRotationStep = 0; // ������� ������� (0=0, 1=90, 2=180, 3=270)
    private bool isCorrectlyRotated = false;
    private Quaternion initialRotation; // �������� ��������� �������� ��� ������� �����

    void Start()
    {
        // �������� ������
        if (gateController == null)
            Debug.LogError($"����� '{gameObject.name}' �� ����� ������ �� PuzzleGateOpener!", this);
        if (interactPromptUI != null)
            interactPromptUI.SetActive(false);
        else
            Debug.LogWarning($"����� '{gameObject.name}' �� ����� ������ �� UI ��������� '������� �'.", this);

        // ��������� ����������
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"��������� �� ����� '{gameObject.name}' �� �������� ���������. ������������ isTrigger = true.", this);
            col.isTrigger = true;
        }

        // ���������� ��������� ��� �� �������� �������� (���������)
        initialRotation = transform.localRotation;
        // TODO: ���� ����� ���������� �� �� 0 ��������, ����� ������ ���������� currentRotationStep
        currentRotationStep = 0; // ���� ������������ ����� � 0 ��������

        CheckIfCorrect(); // ��������� ��������� ���������
    }

    void Update()
    {
        // ��������� ������� E ������ ���� ����� ����� � ����� �� ��������� ������
        if (playerInRange && !isRotating && Input.GetKeyDown(KeyCode.E))
        {
            StartRotation();
        }
    }

    // ����� ����� ������ � ������� �����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            // ���������� ��������� "������� �", ������ ���� ����� ��� �� � ���������� ���������
            if (interactPromptUI != null && !isCorrectlyRotated)
                interactPromptUI.SetActive(true);
            Debug.Log($"����� ����� � ���� ����� {gameObject.name}.");
        }
    }

    // ����� ����� ������� �� �������� �����
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            // �������� ��������� "������� �"
            if (interactPromptUI != null)
                interactPromptUI.SetActive(false);
            Debug.Log($"����� ����� �� ���� ����� {gameObject.name}.");
        }
    }

    // ��������� ������� ��������
    private void StartRotation()
    {
        if (isRotating) return; // ��� ���������

        isRotating = true;
        if (interactPromptUI != null) // ������ ��������� �� ����� ��������
            interactPromptUI.SetActive(false);

        // ��������� ��������� ��� � ������� ����
        int nextStep = (currentRotationStep + 1) % 4; // ���� 0 -> 1 -> 2 -> 3 -> 0
        float targetAngleZ = nextStep * 90.0f;

        // ������� ������� �������� (������������ ����������, ���� ��� ���� �� 0, ��� ������ �� Z)
        // ����� �������� � ����������� ������ 0, 90, 180, 270
        Quaternion targetRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, targetAngleZ);
        // Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, targetAngleZ); // ���� ������� ������������ ����������

        Debug.Log($"����� {gameObject.name}: ������� � ���� {currentRotationStep} �� ��� {nextStep} (���� {targetAngleZ})");
        StartCoroutine(RotateCoroutine(targetRotation, nextStep));
    }

    // �������� ��� �������� ��������
    IEnumerator RotateCoroutine(Quaternion targetLocalRotation, int nextStep)
    {
        Quaternion startLocalRotation = transform.localRotation;
        float time = 0;
        float duration = 90.0f / rotationSpeed; // ����� �� ������� �� 90 ��������

        while (time < duration)
        {
            transform.localRotation = Quaternion.Slerp(startLocalRotation, targetLocalRotation, time / duration);
            time += Time.deltaTime;
            yield return null; // ���� ���������� �����
        }

        // ����������� ������ �������� ���������
        transform.localRotation = targetLocalRotation;
        currentRotationStep = nextStep;
        isRotating = false;

        // ���������, ����� �� ����� � ���������� ���������
        CheckIfCorrect();
        // �������� ��������� �����, ���� ����� ��� ��� � ���� � ����� ��� ��� �� ����������
        if (playerInRange && interactPromptUI != null && !isCorrectlyRotated)
            interactPromptUI.SetActive(true);

        Debug.Log($"����� {gameObject.name}: �������� ��������� �� ���� {currentRotationStep}. ���������: {isCorrectlyRotated}");
    }

    // ���������, ��������� �� ����� � ������� ��������� � ���������� ���������� �����
    void CheckIfCorrect()
    {
        bool previouslyCorrect = isCorrectlyRotated;
        isCorrectlyRotated = (currentRotationStep == targetRotationStep);

        // ���� ��������� ���������� (���� ������������ -> ����� ����������, ��� ��������)
        if (isCorrectlyRotated != previouslyCorrect && gateController != null)
        {
            gateController.PlateStateChanged(isCorrectlyRotated);
        }

        // ���� ����� ����� ����������, �������� ��������� "�" �������� ��� ���� �����
        if (isCorrectlyRotated && interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
    }
}
