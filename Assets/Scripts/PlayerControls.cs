using UnityEngine;
using System.Collections;

public enum Powers {
    SPEED = 0, 
    RATE, 
    ROTATION
}

public class PlayerControls : MonoBehaviour {

    public GameObject bulletPrefab;

    public int level { get; private set; }
    public int advancePoints { get; private set; }

    public int[] powersLevels { get; private set; }

    // Speed power
    private const float bulletBaseVelocity = 300f;
    private const float bulletVelocityStep = 5.0f;

    // Rate power
    private float shootCooldown;
    private const float shootBaseCooldown = 0.75f;
    private const float shootCooldownStep = 0.025f;

    // Rotation power
    private const float rotationBaseSpeed = 1.75f;
    private const float rotationSpeedStep = 0.25f;

    // Growing player
    private Vector3 baseScale;
    private const float scaleStep = 0.015f;

    private OTSprite sprite;
    private GameLogic game;

	void Start() {
        sprite = GetComponent<OTSprite>();
        game = Camera.main.GetComponent<GameLogic>();
        baseScale = sprite.transform.localScale;
        powersLevels = new int[GameLogic.powersCount];
	}

    public void Initialize() {
        level = 1;
        advancePoints = 0;

        for (int i = 0; i < GameLogic.powersCount; i++) {
            powersLevels[i] = 1;
        }

        shootCooldown = 0.0f;
    }

    public void LevelUp() {
        level++;
        advancePoints++;

        // Grow!
        transform.localScale += baseScale * scaleStep;

        game.levelupSound.Play();
    }

    void Advance(Powers power) {
        if (advancePoints > 0) {
            advancePoints--;
            powersLevels[(int)power]++;
            game.advanceSound.Play();
        }
    }

	void Update() {
        if (game.paused) {
            return;
        }

        // Shoot
        shootCooldown -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && shootCooldown <= 0.0f) {
            // Spawn bullet
            GameObject bullet = (GameObject) OT.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<OTSprite>().position = transform.position + transform.up * transform.localScale.x * 0.8f;
            
            // Apply force to the bullet
            bullet.rigidbody.AddForce(transform.up * (bulletBaseVelocity + bulletVelocityStep * powersLevels[(int)Powers.SPEED]), 
                ForceMode.Impulse);

            shootCooldown = shootBaseCooldown - powersLevels[(int)Powers.RATE] * shootCooldownStep;
            if (shootCooldown < shootCooldownStep) {
                shootCooldown = shootCooldownStep;
            }

            game.shootSound.Play();
        }

        // Rotate
        if (Input.GetKey(KeyCode.LeftArrow)) {
            Rotate(false);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            Rotate(true);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) {
            transform.Rotate(0, 0, 180.0f);
        }

        // Advance powers
        if (Input.GetKeyDown(KeyCode.Q)) {
            Advance(Powers.SPEED);
        }

        if (Input.GetKeyDown(KeyCode.W)) {
            Advance(Powers.RATE);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Advance(Powers.ROTATION);
        }
	}

    private void Rotate(bool negative) {
        int sign = negative ? -1 : 1;
        transform.Rotate(0, 0, sign * (rotationBaseSpeed + rotationSpeedStep * powersLevels[(int)Powers.ROTATION]));
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.tag == "Enemy") {
            game.GameOver();
        }
    }
}
