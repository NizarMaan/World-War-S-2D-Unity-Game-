using UnityEngine;
using System.Collections;

public class PlayerWorkerController : MonoBehaviour {
	private float moveSpeed;
	private Animator animator;
	private GameObject thisWorker;
	private int goldEarned;
	private int hitCount;
	private bool cantWalk;

	public float health;
	public Transform showGoldEarned;
	public AudioClip pickaxeImpactFX;
	public AudioClip deathFX;
	
	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		cantWalk = false;
		health = 5;
		goldEarned = 5;
		thisWorker = gameObject;
		moveSpeed = 0.00125f;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0){
			Die ();
		}
		if (animator.GetBool ("canWork") == false && cantWalk == false) {
			transform.position = new Vector2 (transform.position.x + moveSpeed, transform.position.y);
		}
	}

	void OnCollisionEnter2D(Collision2D col){
		if (col.transform.tag == "Mine" && animator.GetBool ("canWork") == false) {
			animator.SetBool ("canWork", true);
			StartCoroutine (MineGold ());
		}
		if(col.transform.tag == "AIArrow"){
			MoveAIArrow arrowObject = col.gameObject.GetComponent <MoveAIArrow>();
			Rigidbody2D arrowBody = arrowObject.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			health = health - AIarcherController.attackDamage;
		}
		if (col.transform.tag == "AIarcher" || col.transform.tag == "AIwarrior"){
			cantWalk = true;
		}
	}

	void OnCollisionExit2D(Collision2D col){
		if (col.transform.tag == "AIarcher" || col.transform.tag == "AIwarrior"){	
			cantWalk = false;
		}
	}

	//gain gold every 3 pickaxe hits
	// ~1 hit per second
	IEnumerator MineGold(){
		if(hitCount < 3 && health > 0 && animator.GetBool ("canWork") == true){
			hitCount++;
			GetComponent<AudioSource>().PlayOneShot (pickaxeImpactFX, 0.2f);						//play pickaxe sound effect at low volume
			yield return new WaitForSeconds(1);
		}

		if (hitCount == 3 && health > 0 && animator.GetBool ("canWork") == true) {
			DisplayGoldEarned();
			GetComponent<AudioSource>().PlayOneShot (pickaxeImpactFX, 0.2f);						//play pickaxe sound effect at low volume
			Controller.playerWealth = Controller.playerWealth + goldEarned;
			hitCount = 0;
			yield return new WaitForSeconds (1);
		}

		if (health <= 0) {
			Die ();
		}

		else if (health > 0 && animator.GetBool ("canWork") == true) {
			StartCoroutine (MineGold ());
		}
	}

	//use this method to show the gold earned from killing this unit on screen
	//object to generate the text specified in Unity inspector
	void DisplayGoldEarned(){
		Vector3 v = Camera.main.WorldToViewportPoint(thisWorker.transform.position);
		Transform goldEarnedText = (Transform) Instantiate(showGoldEarned, new Vector3(v.x, v.y), Quaternion.identity);
		goldEarnedText.GetComponent<GUIText>().text = "+" + goldEarned.ToString ();
	}
	
	void Die(){
		animator.SetBool ("canWork", false);
		transform.Translate (Vector2.up*10);
		Destroy (thisWorker);
	}

	IEnumerator WaitAndPlay(){
		GetComponent<AudioSource>().PlayOneShot (deathFX, 2);
		yield return new WaitForSeconds(2);
	}
}
