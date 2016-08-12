using UnityEngine;
using System.Collections;

public class MoveTestScript : MonoBehaviour {

	public float moveX;
	public float moveY;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float finalMoveX = 0;
		float finalMoveY = 0;

		if (Input.GetKey (KeyCode.D)) {
			finalMoveX += moveX;
		}

		if (Input.GetKey (KeyCode.Q)) {
			finalMoveX += -moveX;
		}

		if (Input.GetKey (KeyCode.Z)) {
			finalMoveY += moveY;
		}

		if (Input.GetKey (KeyCode.S)) {
			finalMoveY += -moveY;
		}

		if (finalMoveX != 0 || finalMoveY != 0) {
			Move (finalMoveX, finalMoveY);
		}
	}

	void Move(float xMove,float yMove){
		this.gameObject.transform.position = new Vector3 (gameObject.transform.position.x + xMove,gameObject.transform.position.y+yMove,gameObject.transform.position.z);
	}
}
