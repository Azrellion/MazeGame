using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed;
	private int count;

	void Start()
	{
		count = 0;

	}

	void Update()
	{

	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVerticle = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVerticle);

		GetComponent<Rigidbody>().AddForce (movement * speed * Time.deltaTime);

        

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Finish")
		{
            Application.LoadLevel("Complete");
		}
			
	}
}
