using UnityEngine;

public class ConfirmationWindow : MonoBehaviour
{
    IConfirmation confirmation;

    public TMPro.TextMeshProUGUI titleText;

    string whoAddressed;

    public void Init(IConfirmation confirmation, string whoAddressed, string titleString = "Are You sure?")
    {
        this.confirmation = confirmation;
        titleText.text = titleString;
        this.whoAddressed = whoAddressed;
    }

    public void ConfirmButtonClicked()
    {
        confirmation.ConfirmationSucceeded(whoAddressed);
    }

    public void CancelButtonClicked()
    {
        confirmation.ConfirmationCanceled(whoAddressed);
    }
}
