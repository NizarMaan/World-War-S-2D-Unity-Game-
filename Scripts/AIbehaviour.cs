using UnityEngine;
using System.Collections;

//all sound effects from
//http://www.pond5.com/


//this class generates the AI 5 gold per second
public class AIbehaviour : MonoBehaviour {
	//variables to keep track of cooldown
	private bool spawnAvailable;
	private float currentTime;
	private float coolDownTime;	

	//variables that keep track of how many of each AI have been spawned
	public static int archerCount;
	public static int warriorCount;	
	public static int workerCount;
	//set the wealth and unit costs to public to be able to calculate wealthGained by player and AI elsewhere
	public static int warriorCost;
	public static int workerCost;
	public static int archerCost;
	public static int AIwealth;

	//references to these objects are in the Unity inspector
	public GameObject _AIunitArcher;
	public GameObject _AIunitWorker;
	public GameObject _AIunitWarrior; 
	public GameObject _AIspawnPoint;

	//Start is only called once at the begining of an object's "life", initialize values
	void Start(){
		spawnAvailable = true;
		coolDownTime = 2.0f;
		AIwealth = 0;
		warriorCost = 50;
		workerCost = 20;
		archerCost = 60;
		InvokeRepeating ("GoldGen", 0, 1.0f);	//generate gold every second
	}
	// Update is called once per frame
	//creates up to three warriors, two archers and two workers if money is available and there is no cool-down
	//spawns a warrior, archer and worker unit again if their count is below 3, 2, and 2 respectively
	void Update () {
		if (AIwealth >= warriorCost && warriorCount <= 2 && spawnAvailable == true) {
			Instantiate (_AIunitWarrior, _AIspawnPoint.transform.position, Quaternion.identity);
			AIwealth = AIwealth - warriorCost;
			warriorCount++;
			currentTime = coolDownTime;
			spawnAvailable = false;
		}
		if (AIwealth >= archerCost && archerCount <= 2 && spawnAvailable == true) {
			Instantiate (_AIunitArcher, _AIspawnPoint.transform.position, Quaternion.identity);
			AIwealth = AIwealth - archerCost;
			archerCount++;
			currentTime = coolDownTime;
			spawnAvailable = false;
		}

		else if (AIwealth >= workerCost && spawnAvailable == true && workerCount < 2) {
			Instantiate (_AIunitWorker, _AIspawnPoint.transform.position, Quaternion.identity);
			AIwealth = AIwealth - workerCost;
			workerCount++;
			currentTime = coolDownTime;
			spawnAvailable = false;
		}
		//count down cooldown timer
		else if (spawnAvailable == false){
			if(currentTime <= 0){
				spawnAvailable = true;
			}
			else{
				currentTime -= Time.deltaTime;
			}
		}
	}

	//method invoked every second; generates 2gold/sec for the AI
	private void GoldGen(){
		AIwealth += 2;
	}
}
