using UnityEngine;
using System.Collections;

public class ElementMoveStateMachineBehaviour : StateMachineBehaviour {
	private Vector3 targetPosition;
	private Vector3 startPosition;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		this.targetPosition = new Vector3(animator.GetFloat("TargetX"), animator.GetFloat("TargetY"));
		this.startPosition = animator.transform.position;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		GameObject controller = GameObject.Find ("Controller");
		if (!animator.GetBool ("Undo")) {
			Animator gAnimator = controller.GetComponent<Animator> ();
			gAnimator.SetBool ("Clear", false);

			int falling = gAnimator.GetInteger("Falling");
			if (falling > 0)
				gAnimator.SetInteger("Falling", falling - 1);
			controller.GetComponent<GameScript> ().UpdateGridPositions ();
		}
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.transform.position = Vector3.Lerp (animator.transform.position, targetPosition, 0.1f);
		if (Vector3.Distance (animator.transform.position, targetPosition) < 0.01f) {
			animator.transform.position = targetPosition;

			if(!animator.GetBool ("Move")) {
				animator.SetBool ("Undo", false);
			}

			animator.SetBool ("Move", false);
			animator.SetBool ("Selected", false);

			// save original position for comeback
			animator.SetFloat("TargetX", this.startPosition.x);
			animator.SetFloat("TargetY", this.startPosition.y);
		}
	}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
