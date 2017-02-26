using UnityEngine;
using System.Collections;

public class PlayerArcherController : MonoBehaviour {
	//set up attributes for Archer
	private float moveSpeed;
	private Animator animator;
	private bool targetFound;
	private float maxRange;
	private GameObject target;          //Target - reference gameobject that was found
	private GameObject thisArcher;
	private float distance;
	private float attackSpeed;
	private Animator colAnimator;
	private bool firstHit;

	public AudioClip bowReleaseSFX;
	public float health;
	public static float attackDamage;
	public GameObject _arrow;

	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		health = 7;
		firstHit = false;
		attackDamage = 4.0f;
		attackSpeed = 3.0f;
		thisArcher = gameObject; 
		maxRange = 0.4f;			//any distance from the target above this value and the archer won't attack
		targetFound = false;
		moveSpeed = 0.0025f;
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0){
			Die ();
		}
		//look for a target and move every frame if not already attacking something
		if (animator.GetBool("inRange") == false){
			GetTarget ();
			transform.position = new Vector2 (transform.position.x + moveSpeed, transform.position.y);
		}
	}

	//collision detection, set animation set according to what has been collided with (ie. Idle or otherwise)
	void OnCollisionStay2D(Collision2D col){
		if (col.transform.tag == "warrior") {
			//sets any warrior that ends up standing behind the archer to Idle animation
			colAnimator = col.transform.GetComponent<Animator> ();
			Vector2 relativePosition = transform.InverseTransformPoint (col.transform.position);
			if(relativePosition.x < 0){
				colAnimator.SetBool ("isIdle", true);
			}
		}
		if (col.transform.tag == "warrior" && animator.GetBool("inRange") == false) {
			animator.SetBool ("isIdle", true);
		}
		if (col.transform.tag == "archer" && animator.GetBool("inRange") == false) {
			animator.SetBool ("isIdle", true);
		}
	}

	//check what objects have come out of contact with the archer
	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "warrior" && targetFound == false) {
			animator.SetBool ("inRange", false);
		}
		if (col.transform.tag == "archer" && targetFound == false) {
			animator.SetBool ("inRange", false);
		}

	}

	//determine whether a target is in range to start attacking
	void GetTarget(){
		target = GameObject.FindGameObjectWithTag ("AIwarrior");
		if (target == null) {
			target =  GameObject.FindGameObjectWithTag ("AIarcher");
		}
		if (target == null){
			target = GameObject.FindGameObjectWithTag ("AIworker");
		}
		if (target == null){
			target =  GameObject.FindGameObjectWithTag ("AIcastle");
		}
		if (target != null) {
			distance = Vector3.Distance (transform.position, target.transform.position);
			targetFound = distance <= maxRange;
		}
		if (targetFound == true){
			if(target.transform.tag == "AIwarrior"){
				AIwarriorController AIwarriorObject = target.GetComponent<AIwarriorController>();
				StartCoroutine(Shoot(AIwarriorObject));
			}
			if(target.transform.tag == "AIworker"){
				AIworkerController AIworkerObject = target.GetComponent<AIworkerController>();
				StartCoroutine(Shoot(AIworkerObject));
			}
			if(target.transform.tag == "AIarcher"){
				AIarcherController AIarcherObject = target.GetComponent<AIarcherController>();
				StartCoroutine(Shoot(AIarcherObject));
			}
			if(target.transform.tag == "AIcastle"){
				AIHealthBar AIcastleObject = target.GetComponent<AIHealthBar>();
				StartCoroutine(Shoot(AIcastleObject));
			}
		}
	}
/*------------------Overloaded Shoot methods for each different attackable unit-----------------------------*/
	IEnumerator Shoot(AIwarriorController enemyWarrior){
		animator.SetBool ("inRange", true);
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			
		/*if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1f);
			yield return new WaitForSeconds (attackSpeed);
		}*/
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			Die ();
		}

		else if (enemyWarrior.health > 0 && animator.GetBool ("inRange") == true) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			Instantiate (_arrow, thisArcher.transform.position, Quaternion.identity);
			yield return new WaitForSeconds (attackSpeed);
			StartCoroutine (Shoot (enemyWarrior));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			animator.SetBool ("inRange", false);
		}
	}

	IEnumerator Shoot(AIworkerController enemyWorker){
		animator.SetBool ("inRange", true);
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
		
		/*if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			yield return new WaitForSeconds (attackSpeed);
		}*/
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			Die ();
		}
		
		//deduce health from the enemy
		else if (enemyWorker.health > 0 && animator.GetBool ("inRange") == true) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			Instantiate (_arrow, thisArcher.transform.position, Quaternion.identity);
			yield return new WaitForSeconds (attackSpeed);
			StartCoroutine (Shoot (enemyWorker));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			animator.SetBool ("inRange", false);
		}
	}

	IEnumerator Shoot(AIarcherController enemyArcher){
		animator.SetBool ("inRange", true);
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			
		/*if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			yield return new WaitForSeconds (attackSpeed);
		}*/
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			Die ();
		}
		
		//deduce health from the enemy
		else if (enemyArcher.health > 0 && animator.GetBool ("inRange") == true) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			Instantiate (_arrow, thisArcher.transform.position, Quaternion.identity);
			yield return new WaitForSeconds (attackSpeed);
			StartCoroutine (Shoot (enemyArcher));
		}
		//else the enemy has been killed thus, reset the warrior
		else{
			firstHit = false;
			animator.SetBool ("inRange", false);
		}
	}

	IEnumerator Shoot(AIHealthBar enemyCastle){
		animator.SetBool ("inRange", true);
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
		
		/*if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);	
			yield return new WaitForSeconds (attackSpeed);
		}*/
		firstHit = true;
		
		if (health <= 0) {
			Die ();
		}	
		
		if (enemyCastle.currentHealth > 0 && animator.GetBool("inRange") == true) {
			GetComponent<AudioSource>().PlayOneShot (bowReleaseSFX, 1);
			Instantiate (_arrow, thisArcher.transform.position, Quaternion.identity);
			yield return new WaitForSeconds (attackSpeed);
			StartCoroutine (Shoot (enemyCastle));
		}
	}

	//method to kill the archer
	void Die(){
		animator.SetBool("inRange", false);
		transform.Translate (Vector2.up*10);
		Destroy (thisArcher);
	}
}
