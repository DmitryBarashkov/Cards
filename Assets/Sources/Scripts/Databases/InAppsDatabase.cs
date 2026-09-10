using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InAppsDatabase", menuName = "Config/InApps Database")]
public class InAppsDatabase : ScriptableObject
{
    public List<ProductItem> Products = new List<ProductItem>();

    public void AddProductItem(string id, int price, string title, bool isOnceBuy = false)
    {
        Products.Add(new ProductItem { Id = id, Price = price, Title = title, IsOnceBuy = isOnceBuy });
    }

    public bool TryGetItem(string id, out ProductItem result)
    {
        foreach (var product in Products)
        {
            if (product.Id == id)
            {
                result = product;
                return true;
            }
        }

        result = default;
        return false;
    }

    [Serializable]
    public struct ProductItem
    {
        public string Id;
        public int Price;
        public string Title;
        public bool IsOnceBuy;
    }
}
