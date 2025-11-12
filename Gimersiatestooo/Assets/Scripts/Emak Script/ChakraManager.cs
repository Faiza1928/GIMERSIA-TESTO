using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChakraManager : MonoBehaviour
{
    [Header("Chakra Settings")]
    public int chakra = 0;
    public int maxChakra = 3;

    [Header("UI Bars")]
    public List<Image> chakraBars; // drag 3 bar kuning di inspector

    void Start()
    {
        UpdateChakraUI();
    }

    public void GainChakra()
    {
        chakra = Mathf.Min(chakra + 1, maxChakra);
        UpdateChakraUI();
    }

    public void UseChakra(int amount = 1)
    {
        chakra = Mathf.Max(chakra - amount, 0);
        UpdateChakraUI();
    }

    public bool HasEnoughChakra(int amount = 1)
    {
        return chakra >= amount;
    }

    private void UpdateChakraUI()
    {
        for (int i = 0; i < chakraBars.Count; i++)
        {
            chakraBars[i].enabled = (i < chakra);
        }
    }
}
