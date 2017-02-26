using UnityEngine;
using System.Collections;

//all sound effects from
//http://www.pond5.com/

//SEE AIwarriorController CLASS FOR DOCUMENTATION-- SIMILAR LOGIC APPLIES
public class PlayerWarriorController : MonoBehaviour {
	private float moveSpeed;
	private Animator animator;
	private Animator colAnimator;
	private GameObject thisWarrior;
	private float attackSpeed;
	private bool firstHit;
	private float attackDamage;

	public float health;
	public AudioClip deathFX;
	public AudioClip swordHitCastleFX;
	public AudioClip swordClashFX;

	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		firstHit = false;
		attackDamage = Random.Range (1.0f,2.0f);
		attackSpeed = 1.5f;
		thisWarrior = gameObject;
		health = 10.0f;
		moveSpeed = 0.0025f;
		animator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () {
		if (health <= 0){
			Die ();
		}
		if (animator.GetBool ("inRange") == false && animator.GetBool("isIdle") == false) {
			transform.position = new Vector2 (transform.position.x + moveSpeed, transform.position.y);
		}
	}

	void OnCollisionStay2D(Collision2D col){
		if (col.transform.tag == "warrior") {
			colAnimator = col.transform.GetComponent<Animator> ();
			Vector2 relativePosition = transform.InverseTransformPoint (col.transform.position);
			if(relativePosition.x < 0){
				colAnimator.SetBool ("isIdle", true);
			}
		}
		if (col.transform.tag == "Archer") {
			colAnimator = col.transform.GetComponent<Animator> ();
			Vector2 relativePosition = transform.InverseTransformPoint (col.transform.position);
			if(relativePosition.x < 0){
				colAnimator.SetBool ("isIdle", true);
			}
		}
		if (col.transform.tag == "AIwarrior" && animator.GetBool ("inRange") == false){
			AIwarriorController someObject = col.gameObject.GetComponent <AIwarriorController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (someObject));
		}
		if (col.transform.tag == "AIcastle" && animator.GetBool("inRange") == false) {
			AIHealthBar castleObject = col.gameObject.GetComponent <AIHealthBar>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (castleObject));		//get the reference of the enemy castle healthbar and pass it to the combat method
		}
		if (col.transform.tag == "AIworker" && animator.GetBool ("inRange") == false){
			AIworkerController workerObject = col.gameObject.GetComponent <AIworkerController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (workerObject));		//get the reference of the enemy warrior and pass it to the combat method		
		}
		if (col.transform.tag == "AIarcher" && animator.GetBool ("inRange") == false){
			AIarcherController someObject = col.gameObject.GetComponent <AIarcherController>();
			animator.SetBool("inRange", true);
			animator.SetBool ("isIdle", false);
			StartCoroutine (InCombat (someObject));
		}
	}

	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "warrior" && animator.GetBool ("isIdle") == true) {
			resetWarrior ();
		}
		if (col.transform.tag == "Archer" && animator.GetBool ("isIdle") == true) {
			resetWarrior ();
		}
	}

	void resetWarrior(){
		animator.SetBool ("inRange", false);
		animator.SetBool ("isIdle", false);
	}

	IEnumerator InCombat(AIwarriorController enemy){
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);

		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);		////play the on-hit sound effect at relatively low volume
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		if (health <= 0) {
			Die ();
		}
		else if (enemy.health > 0 && animator.GetBool ("inRange") == true) {
			enemy.health = enemy.health - attackDamage;
			StartCoroutine (InCombat (enemy));
		}
		else{
			firstHit = false;
			resetWarrior ();
		}

	}

	//combat loop vs castle, same procedure applied as above
	IEnumerator InCombat(AIHealthBar castle){			//overloaded method for case of attacking a Castle
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordHitCastleFX, 0.5f);
		
		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordHitCastleFX, 0.5f);		//play the on-hit sound effect at relatively low volume
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		
		if (health <= 0) {
			Die ();
		}	
		
		if (castle.currentHealth > 0 && animator.GetBool("inRange") == true) {
			castle.currentHealth = castle.currentHealth - attackDamage;
			castle.HandleHealth ();
			StartCoroutine (InCombat (castle));
		}
	}

	IEnumerator InCombat(AIworkerController enemy){
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);
		
		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			Die ();
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
	IEnumerator InCombat(AIarcherController enemy){
		//keep track of first hit so the unit starts attacking as soon as it collides instead of waiting and then begining to attack
		if(firstHit == false)
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);
		
		if (firstHit == true && health > 0) {
			GetComponent<AudioSource>().PlayOneShot (swordClashFX, 0.5f);
			yield return new WaitForSeconds (attackSpeed);
		}
		firstHit = true;
		
		//if this warrior dies during combat then call the DIE method
		if (health <= 0) {
			Die ();
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

	void Die(){
		resetWarrior ();
		transform.Translate (Vector2.up*10);
		Destroy (thisWarrior);
	}
}
