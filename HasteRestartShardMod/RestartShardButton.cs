using Landfall.Haste;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Localization;
using Zorro.UI.Modal;

namespace HasteRestartShardMod;

public class RestartShardButton : MonoBehaviour
{
    public void Awake()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OpenRestartShardPrompt);
    }

    private void RestartRun(bool keepSeed)
    {
        var config = RunHandler.config;
        var shardID = RunHandler.RunData.shardID;
        var seed = keepSeed
            ? RunHandler.RunData.currentSeed
            : RunHandler.GenerateSeed();

        RunHandler.ClearCurrentRun();
        RunHandler.StartAndPlayNewRun(config, shardID, seed);
    }

    public void OpenRestartShardPrompt()
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

    public static RestartShardButton Setup(EscapeMenuMainPage escapeMenuMainPage)
    {
        RectTransform buttonsParent = (RectTransform)escapeMenuMainPage.transform.Find("Buttons");
        RectTransform referenceButton = (RectTransform)buttonsParent.Find("SettingsButton");
        // Why SettingsButton and not AbandonButton?  Well, the AbandonButton's icon is actually a rotated medical cross,
        // while our custom icon is made to be presented upright. So undoing the AbandonButton's rotation is hard.

        var restartShardButtonTransform = GameObject.Instantiate(referenceButton, parent: buttonsParent);
        restartShardButtonTransform.SetSiblingIndex(Math.Max(0, buttonsParent.Find("AbandonButton").GetSiblingIndex()));

        restartShardButtonTransform.name = "RestartButton";

        var restartShardButtonTextTransform = restartShardButtonTransform.Find("Text");
        restartShardButtonTextTransform.GetComponent<LocalizeUIText>().SetString(new UnlocalizedString("Restart Shard"));

        var restartShardButtonIconTransform = restartShardButtonTransform.Find("Icon");
        restartShardButtonIconTransform.GetComponent<Image>().sprite = GetButtonIconSprite();

        return restartShardButtonTransform.gameObject.AddComponent<RestartShardButton>();
    }

    private static Sprite? _redoIconSprite;
    private static Sprite GetButtonIconSprite()
    {
        if (_redoIconSprite == null)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream stream = executingAssembly.GetManifestResourceStream($"{nameof(HasteRestartShardMod)}.Assets.redo.png"))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                Texture2D texture = new(2, 2, TextureFormat.RGBA32, mipChain: false);
                texture.LoadImage(buffer);

                _redoIconSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            };
        }

        return _redoIconSprite;
    }
}