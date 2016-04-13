using UnityEngine;
using System.Collections;

public class SphereScript : MonoBehaviour {

	public Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetToPos() {
		// Vector3 startPos = new Vector3(-1.23f, 4.23f, -0.314018f );

		transform.position = new Vector3(-1.23f, 4.23f, -0.314018f );
		rb.velocity = new Vector3(0, 0, 0);
	}

}
