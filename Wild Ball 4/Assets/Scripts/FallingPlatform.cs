using UnityEngine;
using System.Collections; // ���������� ��� ������������� ������� (IEnumerator)

// ��������� ���������� ����� ��������� Rigidbody �� ��� �� �������,
// ����� �������� ������, ���� ��� ������� �������� �������.
[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("��������� �������")]
    [Tooltip("��� ������� ������, ������� ���������� �������")]
    public string playerTag = "Player";

    [Tooltip("����� �������� ����� ������� ������� (� ��������)")]
    public float vibrationDuration = 0.5f;

    [Tooltip("���� (���������) �������� - ��������� ������ ��������� ���������")]
    public float vibrationMagnitude = 0.05f;

    [Tooltip("�������� ����� ������������ ������� ����� ������ ������� (� ��������)")]
    public float destroyDelay = 2.0f;

    // --- ���������� ���������� ---
    private Rigidbody rb;                  // ������ �� ��������� Rigidbody
    private Vector3 originalPosition;      // �������� ������� ��������� ��� ��������
    private bool isTriggered = false;      // ����, ���� �� ��������� ��� ������������
    private Coroutine vibrationCoroutine = null; // ������ �� �������� �������� ��������

    // ���������� ���� ��� ��� ������������� �������
    void Awake()
    {
        // �������� ��������� Rigidbody
        rb = GetComponent<Rigidbody>();

        // ���������, ������ �� Rigidbody (���� RequireComponent ������ ��� �������������)
        if (rb == null)
        {
            Debug.LogError($"�� ������� {gameObject.name} ����������� ��������� Rigidbody!", this);
            enabled = false; // ��������� ������, ���� ��� Rigidbody
            return;
        }

        // ��������� ��������� ������� ���������
        originalPosition = transform.position;

        // ��������, ��� ��������� ���������� �������� � �� ���������� ����������.
        // �� ������� �� kinematic, ����� �������� (��������� transform.position)
        // �������� ��������� � �� ������������� � ������� �� ������ �������.
        rb.isKinematic = true;
        rb.useGravity = false; // ���������� ���������, ���� ��������� kinematic
    }

    // ����������, ����� ������ Collider ������ � ������� � Collider'�� ����� �������
    void OnCollisionEnter(Collision collision)
    {
        // ���������:
        // 1. �� ���� �� ��������� ��� ������������ (isTriggered == false)
        // 2. ���������� �� ������ � ������ ����� (playerTag)
        if (!isTriggered && collision.gameObject.CompareTag(playerTag))
        {
            // �������������� ��������: ��������, ��� ����� �������� ������.
            // ��� ������������ ������������, ���� ����� �������� ����� ��� �����.
            // �� ��������� ������� ����� ��������. ���� ��� ���������� � �������� ����,
            // ������, ������������ ��������� ������ ���������.
            ContactPoint contact = collision.GetContact(0); // ����� ������ ����� ��������
            // Vector3.down - ��� (0, -1, 0). ��������� ������������ > 0 ��������, ��� ���� < 90 ��������.
            // ���������� ��������� ��������� �������� (��������, 0.7), ����� ���� ����������,
            // ��� ������� ��������� ���������� ������ � ������������� ����������� ������.
            if (Vector3.Dot(contact.normal, Vector3.down) > 0.7f)
            {
                Debug.Log($"��������� {gameObject.name} ������������ ������� {collision.gameObject.name}");
                isTriggered = true; // ������������� ����, ����� ������������� ��������� ������������

                // ��������� ��������, ������� �������� ��� ������������������ ��������
                StartCoroutine(TriggerFallingSequence());
            }
        }
    }

    // ��������, ����������� �������������������: �������� -> ������� -> �����������
    private IEnumerator TriggerFallingSequence()
    {
        // --- ���� 1: �������� ---
        // ��������� �������� �������� � ���� �� ����������
        vibrationCoroutine = StartCoroutine(VibratePlatform());
        yield return vibrationCoroutine; // ����, ���� VibratePlatform() �� ����������
        vibrationCoroutine = null; // ���������� ������ �� ��������

        // --- ���� 2: ������� ---
        // ������ ��������� �� kinematic, ����� �� ��� ����������� ������ (����������)
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true; // �������� ����������
        }
        Debug.Log($"��������� {gameObject.name} ������ ������.");

        // --- ���� 3: ����������� ---
        // ������������� ����������� ����� �������� ������� ����� destroyDelay ������
        // ����� ������ �������.
        Destroy(gameObject, destroyDelay);
    }

    // �������� ��� ������� ��������
    private IEnumerator VibratePlatform()
    {
        float elapsedTime = 0f; // ������� ���������� ������� ��������
        Debug.Log($"��������� {gameObject.name} ���������...");

        // ���� �����������, ���� �� ������� ����� vibrationDuration
        while (elapsedTime < vibrationDuration)
        {
            // ���������� ��������� ��������� �������� �� ����������� (��� X � Z)
            float offsetX = Random.Range(-1f, 1f) * vibrationMagnitude;
            float offsetY = 0f; // �� ��������� �� ���������
            float offsetZ = Random.Range(-1f, 1f) * vibrationMagnitude;

            // ��������� �������� � �������� �������, ����� ��������� �� "�������" ������
            transform.position = originalPosition + new Vector3(offsetX, offsetY, offsetZ);

            // ����������� ������� �������
            elapsedTime += Time.deltaTime;

            // �������� ���������� �� ���������� �����
            yield return null;
        }

        // ����� ���������� ����� �������� ���������� ��������� ����� � �������� ���������.
        // ��� �����, ����� ������� �������� �� ���������� �����.
        transform.position = originalPosition;
        Debug.Log($"��������� {gameObject.name} ��������� �����������.");
    }

    // �����������: ���� ������ ����������� ��� ������������ �� ���������� ��������,
    // ������������� �������� � ���������� �������.
    void OnDisable()
    {
        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
            transform.position = originalPosition; // ������� �� ����� �� ������ ������
            vibrationCoroutine = null;
        }
    }
}