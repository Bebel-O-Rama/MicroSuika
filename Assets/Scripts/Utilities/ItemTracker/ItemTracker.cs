using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public abstract class ItemTracker<T> : MonoBehaviour where T : Component
    {
        protected readonly List<ItemInformation<T>> _itemInformation = new List<ItemInformation<T>>();
        
        public virtual void AddNewItem(T item, List<int> playerIndex = null)
        {
            _itemInformation.Add(new ItemInformation<T>(item, playerIndex));
        }

        public virtual void AddNewItem(T item, int playerIndex)
        {
            _itemInformation.Add(new ItemInformation<T>(item, playerIndex));
        }
        
        public virtual void ClearItem(T item)
        {
            var info = GetInformationFromItem(item);
            Destroy(item.gameObject);
            _itemInformation.Remove(info);
        }

        public void ClearItems()
        {
            Debug.Log("Start");
            for (int i = _itemInformation.Count - 1; i >= 0; i--)
                ClearItem(_itemInformation[i].Item);
            Debug.Log("Finish");
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

        public List<T> GetItemsByPlayer(int playerIndex) =>
            _itemInformation
                .Where(info => info.ContainsPlayerIndex(playerIndex))
                .Select(info => info.Item)
                .ToList();

        public List<int> GetPlayersByItem(T item) =>
            _itemInformation
                .Where(info => info.CompareItem(item))
                .SelectMany(info => info.PlayerIndex)
                .Distinct()
                .OrderBy(index => index)
                .ToList();

        protected bool IsItemRegistered(T item) => GetInformationFromItem(item) != null;

        private ItemInformation<T> GetInformationFromItem(T item) =>
            _itemInformation.FirstOrDefault(c => c.CompareItem(item));
    }

    public class ItemInformation<T> where T : Component
    {
        public T Item { get; }
        public List<int> PlayerIndex { get; }

        public ItemInformation(T item, List<int> playerIndex = null)
        {
            Item = item;
            PlayerIndex = new List<int>(playerIndex);
        }
        
        public ItemInformation(T item, int playerIndex)
        {
            Item = item;
            PlayerIndex = new List<int>() { playerIndex };
        }

        public virtual bool CompareItem(T item) => Item == item;
        
        public bool ContainsPlayerIndex(int playerIndex) => PlayerIndex.Contains(playerIndex);

        public void AddPlayerIndex(int playerIndex) => AddPlayerIndex(new List<int>(playerIndex));
        public void AddPlayerIndex(List<int> playerIndex) => PlayerIndex.AddRange(playerIndex.Except(PlayerIndex));
        public void RemovePlayerIndex(int playerIndex) => RemovePlayerIndex(new List<int>(playerIndex));
        public void RemovePlayerIndex(List<int> playerIndex) => PlayerIndex.RemoveAll(playerIndex.Contains);



    }
}