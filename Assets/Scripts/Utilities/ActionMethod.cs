using System;
using System.Collections.Generic;

namespace MultiSuika.Utilities
{
    public class ActionMethod<TArgs>
    {
        private HashSet<Action<TArgs>> _actions = new HashSet<Action<TArgs>>();
        public void Subscribe(Action<TArgs> method)
        {
            _actions.Add(method);
        }

        public void Unsubscribe(Action<TArgs> method)
        {
            _actions.Remove(method);
        }

        public void Clear()
        {
            _actions.Clear();
        }

        public void CallAction(TArgs args)
        {
            foreach (var subscriber in _actions)
            {
                subscriber?.Invoke(args);
            }
        }
    }
}
