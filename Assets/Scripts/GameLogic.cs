using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public GUIStyle timerBar;

    private float timer;

    private PlayerControls player;

    // GUI stuff
    private Rect powersPanel = new Rect(150.0f, Screen.height - 100.0f, 200.0f, 80.0f);
    private Rect rightPanel = new Rect(Screen.width - 150.0f, Screen.height - 100.0f, 130.0f, 40.0f);

    public string[] powersTitles = { "[Q] Speed", "[W] Rate", "[R] Ammo" };
    public const int powersCount = 3;

	void Start() {
        player = GameObject.Find("Player").GetComponent<PlayerControls>();
	}

	void Update() {
        if (timer >= 10.0f) {
            timer = 0.0f;
            player.LevelUp();
        } else timer += Time.deltaTime;
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
            GUILayout.Label("Time: " + timer);
        GUILayout.EndArea();
        GUI.Box(new Rect(Screen.width - 150.0f, Screen.height - 50.0f, 130.0f * (timer / 10.0f ), 20.0f), "", timerBar);
    }
}
