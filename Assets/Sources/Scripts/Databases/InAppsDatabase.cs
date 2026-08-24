using System;
using System.Collections.Generic;
using UnityEngine;
using static CardsDatabase;

[Serializable]
public class ProductItem
{
    public string id;
    public int price;
    public string title;
    public bool isOnceBuy;
}

[CreateAssetMenu(fileName = "InAppsDatabase", menuName = "Config/InApps Database")]
public class InAppsDatabase: ScriptableObject
{
    public List<ProductItem> products = new List<ProductItem>();

    public bool TryGetItem(string id, out ProductItem result)
    {
        foreach (var product in products)
        {
            if (product.id == id)
            {
                result = product;
                return true;
            }
        }

        result = default;
        return false;
    }
}
