using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {

    private GameLogic game;

	void Start() {
        game = Camera.main.GetComponent<GameLogic>();
	}
	
    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Wall") {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Enemy") {
            game.IncreaseScore();
            Destroy(collision.collider.gameObject);
            Destroy(gameObject);
            game.enemySound.Play();
        }
    }
}
