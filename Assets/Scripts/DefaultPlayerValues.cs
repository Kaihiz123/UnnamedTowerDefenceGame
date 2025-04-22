using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlayerValues", menuName = "Scriptable Objects/DefaultPlayerValues")]
public class DefaultPlayerValues : ScriptableObject
{
    public int defaultPlayerStartMoney = 200;
    public int defaultPlayerStartHealth = 100;
    public int defaultPlayerMaxHealth = 100;
}
