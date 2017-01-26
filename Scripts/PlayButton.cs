using UnityEngine;
using System.Collections;

public class PlayButton : MonoBehaviour {
	// Use this for initialization
	public void Play(int level) 
	{
		Application.LoadLevel (level);

	
	}

}
