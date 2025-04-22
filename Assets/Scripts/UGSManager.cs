using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Collections.Generic;
using Unity.Services.Leaderboards.Models;

public class UGSManager : MonoBehaviour
{
    public Hiscore hiscore;
    public GameOverScript gameOverScript;

    public static UGSManager Instance { get; private set; }

    async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            await InitializeUGS();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private async Task InitializeUGS()
    {
        await UnityServices.InitializeAsync();
    }

    public async void GetPlacementInTheLeaderboard(int score)
    {
        // Force new player ID
        AuthenticationService.Instance.SignOut();
        PlayerPrefs.DeleteKey("unity.player_id");
        PlayerPrefs.Save();

        AuthenticationService.Instance.ClearSessionToken();

        // Sign in anonymously (new player)
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await LeaderboardsService.Instance.AddPlayerScoreAsync("Hiscore", score);

        LeaderboardEntry myEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync("Hiscore");
        int rank = myEntry.Rank + 1; // +1 because UGS ranks are 0-based

        if(gameOverScript == null)
        {
            gameOverScript = FindFirstObjectByType<GameOverScript>();
        }

        string text = "";
        bool placedInTheTop20 = false;
        if(rank <= 20)
        {
            text = "You placed #" + rank + " in the world!";
            placedInTheTop20 = true;
        }
        else
        {
            text = "You didn't place in the top20";
        }
        
        gameOverScript.AddPlacementText(text, placedInTheTop20);
    }

    public async void GetTopScores()
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("Hiscore", new GetScoresOptions { Limit = 20 });

            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (var entry in scoresResponse.Results)
            {
                dict.Add(entry.PlayerName, (int) entry.Score);
            }

            if(hiscore == null)
            {
                hiscore = GameObject.Find("HiscorePanel").GetComponent<Hiscore>();
            }

            hiscore.ShowLeaderboard(dict);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to fetch leaderboard: " + e.Message);
        }
    }

    public async void SubmitScore(string playerName)
    {
        if (playerName.Equals(""))
        {
            playerName = "Anon";
        }

        try
        {
            // Set the player name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        catch (Exception e)
        {
            Debug.LogError("Error submitting score: " + e.Message);
        }
    }
}
