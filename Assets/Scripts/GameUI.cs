using UnityEngine;
using UnityEngine.UI;

//all sound effects from
//http://www.pond5.com/


public class GameUI : MonoBehaviour {
	//public attributes, set via the Unity Inspector
	public Text wealthTotal;
	public GUIContent warriorIcon;
	public GUIContent archerIcon;
	public GUIContent workerIcon;
	public GameObject _spawnPoint;
	public GameObject _unitWorker;
	public GameObject _unitWarrior; 
	public GameObject _unitArcher;
	public Controller control;

	//Initializes values. Start is only called once, automatically, at the begining of an object's "life"
	void Start(){
		setGoldText ();									//set the gold text on screen
		InvokeRepeating("GoldGen", 0, 1.0f);			//invoke GoldGen every 1 second
		control = new Controller ();
	}

	void OnGUI(){
		//GUI component sizes and coordinates determined by ratios relative to screen size
/*-------------------Warrior's Button-----------------------------------------------------------------------------------------------------------------------------*/
		//if cooldown is done and enough gold, then create a pressable button
		if (control.CanSpawnUnit("Warrior")){
			if (GUI.Button (new Rect (Screen.width / 2.5f, Screen.height * .918f, Screen.width / 7, Screen.height / 14), warriorIcon)) {
				control.SpawnUnit ("Warrior");
				Instantiate (_unitWarrior, _spawnPoint.transform.position, Quaternion.identity);
			}
		}
		//else create a non-pressable button with appropriate messages
		else if (control.spawnAvailable == false && Controller.playerWealth >= control.warriorCost){
			GUI.Box (new Rect (Screen.width/2.5f, Screen.height*.918f,Screen.width/7, Screen.height/14), "Cooldown");
			control.NonPressableButton();
		}
		//else create a non-pressable button with appropriate messages
		else if (control.NoMoneyUnit("Warrior")){
			GUI.Box (new Rect (Screen.width/2.5f, Screen.height*.918f, Screen.width/7, Screen.height/14), "Not enough" + "\n" + "gold!");
			control.NonPressableButton();
		}

/*-------------------Archer's Button-----------------------------------------------------------------------------------------------------------------------------*/
		if (control.CanSpawnUnit("Archer")) {
			if (GUI.Button (new Rect (Screen.width / 1.3f, Screen.height * .918f, Screen.width / 7, Screen.height / 14), archerIcon)) {
				control.SpawnUnit ("Archer");
				Instantiate (_unitArcher, _spawnPoint.transform.position, Quaternion.identity);
			}
		}

		else if (control.spawnAvailable == false && Controller.playerWealth >= control.archerCost){
			GUI.Box (new Rect (Screen.width/1.3f, Screen.height*.918f, Screen.width/7, Screen.height/14), "Cooldown");
			control.NonPressableButton();
		}

		else if (control.NoMoneyUnit("Archer")){
			GUI.Box (new Rect (Screen.width/1.3f, Screen.height*.918f, Screen.width/7, Screen.height/14), "Not enough" + "\n" + "gold!");
			control.NonPressableButton();
		}

/*-------------------Worker's Button-----------------------------------------------------------------------------------------------------------------------------*/
		if (control.CanSpawnUnit("Worker"))
		{
			if (GUI.Button (new Rect (Screen.width / 25, Screen.height * .918f, Screen.width / 7, Screen.height / 14), workerIcon)) {
				control.SpawnUnit ("Worker");
				Instantiate (_unitWorker, _spawnPoint.transform.position, Quaternion.identity);
			}
		}

		else if (control.spawnAvailable == false && Controller.playerWealth >= control.workerCost){
			GUI.Box (new Rect (Screen.width/25, Screen.height*.918f,Screen.width/7, Screen.height/14), "Cooldown");
			control.NonPressableButton();
		}
		else if (control.NoMoneyUnit("Worker")){
			GUI.Box (new Rect (Screen.width/25, Screen.height*.918f, Screen.width/7, Screen.height/14), "Not enough" + "\n" + "gold!");
			control.NonPressableButton();
		}
	}

	//update the player's wealth on screen
	void setGoldText(){
		wealthTotal.text = "Gold: " + Controller.playerWealth.ToString();
	}
	//generate 5 gold per frame/second 
	private void GoldGen(){
        control.GoldGen();
		setGoldText ();
	}
}
