using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public abstract class ItemTracker<T> : MonoBehaviour
    {
        private readonly List<ItemInformation<T>> _itemInformation = new List<ItemInformation<T>>();

        public abstract void AddNewItem(T item);
        public abstract void ClearItem(T item);

        public void ClearItems()
        {
            for (int i = _itemInformation.Count - 1; i >= 0; i--)
                ClearItem(_itemInformation[i].Item);
        }

        public T GetItemByIndex(int index)
        {
            if (index < 0 || index >= _itemInformation.Count)
                return default;
            return _itemInformation[index].Item;
        }

        public List<T> GetItems() =>
            _itemInformation
                .Select(info => info.Item)
                .ToList();

        public List<T> GetItemsWithPlayer(int playerIndex) =>
            _itemInformation
                .Where(info => info.ContainsPlayerIndex(playerIndex))
                .Select(info => info.Item)
                .ToList();

        public List<int> GetPlayersWithItem(T item) =>
            _itemInformation
                .Where(info => info.CompareItem(item))
                .SelectMany(info => info.PlayerIndex)
                .Distinct()
                .OrderBy(index => index)
                .ToList();


        private bool IsItemRegistered(T item) => GetInformationFromItem(item) != null;

        private ItemInformation<T> GetInformationFromItem(T item) =>
            _itemInformation.FirstOrDefault(c => c.CompareItem(item));
    }

    public abstract class ItemInformation<T>
    {
        public T Item { get; }
        public List<int> PlayerIndex { get; }

        public abstract bool CompareItem(T item);
        public bool ContainsPlayerIndex(int playerIndex) => PlayerIndex.Contains(playerIndex);
    }
}