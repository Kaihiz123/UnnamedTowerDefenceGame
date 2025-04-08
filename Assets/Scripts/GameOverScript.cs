using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI playerStatsText;
    [SerializeField] public GameObject GameOverScreen;
    private GameTimer gameTimer;
    private WaveManager waveManager;
    public int WavesReached { get; private set; }
    public string TimePlayed { get; private set; }
    public int TotalEnemiesKilled { get; private set; }
    public int BasicEnemiesKilled { get; private set; }
    public int FastEnemiesKilled { get; private set; }
    public int TankyEnemiesKilled { get; private set; }
    public float BasicTowerDamage { get; private set; }
    public float SniperTowerDamage { get; private set; }
    public float AoETowerDamage { get; private set; }
    public float TotalDamage { get; private set; }

    private void Start()
    {
        gameTimer = FindFirstObjectByType<GameTimer>();
        waveManager = FindFirstObjectByType<WaveManager>();
    }

    public void ShowGameOverScreen()
    {
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

        if (waveManager != null)
        {
            waveManager.DisableWaveText();
        }
        
        // Collect statistics to show
        CollectGameStatistics();
        UpdatePlayerStatsText();        
        GameOverScreen.SetActive(true);
    }

    private void CollectGameStatistics()
    {
        if (gameTimer != null)
        {
            TimePlayed = gameTimer.GetFormattedTime();
        }

        if (waveManager != null)
        {
            WavesReached = waveManager.GetWavesReached();
        }
        
        if (StatisticsTracker.Instance != null)
        {
            BasicEnemiesKilled = StatisticsTracker.Instance.GetKillsByEnemyType(EnemyScript.EnemyType.Basic);
            FastEnemiesKilled = StatisticsTracker.Instance.GetKillsByEnemyType(EnemyScript.EnemyType.Fast);
            TankyEnemiesKilled = StatisticsTracker.Instance.GetKillsByEnemyType(EnemyScript.EnemyType.Tanky);
            TotalEnemiesKilled = StatisticsTracker.Instance.GetTotalEnemyKills();
            
            BasicTowerDamage = StatisticsTracker.Instance.GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.Basic);
            SniperTowerDamage = StatisticsTracker.Instance.GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.Sniper);
            AoETowerDamage = StatisticsTracker.Instance.GetTotalEffectiveDamageByTowerType(TowerInfo.TowerType.AOE);
            TotalDamage = StatisticsTracker.Instance.GetTotalEffectiveDamage();
        }
    }

    private void UpdatePlayerStatsText()
    {
        // Make it prettier this is just a block of text
        if (playerStatsText != null)
        {
            string statsMessage =$"Time Played: {TimePlayed}\n" +
                                 $"Wave reached: {WavesReached}\n\n" +
                                 $"Enemies Killed:\n" +
                                 $"Basic: {BasicEnemiesKilled}\n" +
                                 $"Fast: {FastEnemiesKilled}\n" +
                                 $"Tanky: {TankyEnemiesKilled}\n" +
                                 $"Total: {TotalEnemiesKilled}\n\n" +
                                 $"Damage Dealt:\n" +
                                 $"Basic Towers: {BasicTowerDamage:N0}\n" +
                                 $"Sniper Towers: {SniperTowerDamage:N0}\n" +
                                 $"AoE Towers: {AoETowerDamage:N0}\n" +
                                 $"Total Damage: {TotalDamage:N0}";
        
            playerStatsText.text = statsMessage;
        }
    }

    public void BackToMainMenuButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
