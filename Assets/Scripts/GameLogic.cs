using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public GameObject enemyPrefab;

    // GUI Styles    
    public GUIStyle powerAdvanceStyle;
    public GUIStyle powerLevelStyle;
    public GUIStyle powerTextStyle;
    public GUIStyle timerBar;

    // Textures
    public Texture powerFrame;
    public Texture powerAdvance;
    public Texture timerTexture;

    // Sounds
    public AudioSource shootSound;
    public AudioSource levelupSound;
    public AudioSource advanceSound;
    public AudioSource enemySound;
    public AudioSource gameOverSound;

    // Score
    private int score;

    // LevelUp Time
    private float levelUpTime;
    private const float levelUpBaseTime = 10f;

    // EnemySpawn Time
    private float enemySpawnTime;
    private const float enemySpawnBaseTime = 3.0f;
    private const float enemySpawnStep = 0.025f;

    // Enemy speed
    private const float enemyBaseSpeed = 20.0f;
    private const float enemySpeedStep = 7.5f;

    // Enemies count in single wave
    private const int minEnemies = 1;
    private const int maxEnemies = 4;

    private PlayerControls player;
    private GameObject[] dummies;

    private const float distanceFromDummy = 30.0f;

    // GUI stuff
    private Rect levelUpLabel = new Rect(16.0f, Screen.height - 170.0f, 320.0f, 64.0f);
    private Rect powersPanel = new Rect(16.0f, Screen.height - 110.0f, 512.0f, 110.0f);
    // Power frames (relative to powersPanel)
    private Rect powerFramePanel = new Rect(0, 0, 96, 96);
    private Rect powerFrameAdvance = new Rect(36, 2, 25, 25);
    private Rect powerFrameLevel = new Rect(12, 24, 72, 48);
    private Rect powerFrameText = new Rect(12, 75, 72, 16);

    private Rect rightPanel = new Rect(Screen.width - 130.0f, Screen.height - 100.0f, 200.0f, 120.0f);
    private Rect levelLabel = new Rect(12, 0, 72, 25);

    private Rect introPanel = new Rect(Screen.width / 2.0f - 150.0f, Screen.height / 2.0f - 150.0f, 300.0f, 300.0f);
    private Rect gameOverPanel = new Rect(Screen.width / 2.0f - 100.0f, Screen.height / 2.0f - 100.0f, 200.0f, 200.0f);

    private string[] powersTitles = { "Bullet Speed", "Fire Rate", "Rotation Speed" };
    private string[] powersKeys = { "Q", "W", "E" };
    public const int powersCount = 3;                                       

    public bool paused { get; private set; }
    private bool intro;

    private const float guiScale = 0.75f;

	void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerControls>();
        dummies = GameObject.FindGameObjectsWithTag("Dummy");
        paused = intro = true;
	}

    private void Initialize() {
        paused = false;
        if (intro) {
            intro = false;
        }

        levelUpTime = 0.0f;
        enemySpawnTime = 0; // Timer off - spawn enemies at start
        score = 0;
       
        player.Initialize();
    }

    public void GameOver() {
        paused = true;

        // Reset player's rotation
        player.transform.rotation = Quaternion.identity;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy")) {
            Destroy(enemy);
        }

        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet")) {
            Destroy(bullet);
        }

        gameOverSound.Play();
    }

    void Update() {
        if (paused) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                Invoke("Initialize", 0.25f);
            }
            return;
        }

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
        Vector3 position = dummy.transform.position + 
            new Vector3(Random.Range(-distanceFromDummy, distanceFromDummy), 
            Random.Range(-distanceFromDummy, distanceFromDummy), 0);
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
        if (paused) {
            GUI.Box(introPanel, "");

            if (intro) {
                GUILayout.BeginArea(introPanel);
                GUILayout.Label("Hello! This game was made during Ludum Dare 27.", powerTextStyle);
                GUILayout.Label("The rules are simple:", powerTextStyle);
                GUILayout.Label(" - Rotate the player with arrow keys.");
                GUILayout.Label(" - Up/Down arrow will rotate the player by 180*.");
                GUILayout.Label(" - Shoot green enemies using Space.");
                GUILayout.Label(" - Every 10 seconds you will level up.");
                GUILayout.Label(" - Using Q, W and E improve your skills after leveling up.");
                GUILayout.Label(" - Play as long as you can - game is getting harder as the time goes by.");
                GUILayout.Label("");
                GUILayout.Label("Thanks for playing and have fun! ~m1_10sz");
                GUILayout.Label("");
                GUILayout.Label("Press [SPACE] to start...", powerTextStyle);
            } else {
                GUILayout.BeginArea(gameOverPanel);
                GUILayout.Label("GAME OVER", powerLevelStyle);
                GUILayout.Label("Score: " + score, powerLevelStyle);
                GUILayout.Label("Press [SPACE] to restart...", powerAdvanceStyle);
            }

            GUILayout.EndArea();
        } else {
            if (player.advancePoints > 0) {
                GUI.Label(levelUpLabel, "Level Up!", powerLevelStyle);
            }

            GUILayout.BeginArea(powersPanel);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < powersCount; i++) {
                Rect panel = powerFramePanel;
                panel.x += (panel.width + 10.0f) * i;

                GUILayout.BeginArea(panel, powerFrame);

                if (player.advancePoints > 0) {
                    GUILayout.BeginArea(powerFrameAdvance, powerAdvance);
                        GUILayout.Label(powersKeys[i], powerAdvanceStyle);
                    GUILayout.EndArea();
                }

                GUILayout.BeginArea(powerFrameLevel);
                    GUILayout.Label(""+player.powersLevels[i], powerLevelStyle);
                GUILayout.EndArea();

                GUILayout.BeginArea(powerFrameText);
                    GUILayout.Label(powersTitles[i], powerTextStyle);
                GUILayout.EndArea();

                GUILayout.EndArea();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(rightPanel);

            GUILayout.BeginArea(powerFramePanel, powerFrame);

            GUILayout.BeginArea(levelLabel);
                GUILayout.Label("Level", powerAdvanceStyle);
            GUILayout.EndArea();

            GUILayout.BeginArea(powerFrameLevel);
                GUILayout.Label("" + player.level, powerLevelStyle);
            GUILayout.EndArea();

            GUILayout.BeginArea(powerFrameText);
                GUILayout.Label("Score: " + score, powerTextStyle);
            GUILayout.EndArea();

            GUILayout.EndArea();

            GUILayout.EndArea();

            // Timer
            float percentDone = levelUpTime / levelUpBaseTime;
            float height = 48.0f;

            Rect rect = new Rect(Screen.width - 40.0f, Screen.height - 28.0f - percentDone * height, 
                30.0f, height * percentDone);

            GUI.DrawTexture(rect, timerTexture);

            rect.width = 28.0f;
            rect.height = 8.0f;
            rect.y -= 10.0f;
            GUI.Label(rect, levelUpTime.ToString("0.00"), powerTextStyle);
        }
    }

    public void IncreaseScore() {
        score++;
    }
}
