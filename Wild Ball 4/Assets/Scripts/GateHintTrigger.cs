using UnityEngine;
using TMPro; // ���������� ��� ������ � TextMeshPro

public class GateHintTrigger : MonoBehaviour
{
    [Header("��������� ���������")]
    [Tooltip("������� TextMeshProUGUI �� UI Canvas, ��� ����� ������������ ���������")]
    public TextMeshProUGUI hintTextElement;

    [Tooltip("����� ���������, ������� ����� ������� ������")]
    [TextArea(3, 5)]
    public string hintMessage = "����� ������� ����� - ������ ����������� �� ������.";

    [Header("��������� ��������")]
    [Tooltip("��� ������� ������, ������� ������ ������������ ���������")]
    public string playerTag = "Player";

    // --- ���������� ���������� ---
    private bool isPlayerInside = false;
    // private GameManager gameManagerInstance; // ������ �� GameManager (������� ����� Singleton)

    void Start()
    {
        // �������� ������� TextMeshPro �������� (�������� ��� ����)
        if (hintTextElement == null)
        {
            Debug.LogError($"[{this.GetType().Name}] �� �������� ������� TextMeshProUGUI � ���������� �� ������� {gameObject.name}!", this);
            this.enabled = false;
            return;
        }

        // // �������� ������ �� GameManager ����� Singleton
        // // ������ ��� � Start, �����������, ��� GameManager ��� ������ ���� Awake
        // gameManagerInstance = GameManager.Instance;
        // if (gameManagerInstance == null)
        // {
        //     Debug.LogError($"[{this.GetType().Name}] �� ������� ����� ��������� GameManager! ���������, ��� ������ GameManager ���������� � ����� � �������.", this);
        //     this.enabled = false; // �����������, ���� GameManager �� ������
        //     return;
        // }

        // �������� ����� ��������� ��� �������
        hintTextElement.gameObject.SetActive(false);
        isPlayerInside = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��������� ��� ��������� �������
        if (other.CompareTag(playerTag) && !isPlayerInside)
        {
            // <<< ����� ��������: ������ ��� �������� ���������, ��������, ��� ����������� �� ������ >>>
            // ���������� GameManager.Instance ��� ������� � �������� IsPuzzleSolved
            if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
            {
                // ���� ����������� ��� ������, ������ �� ������ (��������� �� �����)
                Debug.Log($"[{this.GetType().Name}] ����� ({other.name}) �����, �� ����������� ��� ������. ��������� �� ��������.");
                return; // ������� �� ������
            }

            // ���� ����������� �� ������ � ��������� ������� ��������
            if (hintTextElement != null)
            {
                isPlayerInside = true;
                Debug.Log($"[{this.GetType().Name}] ����� ({other.name}) ����� � ���� ��������� ����� (����������� �� ������).");
                hintTextElement.text = hintMessage;
                hintTextElement.gameObject.SetActive(true);
            }
            // ������� �������� �� ������, ���� GameManager.Instance ��� null (���� � Start ��� ������ ���� ��������)
            else if (GameManager.Instance == null)
            {
                Debug.LogError($"[{this.GetType().Name}] �� ������� �������� ������ � GameManager.Instance ��� ����� ������!", this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ������ ������ �������� ������� - ������ �������� ���������, ���� ��� ���� ��������
        if (other.CompareTag(playerTag))
        {
            if (hintTextElement != null)
            {
                // �� �����, ���� �� ��� ������� �������� (����� �����, ����� ���� �����),
                // ������ �����������, ��� ��� ����� ������ ��� ������.
                isPlayerInside = false; // ���������� ���� � ����� ������
                if (hintTextElement.gameObject.activeSelf) // �������� ������ ���� �������
                {
                    Debug.Log($"[{this.GetType().Name}] ����� ({other.name}) ������� ���� ��������� �����.");
                    hintTextElement.gameObject.SetActive(false);
                }
            }
        }
    }

    // <<< �����������: �������� � Update >>>
    // ���� �����, ����� ��������� ������� ����� ��, ��� ������ ����������� ��������,
    // ���� ���� ����� ��� ��� ����� � ��������.
    void Update()
    {
        // ���� ����� ������, ��������� ������, �� ����������� ������ ��� ���� ������...
        if (isPlayerInside && hintTextElement != null && hintTextElement.gameObject.activeSelf)
        {
            if (GameManager.Instance != null && GameManager.Instance.IsPuzzleSolved)
            {
                // ...���������� �������� ���������.
                Debug.Log($"[{this.GetType().Name}] ����������� ������, ���� ����� ������ ��������. �������� ���������.");
                hintTextElement.gameObject.SetActive(false);
                // ����� ����� ���������� isPlayerInside = false, ����� OnTriggerExit �� ������� ������ �� �����,
                // �� ��� �� �����������, ��� ��� �� ��������� activeSelf ����� ��������.
            }
            else if (GameManager.Instance == null)
            {
                Debug.LogError($"[{this.GetType().Name}] �� ������� �������� ������ � GameManager.Instance � Update!", this);
            }
        }
    }

    // OnDisable/OnDestroy �������� ��� ��������� (�����������)
}