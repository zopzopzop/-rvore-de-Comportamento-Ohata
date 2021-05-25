using Panda.Examples.Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour {
	
	void OnCollisionEnter(Collision col)
	{
		//encontra colizão com inimigo para dar-lhe dano
		if (col.collider.CompareTag("Enemy"))
		{
			col.collider.GetComponent<AI>().Damege();
			
		}
		Destroy(this.gameObject);
	}
}
