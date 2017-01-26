using UnityEngine;
using System.Collections;

//SEE MoveAIarrow for similar documentation
public class MovePlayerArrow : MonoBehaviour {
	private float moveSpeed;
	private GameObject thisArrow;
	// Use this for initialization
	void Start () {
		moveSpeed = 0.02f;
		thisArrow = gameObject; 
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2 (transform.position.x + moveSpeed, transform.position.y);
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.transform.tag == "AIwarrior") {
			Rigidbody2D arrowBody = thisArrow.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			AIwarriorController warriorObject = col.gameObject.GetComponent <AIwarriorController>();
			warriorObject.health = warriorObject.health - PlayerArcherController.attackDamage;
			Destroy (thisArrow);
		}
		else{
			Destroy (thisArrow);
		}

	}
}
