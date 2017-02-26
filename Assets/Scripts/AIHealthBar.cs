using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

//all sound effects from
//http://www.pond5.com/

//HealthBar Script for the ENEMY's health bar
public class AIHealthBar : MonoBehaviour {
	//public attributes to be set in the Unity inspector
	public RectTransform healthTransform;
	public Text healthText;
	public Image visualHealth;
	public float currentHealth;
	//private attributes
	private float maxHealth = 50;
	private float storedY;
	private float minXval;
	private float maxXval;
	
	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		//initialize attributes
		currentHealth = maxHealth;
		healthText.text = "Enemy: " + currentHealth.ToString ();					//display current health to screen
		storedY = healthTransform.position.y;
		minXval = healthTransform.position.x + healthTransform.rect.width; 			//initialize the health bar's Y coordinate, minium X cord (zero health)
		maxXval = healthTransform.position.x;										//and maximum X cord (i.e. full health)
	}

	//collision detection automatically called every frame
	void OnCollisionEnter2D(Collision2D col){
		//if an arrow has collided with the tower then reduce the health
		if(col.transform.tag == "playerArrow"){
			//get reference to arrow and set it to Kinematic so the Physics engine
			//does not cause the tower to flip over
			MovePlayerArrow arrowObject = col.gameObject.GetComponent <MovePlayerArrow>();
			Rigidbody2D arrowBody = arrowObject.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			currentHealth = currentHealth - PlayerArcherController.attackDamage;
			HandleHealth ();
		}
	}

	//uses the MapValues function to change the health bar's color, and to 'reudce' it's width (it's actually just sliding the bar down)
	public void HandleHealth(){
		currentHealth = (int)currentHealth;
		healthText.text = "Enemy: " + currentHealth.ToString ();
		float currentXpos = MapValues (currentHealth, 0, maxHealth, minXval, maxXval);
		healthTransform.position = new Vector3 (currentXpos, storedY);
		
		if (currentHealth > (maxHealth / 2)) {
			visualHealth.color = new Color32( (byte) MapValues(currentHealth, maxHealth/2, maxHealth, 255, 0), 255, 0, 255);
		}
		else{
			visualHealth.color = new Color32( 255, (byte) MapValues(currentHealth, 0, maxHealth/2, 0, 255), 0, 255);
		}
		//open the YOU WIN menu when the AI's castle has reached 0 health
		if (currentHealth <= 0) {
			Application.LoadLevel(2);
		}

	}
	
	//mapping formula taken from the following tutorial video: https://www.youtube.com/watch?v=NgftVg3idB4
	//used to calculate the health bar's offset and its color
	private float MapValues(float curHealth, float healthMin, float healthMax, float minXpos, float maxXpos){
		return (curHealth - healthMin) * (maxXpos - minXpos) / (healthMax - healthMin) + minXpos; 
	}
}
