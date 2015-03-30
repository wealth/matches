using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallGridStateMachineBehaviour : StateMachineBehaviour {
	private Dictionary<ElementScript,Vector3> fallers;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.fallers = animator.GetComponent<GameScript> ().GetFallingElements ();
		foreach (ElementScript element in this.fallers.Keys) {
			element.MoveTo(this.fallers[element], false);
		}
		animator.SetInteger("Falling", this.fallers.Keys.Count);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool ("Filled", false);
		animator.gameObject.GetComponent<GameScript> ().UpdateGridPositions ();
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
