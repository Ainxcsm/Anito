using System;
using TMPro;
using UnityEngine;

public enum ItemEffectType
{
    MaxHealth,
    Speed,
    MeleeDamage,
    Defense,
    HealOverTime,
    DamageOverTime,
    LifeSteal,
    CooldownReduction,
    CritChance,
    CritDamage,
    AttackSpeed,
    EnemyArmorReduction,
    Evasion
}

public enum CooldownReductionTarget
{
    Dash,
    Skill
}

[Serializable]
public class ItemEffect
{
    public ItemEffectType effectType;
    public float amount = 1f;
    public bool scaleWithQuantity = true;

    [Header("Only for CooldownReduction")]
    public CooldownReductionTarget cooldownTarget;

    [Header("Only for DamageOverTime / ArmorReduction")]
    public float duration = 3f;
}

public class Item : MonoBehaviour
{
    public int ID;
    public string itemKey;
    public string Name;
    public Sprite icon;
    public GameObject uiPrefab;
    public int quantity = 1;

    [Header("Passive Inventory Effects")]
    public bool hasPassiveEffect = false;
    public ItemEffect[] effects;

    private TMP_Text quantityText;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = gameObject.name;
        }

        if (string.IsNullOrWhiteSpace(itemKey))
        {
            itemKey = Name.Trim().ToLower().Replace(" ", "_");
        }
    }

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = gameObject.name;
        }

        if (string.IsNullOrWhiteSpace(itemKey))
        {
            itemKey = Name.Trim().ToLower().Replace(" ", "_");
        }

        quantityText = GetComponentInChildren<TMP_Text>(true);
        UpdateQuantityDisplay();
    }

    public string GetStackKey()
    {
        if (!string.IsNullOrWhiteSpace(itemKey))
        {
            return itemKey;
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            return Name.Trim().ToLower().Replace(" ", "_");
        }

        return gameObject.name.Trim().ToLower().Replace(" ", "_");
    }

    public bool CanStackWith(Item otherItem)
    {
        if (otherItem == null)
        {
            return false;
        }

        return GetStackKey() == otherItem.GetStackKey();
    }

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public void Pickup()
    {
        Debug.Log("Pickup triggered: " + Name);

        if (ItemPickUIController.Instance != null)
        {
            ItemPickUIController.Instance.ShowItemPickup(Name, icon);
        }
        else
        {
            Debug.LogError("ItemPickUIController Instance is NULL.");
        }
    }
}