using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public abstract class ItemTracker<T, U> where T : Component where U : ItemInformation<T>
    {
        private readonly List<U> _itemInformation = new List<U>();
        protected abstract U CreateInformationInstance(T item, int playerIndex);

        public void AddNewItem(T item, int playerIndex = -1)
        {
            if (GetInformationFromItem(item) != null)
            {
                Debug.LogError($"Adding following item to a tracker more than once : {item.gameObject.name} with player {playerIndex}");
                return;
            }

            _itemInformation.Add(CreateInformationInstance(item, playerIndex));
        }
        public virtual void ClearItem(T item)
        {
            var info = GetInformationFromItem(item);
            if (info == null)
                return;
            _itemInformation.Remove(info);
            GameObject.Destroy(item.gameObject);
        }

        public void ClearItems()
        {
            for (int i = _itemInformation.Count - 1; i >= 0; i--)
            {
                var item = _itemInformation[i].Item;
                ClearItem(item);
            }
        }

        public List<T> GetItems() =>
            _itemInformation
                .Select(info => info.Item)
                .ToList();

        public List<T> GetItemsFromPlayer(int playerIndex) =>
            _itemInformation
                .Where(info => info.PlayerIndex == playerIndex)
                .Select(info => info.Item)
                .ToList();

        public T GetItemFromPlayerOrDefault(int playerIndex)
        {
            var info = _itemInformation.FirstOrDefault(info => info.PlayerIndex == playerIndex);
            return info != null ? info.Item : GetItemByIndex(0);
        }

        public int GetPlayerFromItem(T item) => GetInformationFromItem(item).PlayerIndex;

        private U GetInformationFromItem(T item) =>
            _itemInformation.FirstOrDefault(c => c.Item == item);

        private T GetItemByIndex(int index)
        {
            if (index < 0 || index >= _itemInformation.Count)
                return default;
            return _itemInformation[index].Item;
        }
    }

    public class ItemInformation<T> where T : Component
    {
        public T Item { get; }
        public int PlayerIndex { get; }

        protected ItemInformation(T item, int playerIndex = -1)
        {
            Item = item;
            PlayerIndex = playerIndex;
        }
    }
}