public class SettingsManager
{
    private static SettingsManager _instance;
    public static SettingsManager Instance => _instance ??= new SettingsManager();
    


    // Globally access/change any of the variables here. Handy!

    public bool DebugON = true;
    public float playerHealth;
    public float playerMaxHealth;



    /* USAGE, in ANY script:

    if (SettingsManager.Instance.DebugON)
    {
        Debug.Log("Debug mode is ON!");
    } 

    */
    private SettingsManager() { } // Private constructor prevents external instantiation
}
