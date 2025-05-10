// TeleportManager.cs
using UnityEngine;
using TMPro;
using System.Collections; // ��� ��������, ���� �����������

public class TeleportManager : MonoBehaviour
{
    [Header("������ �� �������")]
    public GameObject teleportA;
    public GameObject teleportB;
    public TextMeshProUGUI teleportHint;

    [Header("������ ���������")]
    public string hintTextEnterToTeleport = "������� E ����� �����������������";
    public string hintTextActivateTeleport = "������� E ����� ������������ ��������";
    public string hintTextFindSecondTeleport = "������� ������ �������� ��� ���������";

    [Header("��������� ������������")]
    [Tooltip("������, �� ������� ����� ����� ������ ��� ������ ���������")]
    public float teleportHeightOffset = 1.0f; // ������� �� 1.0f, 2.0f ����� ���� ��������� ��� ��������� ����������
    [Tooltip("�������� � �������� ����� ������������� �����������������")]
    public float teleportCooldown = 0.75f; // ������� �������� ��� ����������

    // ���������� ���������� ���������
    private bool playerIsNearTeleportA = false;
    private bool playerIsNearTeleportB = false;
    private bool teleportsAreActive = false;
    private float lastTeleportTimestamp = -Mathf.Infinity; // ����� ��������� ������������
    private Rigidbody playerRigidbody; // ������������ ������ �� Rigidbody ������

    void Start()
    {
        if (teleportHint != null)
            teleportHint.gameObject.SetActive(false);
        else
            Debug.LogWarning("TeleportManager: UI ��������� (teleportHint) �� ���������.");

        if (teleportA == null || teleportB == null)
        {
            Debug.LogError("TeleportManager: ���� ��� ��� ��������� (teleportA, teleportB) �� ���������! ������ ����� ��������.");
            enabled = false;
            return;
        }
        // ������������ ���� ������� ����������, ���� �� ��� ���� ���-��, ��� �� ������ �������� �� ���������
        // (��������, ���������� �������). ��� �����������.
    }

    void Update()
    {
        // 1. �������� ��������
        bool isOnCooldown = Time.time < lastTeleportTimestamp + teleportCooldown;

        // 2. ��������� ������� ������� E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOnCooldown)
            {
                Debug.Log("TeleportManager: ������� ������������ �� ����� ��������.");
                return; // ������ �� ������, ���� ������� �������
            }

            // ���� ������ ������ ��� ��� �������� ���, ���� �� �� ��������
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("TeleportManager: ����� � ����� 'Player' �� ������ � �����!");
                return;
            }

            // �������� Rigidbody ������ (����� ���������� � Start ��� ��� ������ ����������)
            if (playerRigidbody == null)
            {
                playerRigidbody = playerObject.GetComponent<Rigidbody>();
                if (playerRigidbody == null)
                {
                    Debug.LogError("TeleportManager: �� ������� ������ ����������� ��������� Rigidbody! ������������ ����� MovePosition ����������.");
                    return;
                }
            }

            Vector3? targetWorldPosition = null; // Nullable Vector3 ��� �������� ������� �������

            // �������� 1: ��������� ���������� (����� � ������� ��������� B, ��������� ���������)
            if (playerIsNearTeleportB && !teleportsAreActive)
            {
                ActivateTeleports(); // teleportsAreActive ���������� true
                targetWorldPosition = teleportA.transform.position + Vector3.up * teleportHeightOffset;

                // ����� ������������� ��������� ��������� ����� ������������
                playerIsNearTeleportA = true;
                playerIsNearTeleportB = false; // ����� ������� ���� B
                Debug.Log("TeleportManager: ��������� ������������. ����� ��������������� �� B � A.");
            }
            // �������� 2: ������������� ��� �������� ����������
            else if (teleportsAreActive)
            {
                if (playerIsNearTeleportA)
                {
                    targetWorldPosition = teleportB.transform.position + Vector3.up * teleportHeightOffset;
                    playerIsNearTeleportB = true;
                    playerIsNearTeleportA = false;
                    Debug.Log("TeleportManager: ����� ��������������� �� A � B.");
                }
                else if (playerIsNearTeleportB)
                {
                    targetWorldPosition = teleportA.transform.position + Vector3.up * teleportHeightOffset;
                    playerIsNearTeleportA = true;
                    playerIsNearTeleportB = false;
                    Debug.Log("TeleportManager: ����� ��������������� �� B � A.");
                }
            }

            // ���� ���� ���������� ������� �������, ���������� ������������
            if (targetWorldPosition.HasValue)
            {
                // ���������� MovePosition ��� ��������� ����������� �����������
                playerRigidbody.velocity = Vector3.zero; // �������� �������� ����� �������������, ����� �������� "���������"
                playerRigidbody.angularVelocity = Vector3.zero; // �������� ������� ��������
                playerRigidbody.MovePosition(targetWorldPosition.Value);

                lastTeleportTimestamp = Time.time; // ������������� �������

                // ��������� ��������� ��� ������ ��������������
                // ������ ��� �����, ��� ��� ����� playerIsNear... ��� ����������� � ��������� ���������
                UpdateTeleportHint();
                Debug.Log($"TeleportManager: ����� ��������� � {targetWorldPosition.Value}. ������� �����������.");
            }
            else
            {
                // ���� ����� ����� E, �� ������� ��� ������������ �� ���������
                Debug.Log("TeleportManager: ������ ������� E, �� ������� ��� ������������ �� ���������.");
            }
        }

        // 3. ���������� ���������, ���� �� ���� ������������ � ���� �����
        // ��� �����, ����� ��������� �����������, ����� ����� ������ ������/������� �� ���� ������
        // � �� ��������� �� �������� �� ���������� ������������.
        if (!Input.GetKeyDown(KeyCode.E) && !isOnCooldown)
        {
            //UpdateTeleportHint(); // ��������������, ���� �����, ����� ��������� ����������� ���������.
            // �� �����, ����� PlayerEntered/Exited �������� UpdateTeleportHint.
        }
    }

    // ���������� �� TeleportZone.cs, ����� ����� ������ � �������
    public void OnPlayerEnteredZone(GameObject enteredTeleportZone)
    {
        if (Time.time < lastTeleportTimestamp + teleportCooldown)
        {
            // ���� �� �� �������� �� ������������, �� ������ ����� playerIsNear...
            // ��� ��� ��� ���� ����������� ������������� � Update.
            // �� �� ����� �������� ���������, ���� ��� �� ������������� �������� ���������.
            UpdateTeleportHint();
            return;
        }

        if (enteredTeleportZone == teleportA)
        {
            playerIsNearTeleportA = true;
        }
        else if (enteredTeleportZone == teleportB)
        {
            playerIsNearTeleportB = true;
        }
        UpdateTeleportHint();
        Debug.Log($"TeleportManager: ����� ����� � ���� {(enteredTeleportZone == teleportA ? "A" : "B")}. �����: A={playerIsNearTeleportA}, B={playerIsNearTeleportB}");
    }

    // ���������� �� TeleportZone.cs, ����� ����� ������� �� ��������
    public void OnPlayerExitedZone(GameObject exitedTeleportZone)
    {
        // ���� �� ����� �� ���� ����� ����� ������������ (�.�. �� ��������),
        // ����� playerIsNear... ��� ������ ���� ���� ����������� � Update � ���������
        // "� �������� ���������". ���� �� ����� �� ��������� ��������� - ��� ���������.
        // ���� �� ����� �� �������� ��������� �� ����� �������� - ������ ����� ������ ��������.

        bool updateHintAfterExit = true;

        if (exitedTeleportZone == teleportA)
        {
            if (playerIsNearTeleportA) playerIsNearTeleportA = false;
        }
        else if (exitedTeleportZone == teleportB)
        {
            if (playerIsNearTeleportB) playerIsNearTeleportB = false;
        }

        // ��������� ���������, ������ ���� �� �� ��������, ��� ���� ����� �� ���� ���
        if (Time.time >= lastTeleportTimestamp + teleportCooldown || (!playerIsNearTeleportA && !playerIsNearTeleportB))
        {
            UpdateTeleportHint();
        }
        Debug.Log($"TeleportManager: ����� ������� ���� {(exitedTeleportZone == teleportA ? "A" : "B")}. �����: A={playerIsNearTeleportA}, B={playerIsNearTeleportB}");
    }

    private void ActivateTeleports()
    {
        teleportsAreActive = true;
        // ���������� ������� ��������� ���������� ����� �������� �����
        Debug.Log("TeleportManager: ��������� ������������!");
        // ��������� ����� ��������� � Update ����� ������������ ��� � UpdateTeleportHint
    }

    private void UpdateTeleportHint()
    {
        if (teleportHint == null) return;

        string messageToShow = null;

        if (playerIsNearTeleportA)
        {
            if (!teleportsAreActive) messageToShow = hintTextFindSecondTeleport;
            else messageToShow = hintTextEnterToTeleport;
        }
        else if (playerIsNearTeleportB)
        {
            if (!teleportsAreActive) messageToShow = hintTextActivateTeleport; // ��������� ���������
            else messageToShow = hintTextEnterToTeleport;
        }

        if (string.IsNullOrEmpty(messageToShow))
        {
            teleportHint.gameObject.SetActive(false);
        }
        else
        {
            teleportHint.text = messageToShow;
            teleportHint.gameObject.SetActive(true);
        }
    }

    // ��������� ����� ��� �������� ��������� (���� ����� ������ ��������)
    public bool AreTeleportsActive()
    {
        return teleportsAreActive;
    }
}