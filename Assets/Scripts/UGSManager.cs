using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.Collections.Generic;

public class UGSManager : MonoBehaviour
{
    public TMPro.TMP_InputField playerInputName;

    public Hiscore hiscore;

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

    public async void SetPlayerName()
    {
        if(playerInputName == null)
        {
            playerInputName = GameObject.Find("PlayerNameInputField").GetComponent<TMPro.TMP_InputField>();
        }
        string playerName = playerInputName.text;
        
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            Debug.Log("Player name set to: " + playerName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public async void SubmitScore(int score)
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        SetPlayerName();
        
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Hiscore", score);
            Debug.Log("Score submitted: " + score);
            AuthenticationService.Instance.SignOut();
            PlayerPrefs.DeleteKey("unity.player_id");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }  
    }

    public async void GetTopScores()
    {
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            }

            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync("Hiscore", new GetScoresOptions { Limit = 10 });

            Dictionary<string, int> dict = new Dictionary<string, int>();

            foreach (var entry in scoresResponse.Results)
            {
                Debug.Log($"{entry.Rank + 1}. {entry.PlayerName ?? "Anonymous"} - {entry.Score}");
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

    public async void SubmitScoreWithNewID(int score)
    {
        if (playerInputName == null)
        {
            playerInputName = GameObject.Find("PlayerNameInputField").GetComponent<TMPro.TMP_InputField>();
        }
        string playerName = playerInputName.text;

        try
        {
            // Force new player ID
            AuthenticationService.Instance.SignOut();
            PlayerPrefs.DeleteKey("unity.player_id");
            PlayerPrefs.Save();

            AuthenticationService.Instance.ClearSessionToken();

            // Sign in anonymously (new player)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("New Player ID: " + AuthenticationService.Instance.PlayerId);

            // Set the player name
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

            // Submit the score
            await LeaderboardsService.Instance.AddPlayerScoreAsync("Hiscore", score);

            Debug.Log($"Submitted score: {score} for {playerName}");
        }
        catch (Exception e)
        {
            Debug.LogError("Error submitting score: " + e.Message);
        }
    }
}
