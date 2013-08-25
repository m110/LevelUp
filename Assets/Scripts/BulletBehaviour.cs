using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

	void Start() {
	
	}
	
	void Update() {
	
	}

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "wall"/* || collider.tag == "Player"*/) {
            Destroy(gameObject);
        }
    }
}
