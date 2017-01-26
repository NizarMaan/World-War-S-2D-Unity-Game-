using UnityEngine;
using System.Collections;

public class AIworkerController : MonoBehaviour {
	private float moveSpeed;
	private Animator animator;
	private GameObject thisWorker;
	private int goldEarned;
	private int hitCount;

	public Transform showGoldEarned;
	public float health;
	public AudioClip pickaxeImpactFX;
	
	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		health = 5;
		goldEarned = 5;
		thisWorker = gameObject;
		moveSpeed = 0.00125f;
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0)
			Die ();

		if (animator.GetBool ("canWork") == false) {
			transform.position = new Vector2 (transform.position.x - moveSpeed, transform.position.y);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col){
		if (col.transform.tag == "Mine" && animator.GetBool ("canWork") == false) {
			animator.SetBool ("canWork", true);
			StartCoroutine (MineGold ());
		}
		if(col.transform.tag == "playerArrow"){
			MovePlayerArrow arrowObject = col.gameObject.GetComponent <MovePlayerArrow>();
			Rigidbody2D arrowBody = arrowObject.GetComponent<Rigidbody2D>();
			arrowBody.isKinematic = true;
			health = health - PlayerArcherController.attackDamage;
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
			GetComponent<AudioSource>().PlayOneShot (pickaxeImpactFX, 0.2f);						//play pickaxe sound effect at low volume
			AIbehaviour.AIwealth = AIbehaviour.AIwealth + goldEarned;
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

	//kill the worker
	void Die(){
		animator.SetBool ("canWork", false);
		Controller.playerWealth =(int) (Controller.playerWealth + goldEarned);
		DisplayGoldEarned ();								//display to the player the gold earned from killing this unit
		AIbehaviour.workerCount = AIbehaviour.workerCount-1;
		transform.Translate (Vector2.up*10);
		Destroy (thisWorker);
	}
}