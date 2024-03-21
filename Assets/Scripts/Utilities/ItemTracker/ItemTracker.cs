using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiSuika.Utilities
{
    public abstract class ItemTracker<T, U> where T : Component where U : ItemInformation<T>
    {
        protected readonly List<U> _itemInformation = new List<U>();
        protected abstract U CreateInformationInstance(T item, List<int> playerIndex);

        public void AddNewItem(T item, int playerIndex) =>
            AddNewItem(item, new List<int> { playerIndex });

        public void AddNewItem(T item, List<int> playerIndex = null) =>
            _itemInformation.Add(CreateInformationInstance(item, playerIndex));

        public virtual void ClearItem(T item)
        {
            var info = GetInformationFromItem(item);
            if (info == null)
                return;
            GameObject.Destroy(item.gameObject);
            _itemInformation.Remove(info);
        }

        public void ClearItems()
        {
            for (int i = _itemInformation.Count - 1; i >= 0; i--)
            {
                var item = _itemInformation[i].Item;
                if (item != null)
                    ClearItem(_itemInformation[i].Item);
                else
                    _itemInformation.RemoveAt(i);
            }
        }

        public void SetPlayerForItem(int playerIndex, T item, bool isAdding = true) =>
            SetPlayerForItem(new List<int> { playerIndex }, item, isAdding);


        public void SetPlayerForItem(List<int> playerIndex, T item, bool isAdding = true)
        {
            _itemInformation
                .Where(info => info.CompareItem(item))
                .ToList()
                .ForEach(info =>
                {
                    if (isAdding)
                        info.AddPlayerIndex(playerIndex);
                    else
                        info.RemovePlayerIndex(playerIndex);
                });
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

        private U GetInformationFromItem(T item) =>
            _itemInformation.FirstOrDefault(c => c.CompareItem(item));

        private bool IsItemRegistered(T item) => GetInformationFromItem(item) != null;
    }

    public class ItemInformation<T> where T : Component
    {
        public T Item { get; }
        public List<int> PlayerIndex { get; }

        public ItemInformation(T item, List<int> playerIndex = null)
        {
            Item = item;
            PlayerIndex = playerIndex?.ToList() ?? new List<int>();
        }

        public virtual bool CompareItem(T item) => Item == item;

        public bool ContainsPlayerIndex(int playerIndex) => PlayerIndex.Contains(playerIndex);

        public void AddPlayerIndex(int playerIndex) => AddPlayerIndex(new List<int> { playerIndex });
        public void AddPlayerIndex(List<int> playerIndex) => PlayerIndex.AddRange(playerIndex.Except(PlayerIndex));
        public void RemovePlayerIndex(int playerIndex) => RemovePlayerIndex(new List<int> { playerIndex });
        public void RemovePlayerIndex(List<int> playerIndex) => PlayerIndex.RemoveAll(playerIndex.Contains);
    }
}