using UnityEngine;
using System.Collections;

public class SoundPadCollideHandler : MonoBehaviour {

    private SoundFieldManager thinAir;
    
    void OnCollisionEnter(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        thinAir.initSound(pos);
    }

	// Use this for initialization
	void Start () {
		GameObject go = GameObject.Find ("SoundManagerHolder");
		thinAir = go.GetComponent <SoundFieldManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
