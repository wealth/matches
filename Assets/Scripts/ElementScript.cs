using UnityEngine;
using System.Collections;

public class ElementScript : MonoBehaviour {
	private GameScript gameController;

	public enum EType
	{
		Black,
		White,
		Pink,
		Red,
		Violet,
		Blue,
		Green
	}

	public EType Type;

	public void MoveTo(Vector3 direction, bool undo) {
		this.GetComponent<Animator> ().SetFloat ("TargetX", direction.x);
		this.GetComponent<Animator> ().SetFloat ("TargetY", direction.y);
		this.GetComponent<Animator> ().SetBool ("Move", true);
		this.GetComponent<Animator> ().SetBool ("Undo", undo);
	}

	public void Score() {
		this.GetComponent<Animator> ().SetBool ("Dying", true);
	}

	public void Die() {
		Animator animator = this.gameController.GetComponent<Animator> ();
		animator.SetInteger("Scoring", animator.GetInteger("Scoring") - 1);
		Destroy (this.gameObject);
	}

	public void Pop() {
		this.GetComponent<Animator> ().SetBool ("Selected", true);
	}

	public void Unpop() {
		this.GetComponent<Animator> ().SetBool ("Selected", false);
	}

	public bool IsNeighbour(ElementScript element){
		return Vector3.Distance (this.transform.position, element.transform.position) < 1.1f;
	}

	void OnMouseUp(){
		this.gameController.SelectElement (this);
	}

	void Awake () {
		this.gameController = GameObject.Find ("Controller").GetComponent<GameScript> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
}
