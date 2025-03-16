using UnityEngine;

public interface IConfirmation
{
    public void ConfirmationSucceeded(string whoAddressed);
    public void ConfirmationCanceled(string whoAddressed);
}
