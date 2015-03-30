using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void StartGameButton() {
		Application.LoadLevel (1);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Q) || Input.GetKey (KeyCode.Escape))
			Application.Quit ();
	}
}
