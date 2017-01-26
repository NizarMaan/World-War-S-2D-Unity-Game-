using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

//all sound effects from
//http://www.pond5.com/

//SEE AIHealthBar for similar documentation
public class HealthBar : MonoBehaviour {
	public RectTransform healthTransform;
	public Text healthText;
	public Image visualHealth;
	public float currentHealth;

	private float maxHealth = 50;
	private float storedY;
	private float minXval;
	private float maxXval;
	
	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		currentHealth = maxHealth;
		healthText.text = "Health: " + currentHealth.ToString ();
		storedY = healthTransform.position.y;
		minXval = healthTransform.position.x - healthTransform.rect.width; 
		maxXval = healthTransform.position.x;
	}
	void OnCollisionEnter2D(Collision2D col){
		if(col.transform.tag == "AIArrow"){
			MoveAIArrow arrowObject = col.gameObject.GetComponent <MoveAIArrow>();
			Rigidbody2D arrowBody = arrowObject.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			currentHealth = currentHealth - AIarcherController.attackDamage;
			HandleHealth ();
		}
	}

	public void HandleHealth(){
		currentHealth = (int)currentHealth;
		healthText.text = "Health: " + currentHealth.ToString ();
		float currentXpos = MapValues (currentHealth, 0, maxHealth, minXval, maxXval);
		healthTransform.position = new Vector3 (currentXpos, storedY);

		if (currentHealth > (maxHealth / 2)) {
			visualHealth.color = new Color32( (byte) MapValues(currentHealth, maxHealth/2, maxHealth, 255, 0), 255, 0, 255);
		}
		else{
			visualHealth.color = new Color32( 255, (byte) MapValues(currentHealth, 0, maxHealth/2, 0, 255), 0, 255);
		}
		//open the GAME OVER menu when the player's tower has reached zero health
		if (currentHealth <= 0){
			Application.LoadLevel(3);
		}
	}

	//mapping formula taken from the following tutorial video: https://www.youtube.com/watch?v=NgftVg3idB4
	private float MapValues(float curHealth, float healthMin, float healthMax, float minXpos, float maxXpos){
		return (curHealth - healthMin) * (maxXpos - minXpos) / (healthMax - healthMin) + minXpos; 
	}
}
