using Landfall.Modding;
using Zorro.UI.Modal;

namespace HasteRestartShardMod;

[LandfallPlugin]
public class RestartShardOptionsMod
{
    static RestartShardOptionsMod()
    {
        On.EscapeMenuAbandonPage.OnQuickRestartButtonClicked += static (_original, _escapeMenuMainPage) =>
        {
            OpenRestartShardPrompt();
        };
    }

    static void OpenRestartShardPrompt()
    {
        Modal.OpenModal(
            new DefaultHeaderModalOption(
                "Restart Run",
                string.Join("\n", [
                    "You will lose all progress from this run",
                    "How would you like to restart?"
                ]
            )),
            new ModalButtonsOption([
                new(text: "New seed", () => RestartRun(keepSeed: false)),
                new(text: "Same seed", () => RestartRun(keepSeed: true)),
                new(text: "Cancel", () => { /* No-op. Modal already closes itself for us :^) */ })
            ])
        );
    }

    private static void RestartRun(bool keepSeed)
    {
        var config = RunHandler.config;
        var shardID = RunHandler.RunData.shardID;
        var seed = keepSeed
            ? RunHandler.RunData.currentSeed
            : RunHandler.GenerateSeed();

        RunHandler.ClearCurrentRun();
        RunHandler.StartAndPlayNewRun(config, shardID, seed);
    }
}
