using UnityEngine;

public interface ISettings
{
    public enum Type
    {
        // game settings
        STARTMONEY,
        STARTHEALTH,
        MAXHEALTH,
        DIFFICULTY,
        BREATHERBETWEENWAVES,

        // actual settings

        // graphics
        UIRESOLUTION,
        FULLSCREEN,
        REFRESHRATE,
        SHOWFPS,
        WINDOWMODE,
        VERTICALSYNC,
        ANTIALIAS,
        BRIGHTNESS,
        BLOOM,
        LIGHTQUALITY,
        SHADOWQUALITY,

        // audio
        MASTERVOLUME,
        MUSICVOLUME,
        SOUNDEFFECTVOLUME,
        UIVOLUME,
        MUTEMUSICONPAUSE,

        // gameplay
        SHOWPLAYERHEALTHBAR,
        SHOWENEMYHEALTHBAR
    }

    public void ValueChanged<T>(Type type, T newValue) where T : struct;

    public void ShowFPSPanel(bool show);

    public void ShowPlayerHealthBarInGameScene(bool show);
    public void ShowEnemyHealthBarInGameScene(bool show);

    public void EnableBloom(bool enable);
}
