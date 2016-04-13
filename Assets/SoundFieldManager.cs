using UnityEngine;
using System.Collections;

public class SoundFieldManager : MonoBehaviour {

    public Vector3 soundEventPosition;
    public float  soundEventStartTime = 0;

	// Use this for initialization
	void Start () {
	
	}

	public void initSound (Vector3 soundPosition) {
		soundEventPosition = soundPosition;
		soundEventStartTime = Time.time;
		print("hello1" + soundPosition.x);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
