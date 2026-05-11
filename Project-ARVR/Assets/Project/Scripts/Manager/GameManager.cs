using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [Header("Image Target")]
    public GameObject imageTarget; // Reference to the AR image target in the scene

    [Header("Player and Enemies Config")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public GameObject enemiesPrefab;
    public Transform[] enemiesSpawnPoint;
    public int numberOfEnemies = 3;

    [Header("Game Status")]
    private bool _isGameOver = false;
    public int score = 0;
    public GameObject gameOverContainer;

    public GameObject PlayerInstance;
    public GameObject[] EnemiesInstance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
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

    // Called when the AR target is found
    public void OnTargetFound() { 
        Debug.Log("Target found, spawning player and enemies.");
        Invoke(nameof(SpawnPlayer), 5f); // Spawn player in 5 seconds
        InvokeRepeating(nameof(SpawnEnemies), 5f, 10f); // Spawn enemies every 10 seconds after 5 seconds
    }

    // Called when the AR target is lost
    public void OnTargetLost() { 
        Debug.Log("Target lost, cleaning up player and enemies.");
        ClearPlayerAndEnemies(); // Clear player and enemies from the scene
        CancelInvoke(nameof(SpawnPlayer)); // Cancel any pending player spawn
        CancelInvoke(nameof(SpawnEnemies)); // Cancel any pending enemy spawns
    }

    private void SpawnPlayer() { 
        PlayerInstance = Instantiate(playerPrefab, 
            playerSpawnPoint.position, playerSpawnPoint.rotation);
        PlayerInstance.transform.SetParent(imageTarget.transform); // Parent the player to the image target for AR tracking
    }

    private void SpawnEnemies() {
        EnemiesInstance = new GameObject[numberOfEnemies];
        for(int i = 0; i < numberOfEnemies; i++) { 
            EnemiesInstance[i] = Instantiate(enemiesPrefab,
                enemiesSpawnPoint[i].position, enemiesSpawnPoint[i].rotation);
            EnemiesInstance[i].transform.SetParent(imageTarget.transform); // Parent the enemies to the image target for AR tracking
        }
    }

    public void GameOver() { 
        Handheld.Vibrate(); // Vibrate the device to indicate game over
        _isGameOver = true;
        ClearPlayerAndEnemies(); // Clear player and enemies from the scene
        Time.timeScale = 0f; // Pause the game
        gameOverContainer.SetActive(true);
    }

    public void RestartGame() { 
        Time.timeScale = 1f; // Resume the game
        gameOverContainer.SetActive(false);
        score = 0; // Reset score
        _isGameOver = false;
    }

    // Method to adjust the score, can be called from other scripts when player scores points
    public void AdjustScore(int scoreAmount) { 
        score += scoreAmount;
        Debug.Log("Score adjusted: " + score);
    }

    public void ClearPlayerAndEnemies() { 
        if (PlayerInstance != null) { 
            Destroy(PlayerInstance);
        }
        if(EnemiesInstance != null) { 
            foreach(var enemy in EnemiesInstance) { 
                if(enemy != null) { 
                    Destroy(enemy);
                }
            }
            EnemiesInstance = null; // Clear the reference to the enemies array
        }
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
