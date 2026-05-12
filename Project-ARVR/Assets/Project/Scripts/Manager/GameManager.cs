using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Image Target")]
    public GameObject imageTarget; // Reference to the AR image target in the scene

    [Header("Player and Enemies Config")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public GameObject[] enemiesPrefab;
    public Transform[] enemiesSpawnPoint;
    public int numberOfEnemies = 3;

    [Header("Game Status")]
    private bool _isGameOver = false;
    public int score = 0;
    public int enemyCount = 0;
    public GameObject gameOverContainer;
    public TMP_Text enemyLeftContainer;

    [Header("UI")]
    public GameObject tapToPlay;
    public GameObject foundTarget;
    public GameObject lostTarget;
    public GameObject hud;

    public GameObject PlayerInstance;
    public List<GameObject> enemies = new List<GameObject>();

    private const string enemyLeft = "Enemy Left: ";

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        // Ensure the game over UI is hidden at the start
        hud.SetActive(false);
        foundTarget.SetActive(true);
        lostTarget.SetActive(false);
        tapToPlay.SetActive(false);
    }

    //private IEnumerator Start() {
    //    // GPS Setup - Check if GPS is enabled and start it
    //    if (!Input.location.isEnabledByUser) {
    //        Debug.Log("GPS is not enabled on this device.");
    //        yield break;
    //    }
    //    Input.location.Start();
    //    int maxWait = 20;
    //    while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
    //        yield return new WaitForSeconds(1);
    //        maxWait--;
    //    }
    //    if(maxWait <= 0) {
    //        Debug.Log("GPS initialization timed out.");
    //        yield break;
    //    }
    //    if (Input.location.status == LocationServiceStatus.Failed) {
    //        Debug.Log("Unable to determine device location.");
    //        yield break;
    //    } else {
    //        Debug.Log("GPS initialized successfully. Latitude: " + Input.location.lastData.latitude + 
    //            ", Longitude: " + Input.location.lastData.longitude);
    //    }
    //}

    // Testing spawn player and enemies without AR target detection
    //private void Start()
    //{
    //    gameOverContainer.SetActive(false); // Ensure game over UI is hidden at the start
    //    enemyLeftContainer.text = enemyLeft + enemyCount; // Initialize enemy left text
    //    SpawnPlayer(); // Spawn player immediately for testing
    //    InvokeRepeating(nameof(SpawnEnemies), 3f, 5f); // Start spawning enemies every 5 seconds after a 3-second delay
    //}

    // Called when the AR target is found
    public void OnTargetFound() { 
        foundTarget.SetActive(false);
        lostTarget.SetActive(false);
        tapToPlay.SetActive(true);
        StartCoroutine(WaitingTapToPlay()); // Wait for player to tap before starting the game
        Debug.Log("Target found, spawning player and enemies.");
        gameOverContainer.SetActive(false);
        enemyLeftContainer.text = enemyLeft + enemyCount; // Initialize enemy left text
    }

    // Called when the AR target is lost
    public void OnTargetLost() { 
        lostTarget.SetActive(true);
        tapToPlay.SetActive(false);
        foundTarget.SetActive(false);
        hud.SetActive(false);
        Debug.Log("Target lost, cleaning up player and enemies.");
        CancelInvoke(nameof(SpawnEnemies)); // Cancel any pending enemy spawns
        ClearPlayerAndEnemies(); // Clear player and enemies from the scene
    }

    IEnumerator WaitingTapToPlay() { 
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0)); // Wait until the player taps the screen
        tapToPlay.SetActive(false);
        hud.SetActive(true); // Show the HUD after tapping to play
        SpawnPlayer(); // Spawn the player when the target is found
        InvokeRepeating(nameof(SpawnEnemies), 3f, 5f); // Start spawning enemies every 5 seconds after a 3-second delay
    }

    private void SpawnPlayer() {
        if(PlayerInstance != null) return; // Avoid spawning multiple players if the target is found again before losing it
        PlayerInstance = Instantiate(playerPrefab, 
            playerSpawnPoint.position, playerSpawnPoint.rotation);
        PlayerInstance.transform.SetParent(imageTarget.transform); // Parent the player to the image target for AR tracking
    }

    private void SpawnEnemies() {
        RemoveEnemies(); // Clean up any destroyed enemies before spawning new ones
        if(enemies.Count >= numberOfEnemies) return;

        int randomIndex = Random.Range(0, enemiesSpawnPoint.Length);
        int randomEnemyIndex = Random.Range(0, enemiesPrefab.Length);

        GameObject enemy = Instantiate(enemiesPrefab[randomEnemyIndex],
            enemiesSpawnPoint[randomIndex].position, enemiesSpawnPoint[randomIndex].rotation);
        enemy.transform.SetParent(imageTarget.transform); // Parent the enemy to the image target for AR tracking
        enemies.Add(enemy); // Add the new enemy to the list
        AdjustEnemyLeft(1); // Increment the enemy left count
        Debug.Log("Spawned enemy: " + enemy.name + " at position: " + enemy.transform.position);
    }

    public void RemoveEnemies() 
        => enemies.RemoveAll(enemy => enemy == null); // Remove destroyed enemies from the list

    public void GameOver() { 
        if(_isGameOver) return; // Prevent multiple game over triggers
        Handheld.Vibrate(); // Vibrate the device to indicate game over
        _isGameOver = true;
        CancelInvoke(nameof(SpawnEnemies)); // Stop spawning new enemies
        ClearPlayerAndEnemies(); // Clear player and enemies from the scene
        gameOverContainer.SetActive(true);
    }

    public void RestartGame() { 
        gameOverContainer.SetActive(false);
        score = 0; // Reset score
        _isGameOver = false;
        SpawnPlayer(); // Respawn player
        InvokeRepeating(nameof(SpawnEnemies), 3f, 5f); // Restart spawning enemies
    }

    // Method to adjust the score, can be called from other scripts when player scores points
    public void AdjustScore(int scoreAmount) { 
        score += scoreAmount;
        Debug.Log("Score adjusted: " + score);
    }

    public void AdjustEnemyLeft(int amount) { 
        Debug.Log(enemyLeft + amount);
        enemyCount += amount;
        enemyCount = Mathf.Max(enemyCount, 0); // Ensure enemy count doesn't go negative
        enemyLeftContainer.text = enemyLeft + enemyCount;
    }

    public void ClearPlayerAndEnemies() { 
        // Clear player
        if (PlayerInstance != null) { 
            Destroy(PlayerInstance);
        }
        // Clear enemies
        foreach (GameObject enemy in enemies) { 
            if (enemy != null) {
                Destroy(enemy);
            }
        }
        enemies.Clear(); // Clear the list of enemies
    }


    // Hàm để gán vào nút Jump trên màn hình
    public void OnJumpButtonPressed()
    {
        if (PlayerInstance != null)
        {
            // Lấy script PlayerController (Movement) từ nhân vật đang đứng trong AR
            PlayerMovement pc = PlayerInstance.GetComponent<PlayerMovement>();
            if (pc != null) pc.Jump();
        }
    }

    // Hàm để gán vào nút Attack trên màn hình
    public void OnAttackButtonPressed()
    {
        if (PlayerInstance != null)
        {
            PlayerAttack pa = PlayerInstance.GetComponent<PlayerAttack>();

            if (pa != null)
            {
                pa.Attack(); // Gọi hàm Attack ở script riêng 
            }
            else
            {
                Debug.LogWarning("Nhân vật chưa được gắn script PlayerAttack rồi!");
            }
        }
    }
}
