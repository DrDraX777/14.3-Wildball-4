using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PeriodicAnimatorTrigger : MonoBehaviour
{
    public float interval = 5.0f; // Интервал в секундах
    public string triggerName = "StartRotation"; // Точное имя твоего триггера!

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator не найден!"); enabled = false; return;
        }
        if (string.IsNullOrEmpty(triggerName))
        {
            Debug.LogError("Имя триггера не указано!"); enabled = false; return;
        }

        // Запустить TriggerAnimation через 'interval' секунд,
        // и затем повторять каждые 'interval' секунд
        InvokeRepeating("TriggerAnimation", interval, interval);
    }

    void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName); // Активируем триггер
            Debug.Log($"Trigger '{triggerName}' activated at {Time.time}");
        }
    }

}
