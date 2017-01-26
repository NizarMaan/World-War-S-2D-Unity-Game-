using UnityEngine;
using System.Collections;

public class AIarcherController : MonoBehaviour {
	//set up attributes for AI archer
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
	public Transform showGoldEarned;
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
		if (animator.GetBool("inRange") == false){
			GetTarget ();
			transform.position = new Vector2 (transform.position.x - moveSpeed, transform.position.y);
		}
	}

	//collision detection automatically called every frame
	void OnCollisionEnter2D(Collision2D col){
		//with the Col parameter you can get a reference to the collided object
		if (col.transform.tag == "AIwarrior") {
			colAnimator = col.transform.GetComponent<Animator> ();
			Vector2 relativePosition = transform.InverseTransformPoint (col.transform.position);
			if(relativePosition.x < 0){
				colAnimator.SetBool ("isIdle", true);
			}
		}
		if(col.transform.tag == "playerArrow"){
			MovePlayerArrow arrowObject = col.gameObject.GetComponent <MovePlayerArrow>();
			Rigidbody2D arrowBody = arrowObject.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			health = health - PlayerArcherController.attackDamage;
		}
		if (col.transform.tag == "AIwarrior" && animator.GetBool("inRange") == false) {
			animator.SetBool ("isIdle", true);
		}
		if (col.transform.tag == "AIarcher" && animator.GetBool("inRange") == false) {
			animator.SetBool ("isIdle", true);
		}
	}

	//called once per frame automatically, detects when collision has stopped with an object
	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "AIwarrior" && targetFound == false) {
			animator.SetBool ("inRange", false);
		}
		if (col.transform.tag == "AIarcher" && targetFound == false) {
			animator.SetBool ("inRange", false);
		}
		
	}

	//findgameobject with an equal tag, if no such object currently exists, look for the next unit in range
	void GetTarget(){
		target = GameObject.FindGameObjectWithTag ("warrior");
		if (target == null) {
			target =  GameObject.FindGameObjectWithTag ("Archer");
		}
		if (target == null){
			target = GameObject.FindGameObjectWithTag ("Worker");
		}
		if (target == null){
			target =  GameObject.FindGameObjectWithTag ("PlayerCastle");
		}
		//check to see if the found enemy unit is in range, otherwise method falls through
		if (target != null) {
			distance = Vector3.Distance (transform.position, target.transform.position);
			targetFound = distance <= maxRange;
		}
		//if valid target found then call appropriate Shoot method with target object reference
		if (targetFound == true){
			if(target.transform.tag == "warrior"){
				PlayerWarriorController warriorObject = target.GetComponent<PlayerWarriorController>();
				StartCoroutine(Shoot(warriorObject));
			}
			if(target.transform.tag == "Worker"){
				PlayerWorkerController workerObject = target.GetComponent<PlayerWorkerController>();
				StartCoroutine(Shoot(workerObject));
			}
			if(target.transform.tag == "Archer"){
				PlayerArcherController archerObject = target.GetComponent<PlayerArcherController>();
				StartCoroutine(Shoot(archerObject));
			}
			if(target.transform.tag == "PlayerCastle"){
				HealthBar castleObject = target.GetComponent<HealthBar>();
				StartCoroutine(Shoot(castleObject));
			}
		}
	}
/*-----------------------------------Overloaded Shoot methods with different parameters------------------------------------------------*/

	IEnumerator Shoot(PlayerWarriorController enemyWarrior){
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
	
	IEnumerator Shoot(PlayerWorkerController enemyWorker){
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
	
	IEnumerator Shoot(PlayerArcherController enemyArcher){
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
	
	IEnumerator Shoot(HealthBar enemyCastle){
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

	//method to terminate this object on death
	void Die(){
		animator.SetBool("inRange", false);
		Controller.playerWealth =(int) (Controller.playerWealth + (AIbehaviour.archerCost / 3));
		DisplayGoldEarned();
		AIbehaviour.archerCount = AIbehaviour.archerCount - 1;
		transform.Translate (Vector2.up*10);
		Destroy (thisArcher);
	}
	
	//method to display the gold earned by player right above the warrior's coordinates
	//gold earned is the warrior's cost divided by 2.5
	void DisplayGoldEarned(){
		Vector3 v = Camera.main.WorldToViewportPoint(thisArcher.transform.position);
		Transform goldEarnedText = (Transform) Instantiate(showGoldEarned, new Vector3(v.x, v.y), Quaternion.identity);
		goldEarnedText.GetComponent<GUIText>().text = "+" + (AIbehaviour.archerCost / 3).ToString ();
	}
}
