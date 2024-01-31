using UnityEngine;

namespace MultiSuika.Utilities
{
    public class AnimatorDestroyOnExit : StateMachineBehaviour {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Destroy(animator.gameObject, stateInfo.length);
        }
    }
}