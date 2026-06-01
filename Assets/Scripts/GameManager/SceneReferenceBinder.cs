using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReferenceBinder : MonoBehaviour
{
    public string playerTag = "Player";
    public bool bindOnStart = true;
    public bool bindOnSceneLoaded = true;
    public bool bindGenericReferences = true;

    private GameObject playerObject;
    private StatPlayer statPlayer;
    private Running running;
    private Animator playerAnimator;
    private Rigidbody2D playerRigidbody;

    private void OnEnable()
    {
        if (bindOnSceneLoaded)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        if (bindOnStart)
        {
            yield return null;
            yield return null;
            BindSceneReferences();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(BindAfterSceneLoaded());
    }

    private IEnumerator BindAfterSceneLoaded()
    {
        yield return null;
        yield return null;
        BindSceneReferences();
    }

    public void BindSceneReferences()
    {
        FindPlayer();

        if (playerObject == null || statPlayer == null)
        {
            Debug.LogError("SceneReferenceBinder could not find Player or StatPlayer.");
            return;
        }

        BindKnownReferences();

        if (bindGenericReferences)
        {
            BindGenericPlayerReferences();
        }

        Debug.Log("Scene references bound to Player.");
    }

    private void FindPlayer()
    {
        playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject == null)
        {
            statPlayer = FindObjectOfType<StatPlayer>(true);

            if (statPlayer != null)
            {
                playerObject = statPlayer.gameObject;
            }
        }

        if (playerObject == null)
        {
            running = FindObjectOfType<Running>(true);

            if (running != null)
            {
                playerObject = running.gameObject;
            }
        }

        if (playerObject == null)
        {
            return;
        }

        statPlayer = playerObject.GetComponent<StatPlayer>();
        running = playerObject.GetComponent<Running>();
        playerAnimator = playerObject.GetComponent<Animator>();
        playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
    }

    private void BindKnownReferences()
    {
        GameOver gameOver = FindObjectOfType<GameOver>(true);

        if (gameOver != null)
        {
            statPlayer.gameOverManager = gameOver;
        }

        UIManager[] uiManagers = FindObjectsOfType<UIManager>(true);

        foreach (UIManager uiManager in uiManagers)
        {
            uiManager.player = statPlayer;
        }

        Running[] runningScripts = FindObjectsOfType<Running>(true);

        foreach (Running runningScript in runningScripts)
        {
            if (runningScript.statPlayer == null)
            {
                runningScript.statPlayer = runningScript.GetComponent<StatPlayer>();
            }

            if (runningScript.statPlayer == null)
            {
                runningScript.statPlayer = statPlayer;
            }
        }

        InventoryController inventoryController = FindObjectOfType<InventoryController>(true);

        if (inventoryController != null)
        {
            SetPrivateField(inventoryController, "playerStats", statPlayer);
            inventoryController.RecalculatePlayerStats();
        }

        PlayerItemCollector[] collectors = FindObjectsOfType<PlayerItemCollector>(true);

        foreach (PlayerItemCollector collector in collectors)
        {
            if (inventoryController != null)
            {
                SetPrivateField(collector, "inventoryController", inventoryController);
            }
        }
    }

    private void BindGenericPlayerReferences()
    {
        MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>(true);

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null)
            {
                continue;
            }

            FieldInfo[] fields = behaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                if (!CanSetField(field))
                {
                    continue;
                }

                object currentValue = field.GetValue(behaviour);

                if (HasValidValue(currentValue))
                {
                    continue;
                }

                if (field.FieldType == typeof(StatPlayer))
                {
                    field.SetValue(behaviour, statPlayer);
                    continue;
                }

                if (field.FieldType == typeof(Running))
                {
                    field.SetValue(behaviour, running);
                    continue;
                }

                if (!ShouldBindAsPlayerReference(behaviour, field))
                {
                    continue;
                }

                if (field.FieldType == typeof(GameObject))
                {
                    field.SetValue(behaviour, playerObject);
                }
                else if (field.FieldType == typeof(Transform))
                {
                    field.SetValue(behaviour, playerObject.transform);
                }
                else if (field.FieldType == typeof(Animator))
                {
                    field.SetValue(behaviour, playerAnimator);
                }
                else if (field.FieldType == typeof(Rigidbody2D))
                {
                    field.SetValue(behaviour, playerRigidbody);
                }
            }
        }
    }

    private bool CanSetField(FieldInfo field)
    {
        if (field.IsStatic || field.IsInitOnly)
        {
            return false;
        }

        if (field.IsPublic)
        {
            return true;
        }

        return field.GetCustomAttribute<SerializeField>() != null;
    }

    private bool HasValidValue(object value)
    {
        if (value == null)
        {
            return false;
        }

        Object unityObject = value as Object;

        if (unityObject != null)
        {
            return true;
        }

        return true;
    }

    private bool ShouldBindAsPlayerReference(MonoBehaviour behaviour, FieldInfo field)
    {
        string fieldName = field.Name.ToLower();
        string scriptName = behaviour.GetType().Name.ToLower();

        if (fieldName.Contains("player"))
        {
            return true;
        }

        if (fieldName.Contains("target"))
        {
            if (scriptName.Contains("camera") || scriptName.Contains("follow") || scriptName.Contains("enemy") || scriptName.Contains("ai"))
            {
                return true;
            }
        }

        if (fieldName.Contains("follow"))
        {
            if (scriptName.Contains("camera") || scriptName.Contains("follow"))
            {
                return true;
            }
        }

        return false;
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, value);
        }
    }
}