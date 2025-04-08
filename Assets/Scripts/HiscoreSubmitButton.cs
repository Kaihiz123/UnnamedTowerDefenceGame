using UnityEngine;

public class HiscoreSubmitButton : MonoBehaviour
{
    public void Submit()
    {
        UGSManager.Instance.SubmitScoreWithNewID(12);
    }
}
