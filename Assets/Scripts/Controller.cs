//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using UnityEngine;

public class Controller
{
    public static int playerWealth;
    public int warriorCost;
    public int archerCost;
    public int workerCost;
    public float coolDownTime;						//cool-down time set in inspect, set to 2 seconds
    public float currentTime;
    public bool spawnAvailable;

	public Controller (){
		coolDownTime = 2.00f;
        workerCost = 20;
        warriorCost = 50;
        archerCost = 60;
        playerWealth = 0;
        spawnAvailable = true;
	}

	// Spawning a unit
	public void SpawnUnit(String type){
		if (CanSpawnUnit ("Warrior") && type == "Warrior")
			playerWealth = playerWealth - warriorCost;
		else if (CanSpawnUnit ("Worker") && type == "Worker")
			playerWealth = playerWealth - workerCost;
		else if (CanSpawnUnit ("Archer") && type == "Archer")
			playerWealth = playerWealth - archerCost;

		currentTime = coolDownTime;
		spawnAvailable = false;
		}

	// When a button cannot be pressed, this method is called. It will reduce the cooldown timer everytime it's called, or it will allow you to spawna a unit
	public void NonPressableButton(){
		if (currentTime <= 0) {
			spawnAvailable = true;
		} else {
			currentTime -= Time.deltaTime;
		}
	}

	// Returns true or false depending on whether you can spawn a unit or not
	public bool CanSpawnUnit(String type){
		if (type == "Warrior")
			return (spawnAvailable && playerWealth >= warriorCost);
		else if (type == "Archer")
			return (spawnAvailable && playerWealth >= archerCost);
		else if (type == "Worker")
			return (spawnAvailable && playerWealth >= workerCost);
		else
			return false;
	}

	// Returns true if player doesn't have enough money to spawn a certain unit, false if the player does have enough money
	public bool NoMoneyUnit(String type){
		if (type == "Warrior")
			return (playerWealth < warriorCost);
		else if (type == "Archer")
			return (playerWealth < archerCost);
		else if (type == "Worker")
			return (playerWealth < workerCost);
		else
			return true;

	}

	// Generates gold every second
    public void GoldGen()
    {
        playerWealth += 2;
    }
}


