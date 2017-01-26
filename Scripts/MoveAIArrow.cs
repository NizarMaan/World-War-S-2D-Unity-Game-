using UnityEngine;
using System.Collections;

public class MoveAIArrow : MonoBehaviour {
	//class that generates the Arrow shot by the AI archer, specifies its movement speed and direction
	private float moveSpeed;
	private GameObject thisArrow;
	// Use this for initialization
	void Start () {
		moveSpeed = 0.02f;
		thisArrow = gameObject; 
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector2 (transform.position.x - moveSpeed, transform.position.y);
	}

	//detect arrow collisions with the following units, cant check for collisions in respectice class files (warrior, archer)
	//since those already use an OnCollisionSTAY method
	void OnCollisionEnter2D(Collision2D col){
		//set arrow to isKinematic so that physics engine doesn't knock units over on impact
		//destory the arrow on impact
		if (col.transform.tag == "warrior") {
			Rigidbody2D arrowBody = thisArrow.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			PlayerWarriorController warriorObject = col.gameObject.GetComponent <PlayerWarriorController>();
			warriorObject.health = warriorObject.health - AIarcherController.attackDamage;
			Destroy (thisArrow);
		}
		if (col.transform.tag == "Archer") {
			Rigidbody2D arrowBody = thisArrow.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			PlayerArcherController archerObject = col.gameObject.GetComponent <PlayerArcherController>();
			archerObject.health = archerObject.health - AIarcherController.attackDamage;
			Destroy (thisArrow);
		}
		else{
			Destroy (thisArrow);
		}
		
	}
}
