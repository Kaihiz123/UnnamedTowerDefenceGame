using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerBaseHealthBar playerBaseHealthBar;
    public BloomActivator bloomActivator;

    public delegate void ShowEnemyHealthBar(bool show);
    public static event ShowEnemyHealthBar OnShowEnemyHealthBar;

    public delegate void EnablePostProcessingBloom(bool enable);
    public static event EnablePostProcessingBloom OnEnableBloom;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // this is called when scene loads


        //TODO: go through all the options such as anti aliasing, showFPS...

        OnEnableBloom(PlayerPrefs.GetInt(ISettings.Type.BLOOM.ToString()) == 1);
        ShowPlayerHealthBar(PlayerPrefs.GetInt(ISettings.Type.SHOWPLAYERHEALTHBAR.ToString()) == 1);
        ShowEnemyHealthBars(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString()) == 1);
    }

    public void ShowPlayerHealthBar(bool show)
    {
        playerBaseHealthBar.gameObject.SetActive(show);
    }

    public void ShowEnemyHealthBars(bool show)
    {
        OnShowEnemyHealthBar?.Invoke(show);
    }

    public void EnableBloom(bool enable)
    {
        OnEnableBloom?.Invoke(enable);
    }

    private void OnEnable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar += ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar += ShowEnemyHealthBars;
        MenuSettingsPanelScript.OnEnableBloom += EnableBloom;
    }

    private void OnDisable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar -= ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar -= ShowEnemyHealthBars;
        MenuSettingsPanelScript.OnEnableBloom -= EnableBloom;
    }
}
