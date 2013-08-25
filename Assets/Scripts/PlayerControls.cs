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

    private float shootCooldown;
    private const float shootBaseCooldown = 0.8f;
    private const float shootCooldownStep = 0.025f;

    private const float rotationBaseSpeed = 2.0f;
    private const float rotationSpeedStep = 0.25f;

    private OTSprite sprite;
    private const float bulletVelocity = 300f;

	void Start() {
        sprite = GetComponent<OTSprite>();
        powersLevels = new int[GameLogic.powersCount];
        Initialize();   
	}

    void Initialize() {
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
    }

    void Advance(Powers power) {
        if (advancePoints > 0) {
            advancePoints--;
            powersLevels[(int)power]++;
        }
    }

	void Update() {
        // Shoot
        shootCooldown -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && shootCooldown <= 0.0f) {
            // Spawn bullet
            GameObject bullet = (GameObject) OT.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<OTSprite>().position = transform.position + transform.up * transform.localScale.x * 0.8f;
            
            // Apply force to the bullet
            bullet.rigidbody.AddForce(transform.up * bulletVelocity, ForceMode.Impulse);

            shootCooldown = shootBaseCooldown - powersLevels[(int)Powers.RATE] * shootCooldownStep;
            if (shootCooldown < shootCooldownStep) {
                shootCooldown = shootCooldownStep;
            }
        }

        // Rotate
        if (Input.GetKey(KeyCode.LeftArrow)) {
            Rotate(false);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            Rotate(true);
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
}
