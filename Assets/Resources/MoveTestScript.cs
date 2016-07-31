using UnityEngine;
using System.Collections;

public class MoveTestScript : MonoBehaviour {

	public float moveX;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.D)) {
			Move (moveX);
		}

		if (Input.GetKey (KeyCode.Q)) {
			Move (-moveX);
		}
	}

	void Move(float xMove){
		this.gameObject.transform.position = new Vector3 (gameObject.transform.position.x + xMove,gameObject.transform.position.y,gameObject.transform.position.z);
	}
}
