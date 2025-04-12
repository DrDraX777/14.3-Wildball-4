using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PeriodicAnimatorTrigger : MonoBehaviour
{
    public float interval = 5.0f; // �������� � ��������
    public string triggerName = "StartRotation"; // ������ ��� ������ ��������!

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator �� ������!"); enabled = false; return;
        }
        if (string.IsNullOrEmpty(triggerName))
        {
            Debug.LogError("��� �������� �� �������!"); enabled = false; return;
        }

        // ��������� TriggerAnimation ����� 'interval' ������,
        // � ����� ��������� ������ 'interval' ������
        InvokeRepeating("TriggerAnimation", interval, interval);
    }

    void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName); // ���������� �������
            Debug.Log($"Trigger '{triggerName}' activated at {Time.time}");
        }
    }

}
