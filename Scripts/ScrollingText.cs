using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//all sound effects from
//http://www.pond5.com/

//this class generates the text object that shows the user any Gold that was gained
public class ScrollingText : MonoBehaviour {
	private Color myColor;
	private float textScrollSpeed;
	private float textFadeVal;
	private float scrollingDuration;
	
	public AudioClip earnedCoinsFX;

	// Use this for initialization; Initializes values. Start is only called once at the begining of an object's "life"
	void Start () {
		//get a reference to the text color, initialize attributes
		myColor = GetComponent<GUIText>().color;
		scrollingDuration = 2.5f;
		textFadeVal = 1;
		textScrollSpeed = 0.07f;
		//play the "gold coins" sound effect everytime this class is used
		GetComponent<AudioSource>().PlayOneShot (earnedCoinsFX, 5f);
	}
	
	// Update is called once per frame
	void Update(){
		//keep fading and moving the text
		if(textFadeVal > 0){
			transform.position = new Vector2 (transform.position.x, transform.position.y+(textScrollSpeed*Time.deltaTime));
			textFadeVal -= Time.deltaTime/scrollingDuration;
			myColor.a = textFadeVal;
		}
		//erase text when done fading
		else{
			Destroy (gameObject);
		}
	}
}
