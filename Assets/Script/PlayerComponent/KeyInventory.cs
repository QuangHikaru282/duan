using UnityEngine;
using System.Collections.Generic;

public enum KeyType
{
    BossKey,
    RuneStone
}

public class KeyInventory : MonoBehaviour
{
    private Dictionary<KeyType, int> keyInventory = new Dictionary<KeyType, int>();

    public void AddKey(KeyType type, int amount = 1)
    {
        if (keyInventory.ContainsKey(type))
            keyInventory[type] += amount;
        else
            keyInventory[type] = amount;

        UIUpdateLogic.Instance?.UpdateKeyUI(type, keyInventory[type]);
    }

    public bool UseKey(KeyType type, int amount = 1)
    {
        if (!HasKey(type, amount))
            return false;

        keyInventory[type] -= amount;
        if (keyInventory[type] <= 0)
            keyInventory.Remove(type);

        UIUpdateLogic.Instance?.UpdateKeyUI(type, GetKeyCount(type));
        return true;
    }

    public bool HasKey(KeyType type, int amount = 1)
    {
        return keyInventory.ContainsKey(type) && keyInventory[type] >= amount;
    }

    public int GetKeyCount(KeyType type)
    {
        return keyInventory.TryGetValue(type, out int count) ? count : 0;
    }
}
