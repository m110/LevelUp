using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public GameObject enemyPrefab;

    // Sounds
    public AudioSource shootSound;
    public AudioSource levelupSound;
    public AudioSource advanceSound;
    public AudioSource enemySound;

    public GUIStyle timerBar;

    // LevelUp Time
    private float levelUpTime;
    private const float levelUpBaseTime = 10f;

    // EnemySpawn Time
    private float enemySpawnTime;
    private const float enemySpawnBaseTime = 3.0f;
    private const float enemySpawnStep = 0.025f;

    // Enemy speed
    private const float enemyBaseSpeed = 20.0f;
    private const float enemySpeedStep = 6.0f;

    // Enemies count in single wave
    private const int minEnemies = 1;
    private const int maxEnemies = 5;

    private PlayerControls player;
    private GameObject[] dummies;

    // GUI stuff
    private Rect powersPanel = new Rect(150.0f, Screen.height - 100.0f, 200.0f, 80.0f);
    private Rect rightPanel = new Rect(Screen.width - 150.0f, Screen.height - 100.0f, 130.0f, 40.0f);

    private string[] powersTitles = { "Speed", "Rate", "Rotation" };
    public const int powersCount = 3;

	void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerControls>();
        dummies = GameObject.FindGameObjectsWithTag("Dummy");
        Initialize();
	}

    void Initialize() {
        levelUpTime = 0.0f;
        enemySpawnTime = 0; // Timer off - spawn enemies at start
    }

    void Update() {
        if (levelUpTime >= levelUpBaseTime) {
            levelUpTime = 0.0f;
            player.LevelUp();
        } else levelUpTime += Time.deltaTime;

        if (enemySpawnTime <= 0.0f) {
            enemySpawnTime = enemySpawnBaseTime - (enemySpawnStep * player.level);
            for (int i = minEnemies; i <= Random.Range(minEnemies, maxEnemies); i++) {
                SpawnEnemy();
            }
        } else enemySpawnTime -= Time.deltaTime;
	}

    private void SpawnEnemy() {
        GameObject dummy = dummies[Random.Range(0, dummies.Length)];
        Vector3 position = dummy.transform.position + new Vector3(Random.Range(-40.0f, 40.0f), Random.Range(-40.0f, 40.0f), 0);
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity) as GameObject;

        enemy.GetComponent<OTSprite>().position = position;

        // Look at player
        Quaternion newRotation = Quaternion.LookRotation(position - player.transform.position, Vector3.forward);
        newRotation.x = 0.0f;
        newRotation.y = 0.0f;
        enemy.transform.rotation = newRotation;

        // Apply force
        enemy.rigidbody.AddForce(enemy.transform.up * (enemyBaseSpeed + enemySpeedStep * player.level), ForceMode.Impulse);
    }

    void OnGUI() {
        GUILayout.BeginArea(powersPanel);
            GUILayout.BeginHorizontal();

            if (player.advancePoints > 0) {
                for (int i = 0; i < powersCount; i++) {
                    GUILayout.Box(powersTitles[i] + " (" + player.powersLevels[i] + ")");
                }
            }

            GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUI.Box(rightPanel, "");
        GUILayout.BeginArea(rightPanel);
            GUILayout.Label("Level: " + player.level);
            GUILayout.Label("Time: " + levelUpTime);
        GUILayout.EndArea();
        GUI.Box(new Rect(Screen.width - 150.0f, Screen.height - 50.0f, 130.0f * (levelUpTime / 10.0f ), 20.0f), "", timerBar);
    }
}
