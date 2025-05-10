using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Needed for List<>
using System.Linq;             // Needed for FirstOrDefault()

// Helper class (PlatformRotationPair) remains the same as before
[System.Serializable]
public class PlatformRotationPair
{
    [Tooltip("Descriptive name for this puzzle element (optional)")]
    public string elementName = "Puzzle Element";

    [Tooltip("PlatformNotifier script on the platform object")]
    public PlatformNotifier platformScript;

    [Tooltip("Object that will rotate for this platform")]
    public Transform objectToRotate;

    [Header("Rotation Settings")]
    [Tooltip("Angle in degrees to rotate EACH TIME the player interacts (clockwise around Y)")]
    public float rotationIncrementDegrees = 90f;

    [Tooltip("Duration of the rotation animation in seconds")]
    public float rotationDuration = 1.0f;

    [Header("Puzzle Goal")]
    [Tooltip("The TARGET absolute rotation angle (Y-axis) required to solve this part of the puzzle (0-360 degrees)")]
    public float targetRotationY = 180f;

    [HideInInspector]
    public float currentAccumulatedRotationY = 0f;
}

// Main GameManager script
public class GameManager : MonoBehaviour
{
    [Header("Puzzle Configuration")]
    [Tooltip("List of all platform/rotating object pairs and their puzzle goals")]
    public List<PlatformRotationPair> platformPairs = new List<PlatformRotationPair>();

    [Tooltip("How close the current angle needs to be to the target angle (in degrees) to be considered 'correct'")]
    public float angleTolerance = 1.0f;

    [Header("Puzzle Reward - Gate Animation")]
    [Tooltip("Animator component on the LEFT gate door object")]
    public Animator gateDoorLeftAnimator; // <<< NEW: Reference to left door animator

    [Tooltip("Animator component on the RIGHT gate door object")]
    public Animator gateDoorRightAnimator; // <<< NEW: Reference to right door animator

    [Tooltip("The name of the Trigger parameter in the Animators to start the opening animation")]
    public string gateOpenTriggerName = "Open"; // <<< NEW: Name of the trigger parameter

    // --- Internal Variables ---
    private PlatformNotifier currentActivePlatform = null;
    private bool isAnyRotationInProgress = false;
    private bool isPuzzleSolved = false;

    public bool IsPuzzleSolved => isPuzzleSolved; // <<< НОВОЕ: Публичный геттер


    // --- Singleton Pattern ---
    // Делаем GameManager доступным из любого другого скрипта через GameManager.Instance
    public static GameManager Instance { get; private set; }

    void Awake() // Используем Awake, так как он вызывается раньше Start
    {
        // Классическая реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Раскомментируй, если GameManager должен сохраняться между сценами
        }
        else
        {
            // Если экземпляр уже существует, удаляем этот дубликат
            Debug.LogWarning("Обнаружен дубликат GameManager. Удаляю этот экземпляр.", this);
            Destroy(gameObject);
        }
    }

    // PlayerEnteredPlatform and PlayerExitedPlatform remain the same

    public void PlayerEnteredPlatform(PlatformNotifier platform)
    {
        currentActivePlatform = platform;
        Debug.Log($"GM: Player entered platform: {platform.gameObject.name}");
    }

    public void PlayerExitedPlatform(PlatformNotifier platform)
    {
        if (currentActivePlatform == platform)
        {
            currentActivePlatform = null;
            Debug.Log($"GM: Player exited platform: {platform.gameObject.name}");
        }
    }

    void Update()
    {
        if (currentActivePlatform != null && Input.GetKeyDown(KeyCode.E) && !isAnyRotationInProgress && !isPuzzleSolved)
        {
            PlatformRotationPair activePair = platformPairs.FirstOrDefault(pair => pair.platformScript == currentActivePlatform);

            if (activePair != null && activePair.objectToRotate != null)
            {
                Debug.Log($"GM: Starting rotation for object: {activePair.objectToRotate.name} (Platform: {currentActivePlatform.gameObject.name})");
                StartCoroutine(RotateObjectSmoothly(activePair));
            }
            else if (activePair != null && activePair.objectToRotate == null)
            {
                Debug.LogWarning($"GM: Active platform {currentActivePlatform.gameObject.name} is configured in GameManager, but 'objectToRotate' is not assigned!", this);
            }
            else
            {
                Debug.LogWarning($"GM: No configuration found for active platform {currentActivePlatform.gameObject.name} in GameManager's platformPairs list!", this);
            }
        }
    }

    private IEnumerator RotateObjectSmoothly(PlatformRotationPair pairToRotate)
    {
        isAnyRotationInProgress = true;

        Transform targetTransform = pairToRotate.objectToRotate;
        Quaternion startRotation = targetTransform.rotation;
        Quaternion rotationStep = Quaternion.Euler(0, pairToRotate.rotationIncrementDegrees, 0);
        Quaternion targetRotation = startRotation * rotationStep;
        float elapsedTime = 0f;

        while (elapsedTime < pairToRotate.rotationDuration)
        {
            targetTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / pairToRotate.rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.rotation = targetRotation;

        pairToRotate.currentAccumulatedRotationY += pairToRotate.rotationIncrementDegrees;
        pairToRotate.currentAccumulatedRotationY = NormalizeAngle(pairToRotate.currentAccumulatedRotationY);

        Debug.Log($"GM: Rotation of {targetTransform.name} complete. New accumulated angle: {pairToRotate.currentAccumulatedRotationY:F1} degrees.");

        isAnyRotationInProgress = false;

        // Check if the puzzle is solved AFTER rotation finishes and flag is reset
        CheckPuzzleSolved(); // <<< IMPORTANT: Moved check here
    }

    private void CheckPuzzleSolved()
    {
        // Don't check if already solved or if a rotation is currently animating
        if (isPuzzleSolved || isAnyRotationInProgress)
        {
            return;
        }

        bool allCorrect = true;
        foreach (PlatformRotationPair pair in platformPairs)
        {
            if (pair.objectToRotate == null) continue; // Skip invalid pairs

            float angleDifference = Mathf.Abs(Mathf.DeltaAngle(pair.currentAccumulatedRotationY, pair.targetRotationY));
            if (angleDifference > angleTolerance)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.LogWarning("--- GM: PUZZLE SOLVED! ---");
            isPuzzleSolved = true; // Set the flag first

            // --- Trigger Gate Animation ---
            OpenGates(); // <<< NEW: Call the gate opening method
            // -----------------------------
        }
    }

    // <<< NEW: Method to handle opening the gates >>>
    private void OpenGates()
    {
        Debug.Log("GM: Attempting to open gates...");

        bool openedAny = false; // Track if we successfully triggered at least one door

        // Trigger Left Door
        if (gateDoorLeftAnimator != null)
        {
            // Check if the trigger parameter exists to avoid errors
            if (HasParameter(gateDoorLeftAnimator, gateOpenTriggerName, AnimatorControllerParameterType.Trigger))
            {
                gateDoorLeftAnimator.SetTrigger(gateOpenTriggerName);
                Debug.Log($"GM: Trigger '{gateOpenTriggerName}' set on Left Gate Animator.");
                openedAny = true;
            }
            else
            {
                Debug.LogError($"GM: Animator on {gateDoorLeftAnimator.gameObject.name} does not have a Trigger parameter named '{gateOpenTriggerName}'!", gateDoorLeftAnimator.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("GM: Left Gate Animator reference is not set in the Inspector.", this);
        }

        // Trigger Right Door
        if (gateDoorRightAnimator != null)
        {
            if (HasParameter(gateDoorRightAnimator, gateOpenTriggerName, AnimatorControllerParameterType.Trigger))
            {
                gateDoorRightAnimator.SetTrigger(gateOpenTriggerName);
                Debug.Log($"GM: Trigger '{gateOpenTriggerName}' set on Right Gate Animator.");
                openedAny = true;
            }
            else
            {
                Debug.LogError($"GM: Animator on {gateDoorRightAnimator.gameObject.name} does not have a Trigger parameter named '{gateOpenTriggerName}'!", gateDoorRightAnimator.gameObject);
            }
        }
        else
        {
            Debug.LogWarning("GM: Right Gate Animator reference is not set in the Inspector.", this);
        }

        if (!openedAny)
        {
            Debug.LogError("GM: Failed to open gates - check Animator references and trigger name in the Inspector and Animator Controller.", this);
        }
    }

    // Helper function to check if an AnimatorControllerParameter exists
    // (Prevents errors if the trigger name is misspelled or doesn't exist)
    private bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType paramType)
    {
        if (animator == null || string.IsNullOrEmpty(paramName) || animator.runtimeAnimatorController == null)
            return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == paramType && param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }

    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle < 0)
        {
            angle += 360f;
        }
        return angle;
    }

    // Optional Start/InitializeRotations and Singleton Pattern remain the same
    // ... (You can keep or remove the commented-out Start/Awake/InitializeRotations)
}