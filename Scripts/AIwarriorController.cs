using UnityEngine;
using System.Collections;

//all sound effects from
//http://www.pond5.com/


public class AIwarriorController : MonoBehaviour {
	//private attributes
	private float moveSpeed;
	private Animator animator;
	private Animator colAnimator;
	private GameObject thisWarrior;
	private float attackSpeed;
	private bool firstHit;
	private float attackDamage;

	//public object references specified in the Unity inspector
	public Transform showGoldEarned;
	public float health;
	public AudioClip swordClashFX;
	public AudioClip swordHitCastleFX;
	public AudioClip deathFX;
	
	// Use this for initialization Start is only called once at the begining of an object's "life"
	void Start () {
		firstHit = false;
		attackDamage = Random.Range (1.0f, 2.0f);				//generate the warrior's damage value (random val between 1-2)
		attackSpeed = 1.5f;								
		thisWarrior = gameObject;								//reference to this particular warrior
		health = 10;
		moveSpeed = 0.0025f;									
		transform.localScale = new Vector2(transform.localScale.x*-1, transform.localScale.y) ;
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	//Warrior moves every game frame
	void Update () {
		if(health <=0)
		   AIDie ();

		if (animator.GetBool ("inRange") == false && animator.GetBool("isIdle") == false) {
			transform.position = new Vector2 (transform.position.x - moveSpeed, transform.position.y);
		}
	}

	//Detects collisions on this warrior's collision box and takes action
	//according to what it's colliding with
	void OnCollisionStay2D(Collision2D col){
		if (col.transform.tag == "AIwarrior") {
			colAnimator = col.transform.GetComponent<Animator> ();			//get reference to the warrior's animator that is colliding with this warrior
			Vector2 relativePosition = transform.InverseTransformPoint (col.transform.position);
			if(relativePosition.x < 0){					//check whether the AI warrior this warrior has collided with is infront (to the left) of itself
				colAnimator.SetBool ("isIdle", true);
			}
		}

		//if this warrior has collided with an enemy warrior then call the combat routine
		if (col.transform.tag == "warrior" && animator.GetBool ("inRange") == false){
			PlayerWarriorController warriorObject = col.gameObject.GetComponent <PlayerWarriorController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (warriorObject));		//get the reference of the enemy warrior and pass it to the combat method		
		}

		//if this warrior has collided with an enemy castle then call the castle combat routine
		if (col.transform.tag == "PlayerCastle" && animator.GetBool("inRange") == false) {
			HealthBar castleObject = col.gameObject.GetComponent <HealthBar>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (castleObject));		//get the reference of the enemy castle healthbar and pass it to the combat method
		}

		//if this warrior has collided with an enemy worker call the combat routine
		if (col.transform.tag == "Worker" && animator.GetBool ("inRange") == false){
			PlayerWorkerController workerObject = col.gameObject.GetComponent <PlayerWorkerController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (workerObject));		//get the reference of the enemy warrior and pass it to the combat method		
		}
		if (col.transform.tag == "Archer" && animator.GetBool ("inRange") == false){
			PlayerArcherController archerObject = col.gameObject.GetComponent <PlayerArcherController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (archerObject));		//get the reference of the enemy warrior and pass it to the combat method		
		}
	}
	//check to see if an AI warrior has lost contact with this object
	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "AIwarrior" && animator.GetBool ("isIdle") == true) {
			resetWarrior ();
		}
	}
	//call this to get the warrior to start walking again (i.e. reset it)
	void resetWarrior(){ 
		animator.SetBool ("inRange", false);
		animator.SetBool ("isIdle", false);
	}

	//combat loop vs enemy warrior 
	//IEnumerator must be returned in cases where WaitForSeconds method is being used
	IEnumerator InCombat(PlayerWarriorController enemy){
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);

		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;

		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			AIDie ();
		}

		//deduce health from the enemy
		else if (enemy.health > 0 && animator.GetBool ("inRange") == true) {
			enemy.health = enemy.health - attackDamage;
			StartCoroutine (InCombat (enemy));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			resetWarrior ();
		}
	}

	//combat loop vs castle, same procedure applied as above
	IEnumerator InCombat(HealthBar castle){			//overloaded method for case of attacking a Castle
		if(firstHit == false)
		GetComponent<AudioSource>().PlayOneShot (swordHitCastleFX, 0.1f);

		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordHitCastleFX, 0.1f);		//play the on-hit sound effect at relatively low volume
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;

		if (health <= 0) {
			AIDie ();
		}	

		if (castle.currentHealth > 0 && animator.GetBool("inRange") == true) {
			castle.currentHealth = castle.currentHealth - attackDamage;
			castle.HandleHealth ();
			StartCoroutine (InCombat (castle));
		}
	}


	IEnumerator InCombat(PlayerWorkerController enemy){
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);

		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			AIDie ();
		}
		
		//deduce health from the enemy
		else if (enemy.health > 0 && animator.GetBool ("inRange") == true) {
			enemy.health = enemy.health - attackDamage;
			StartCoroutine (InCombat (enemy));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			resetWarrior ();
		}
	}

	IEnumerator InCombat(PlayerArcherController enemy){
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);
		
		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.1f);
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			AIDie ();
		}
		
		//deduce health from the enemy
		else if (enemy.health > 0 && animator.GetBool ("inRange") == true) {
			enemy.health = enemy.health - attackDamage;
			StartCoroutine (InCombat (enemy));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			resetWarrior ();
		}
	}

	//display the wealth gained by player from dying AIwarrior
	void AIDie(){
		resetWarrior ();
		Controller.playerWealth =(int) (Controller.playerWealth + (AIbehaviour.warriorCost / 3.3f));
		DisplayGoldEarned();
		AIbehaviour.warriorCount = AIbehaviour.warriorCount - 1;
		transform.Translate (Vector2.up*10);					//move the warrior off-screen and then destroy it
		Destroy (thisWarrior);
	}

	//method to display the gold earned by player right above the warrior's coordinates
	//gold earned is the warrior's cost divided by 2.5
	void DisplayGoldEarned(){
		Vector3 v = Camera.main.WorldToViewportPoint(thisWarrior.transform.position);
		Transform goldEarnedText = (Transform) Instantiate(showGoldEarned, new Vector3(v.x, v.y), Quaternion.identity);
		goldEarnedText.GetComponent<GUIText>().text = "+" + ((int)(AIbehaviour.warriorCost / 3.3f)).ToString ();
	}
}
