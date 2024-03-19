using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MultiSuika.Ball;
using MultiSuika.Utilities;

namespace MultiSuika.Container
{
    public class ContainerTracker : ItemTracker<ContainerInstance, ContainerTrackerInformation>
    {
        #region Singleton

        [SuppressMessage("ReSharper", "Unity.IncorrectMonoBehaviourInstantiation")]
        public static ContainerTracker Instance => _instance ??= new ContainerTracker();

        private static ContainerTracker _instance;

        private ContainerTracker()
        {
        }

        private void Awake()
        {
            _instance = this;
        }
        #endregion
        
        public ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)> OnContainerHit;
        private ActionMethodPlayerWrapper<(BallInstance, ContainerInstance)>[] _onContainerHit;

        protected override ContainerTrackerInformation CreateInformationInstance(ContainerInstance item, List<int> playerIndex)
        {
            return new ContainerTrackerInformation(item, playerIndex);
        }
    }

    public class ContainerTrackerInformation : ItemInformation<ContainerInstance>
    {
        public ContainerTrackerInformation(ContainerInstance item, List<int> playerIndex) : base(item, playerIndex)
        {
        }
    }
}

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