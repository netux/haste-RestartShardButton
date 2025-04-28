using Landfall.Modding;

namespace HasteRestartShardMod;

[LandfallPlugin]
public class RestartShardMod
{
    public static RestartShardButton? RestartShardButton;

    static RestartShardMod()
    {
        On.EscapeMenuMainPage.OnPageEnter += static (original, escapeMenuMainPage) =>
        {
            original(escapeMenuMainPage);

            if (RestartShardButton == null || RestartShardButton.gameObject == null)
            {
                RestartShardButton = RestartShardButton.Setup(escapeMenuMainPage);
            }

            RestartShardButton.gameObject.SetActive(RunHandler.InRun);
        };
    }
}
