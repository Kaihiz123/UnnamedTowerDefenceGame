using UnityEngine;

public class Cleanup
{
    public void ForceCleanup()
    {
        // Force UI rebuilds
        Canvas.ForceUpdateCanvases();
        
        // Force mesh garbage collection
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
