using UnityEngine;

public class PuzzleGateOpener : MonoBehaviour
{
    [Header("������ �� ��������� �������")]
    [Tooltip("�������� ����� ������� �����")]
    public Animator leftDoorAnimator;
    [Tooltip("�������� ������ ������� �����")]
    public Animator rightDoorAnimator;

    [Header("��������� �����������")]
    [Tooltip("������� ����� ���� ������ ���� � ���������� ���������")]
    public int requiredCorrectPlates = 4; // ������ ��������� � ����������� ����
    [Tooltip("��� �������� � ���������� ������� ��� ��������")]
    public string openAnimationTrigger = "Open"; // ��� �������� � Animator'�� �������

    // ��������� ���������� ���������
    private int currentCorrectPlates = 0; // ������� ��������� ���������� ����
    private bool isOpen = false;          // ������ ��� �������?

    void Start()
    {
        // ��������
        if (leftDoorAnimator == null || rightDoorAnimator == null)
            Debug.LogError("�� ��������� ��������� ������� ����� �� PuzzleGateOpener!", this);
        currentCorrectPlates = 0;
        isOpen = false;
    }

    /// <summary>
    /// ���������� �� ������� RotatingPlate, ����� ��������� ����� (����������/������������) ��������.
    /// </summary>
    /// <param name="plateBecameCorrect">True - ���� ����� ����� ����������, False - ���� ����� ������������.</param>
    public void PlateStateChanged(bool plateBecameCorrect)
    {
        if (isOpen) return; // ���� ������ ��� �������, ������ �� ������

        if (plateBecameCorrect)
        {
            currentCorrectPlates++;
            Debug.Log($"���������� �����! ����� ����������: {currentCorrectPlates}/{requiredCorrectPlates}");
        }
        else
        {
            currentCorrectPlates--;
            Debug.Log($"����� ����� ������������! ����� ����������: {currentCorrectPlates}/{requiredCorrectPlates}");
        }

        // Clamp �� ������ ������ (���� �� ������ �������� �� �������)
        currentCorrectPlates = Mathf.Clamp(currentCorrectPlates, 0, requiredCorrectPlates);

        // ���������, ���������� �� ������ ����������
        if (currentCorrectPlates >= requiredCorrectPlates)
        {
            OpenGate();
        }
    }

    // ����� �������� �����
    private void OpenGate()
    {
        if (isOpen) return; // ������ �� ���������� ��������

        isOpen = true;
        Debug.Log("����������� ������! ��������� ������...");

        // ��������� �������� �� ����� ��������
        if (leftDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            leftDoorAnimator.SetTrigger(openAnimationTrigger);
        if (rightDoorAnimator != null && !string.IsNullOrEmpty(openAnimationTrigger))
            rightDoorAnimator.SetTrigger(openAnimationTrigger);

        // �����������: ��� ����� ��������� ���� ��������, �������� ���� � �.�.
    }
}

