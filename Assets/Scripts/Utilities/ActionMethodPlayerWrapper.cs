using System;
using System.Collections.Generic;

namespace MultiSuika.Utilities
{
    public class ActionMethodPlayerWrapper<TArgs>
    {
        private Dictionary<int, ActionMethod<TArgs>> _actionMethods = new Dictionary<int, ActionMethod<TArgs>>();
    
        public void Subscribe(Action<TArgs> method, int playerIndex)
        {
            if (!_actionMethods.ContainsKey(playerIndex))
                _actionMethods.Add(playerIndex, new ActionMethod<TArgs>());
            _actionMethods[playerIndex].Subscribe(method);
        }

        public void Unsubscribe(Action<TArgs> method, int playerIndex)
        {
            if (!_actionMethods.ContainsKey(playerIndex))
                return;
            _actionMethods[playerIndex].Unsubscribe(method);
        }

        public void Clear(int playerIndex)
        {
            if (!_actionMethods.ContainsKey(playerIndex))
                return;
            _actionMethods[playerIndex].Clear();
        }

        public void ClearAll()
        {
            foreach (var actionMethod in _actionMethods)
            {
                actionMethod.Value.Clear();
            }
        }

        public void CallAction(TArgs args, int playerIndex)
        {
            if (!_actionMethods.ContainsKey(playerIndex))
                return;
            _actionMethods[playerIndex].CallAction(args);
        }
    }
}