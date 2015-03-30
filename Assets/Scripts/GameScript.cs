using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScript : MonoBehaviour {

	public ElementScript[] ElementTypes;
	public GameObject Timer;
	public GameObject Score;

	float time = 60.0f;
	int score = 0;
	ElementScript currentElement;
	public ElementScript[,] elements = new ElementScript[8,8];

	public void SelectElement (ElementScript element) {
		if (!this.GetComponent<Animator> ().GetBool ("Clear"))
			return;

		if (currentElement == null) {
			this.currentElement = element;
			this.currentElement.Pop ();
		}
		else {
			if (this.currentElement == element || !this.currentElement.IsNeighbour(element)) {
				element.Unpop();
				this.currentElement.Unpop();
				this.currentElement = null;
			}
			else
				this.moveElements(element, this.currentElement);
		}
	}

	void moveElements (ElementScript element, ElementScript currentElement)	{
		// check for undo
		Vector3 el1Pos = element.transform.position;
		Vector3 el2Pos = this.currentElement.transform.position;

		element.transform.position = el2Pos;
		this.currentElement.transform.position = el1Pos;
		this.UpdateGridPositions ();

		bool undo = !this.GotMatches ();

		// get back for move animation
		element.transform.position = el1Pos;
		this.currentElement.transform.position = el2Pos;
		this.UpdateGridPositions ();

		element.MoveTo (currentElement.transform.position, undo);
		this.currentElement.MoveTo (element.transform.position, undo);

		this.currentElement = null;
	}

	public void PrintGrid() {
		if (this.elements [0, 0] == null)
			return;
		string output = "";
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (this.elements [j, 7-i] != null)
					output += "\t" + this.elements [j, 7-i].Type;
				else
					output += "\tNull";
			}
			output += "\n";
		}
		Debug.Log (output);
	}

	public void UpdateGridPositions() {
		ElementScript[,] oldElements = (ElementScript[,]) elements.Clone ();
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (oldElements[i, j] != null) {
					Vector3 pos = oldElements[i, j].gameObject.transform.position;
					int newX = Mathf.CeilToInt(pos.x + 3.5f);
					int newY = Mathf.CeilToInt(pos.y + 3.5f);
					if(newX != i || newY != j) {
						this.elements[newX, newY] = oldElements[i, j];
						if (this.elements[i, j] == oldElements[i, j])
							this.elements[i, j] = null;
					}
				}
			}
		}
	}

	public Dictionary<ElementScript,Vector3> GetFallingElements() {
		Dictionary<ElementScript,Vector3> fallers = new Dictionary<ElementScript, Vector3> ();
		for (int i = 0; i < 8; i++) {
			bool falling = false;
			Vector3 fallTarget = Vector3.zero;
			for (int j = 0; j < 8; j++) {
				if (falling) {
					if (this.elements[i, j] != null) {
						fallers.Add(this.elements[i, j], fallTarget);
						fallTarget.y += 1.0f;
					}
				}
				else {
					if (this.elements[i, j] == null) {
						fallTarget = new Vector3 (i - 3.5f, j - 3.5f, 0);
						falling = true;
					}
				}
			}
		}
		return fallers;
	}

	public bool GotMatches () {
		return this.findMatches ().Count > 0;
	}

	public void RemoveElements (List<ElementScript> elts) {
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				if (elts.Contains (elements [i, j])) {
					this.elements [i, j] = null;
				}
			}
		}
	}

	public void Reduce (){
		List<List<ElementScript>> matches = this.findMatches();
		Animator animator = this.GetComponent<Animator>();

		if (matches.Count > 0) {
			List<ElementScript> affectedElements = new List<ElementScript>();
			foreach (var match in matches)
				foreach (ElementScript el in match)
					affectedElements.Add(el);
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					if (affectedElements.Contains(elements[i, j])) {
						this.elements[i, j].Score ();
						this.elements[i, j] = null;
						score += 1;
//						time += 1.0f;
						animator.SetInteger("Scoring", animator.GetInteger("Scoring") + 1);
					}
				}
			}
		}
		else
			this.GetComponent<Animator> ().SetBool ("Clear", true);
	}

	public void Fill (){
		if (ElementTypes.Length > 0) {
			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {
					if (this.elements [i, j] == null) {
						this.elements [i, j] = (ElementScript)Instantiate (ElementTypes [Random.Range (0, ElementTypes.Length)], new Vector3 (i - 3.5f, j - 3.5f, 0), Quaternion.identity);
					}
				}
			}
		}
	}

	List<List<ElementScript>> findMatches() {
		List<List<ElementScript>> matches = new List<List<ElementScript>> ();
		for (int i = 0; i < 8; i++) {
			foreach(var item in verticalMatches(i))
				this.addToMatches(matches, item);
		}
		for (int i = 0; i < 8; i++) {
			foreach(var item in horizontalMatches(i))
				this.addToMatches(matches, item);
		}
		return matches;
	}

	List<List<ElementScript>> verticalMatches(int line) {
		List<List<ElementScript>> matches = new List<List<ElementScript>> ();
		for (int i = 0; i < 2; i++) { // maximum 2 matches per line
			for (int j = 2; j < 8; j++) {
				if (elements[line,j-2].Type == elements[line,j-1].Type && elements[line,j-1].Type == elements[line,j].Type) {
					this.addToMatches(matches, new List<ElementScript>(){elements[line,j-2],elements[line,j-1],elements[line,j]});
				}
			}
		}
		return matches;
	}

	List<List<ElementScript>> horizontalMatches(int line) {
		List<List<ElementScript>> matches = new List<List<ElementScript>> ();
		for (int i = 0; i < 2; i++) { // maximum 2 matches per line
			for (int j = 2; j < 8; j++) {
				if (elements[j-2, line].Type == elements[j-1, line].Type && elements[j-1, line].Type == elements[j, line].Type) {
					this.addToMatches(matches, new List<ElementScript>(){elements[j-2, line],elements[j-1, line],elements[j, line]});
				}
			}
		}
		return matches;
	}

	List<List<ElementScript>> addToMatches(List<List<ElementScript>> matches, List<ElementScript> match) {
		foreach (var item in matches) {
			foreach (var matchElement in match) {
				if (item.Contains(matchElement)) {
					foreach (var matchEl in match)
						if (!item.Contains(matchEl))
							item.Add(matchEl);
					return matches;
				}
			}
		}
		matches.Add (match);
		return matches;
	}

	// Use this for initialization
	void Start () {
		//reduce ();
	}
	
	// Update is called once per frame
	void Update () {
		time -= Time.deltaTime;
		if (time < 0 || Input.GetKey (KeyCode.Q) || Input.GetKey (KeyCode.Escape))
			Application.LoadLevel (2);
		if (Timer) {
			Timer.GetComponent<UnityEngine.UI.Text>().text = Mathf.CeilToInt(time).ToString();
		}
		if (Score) {
			Score.GetComponent<UnityEngine.UI.Text>().text = score.ToString();
		}
	}
}
