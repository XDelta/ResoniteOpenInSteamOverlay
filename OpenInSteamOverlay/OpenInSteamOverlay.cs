using Elements.Core;
using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using Steamworks;

namespace OpenInSteamOverlay;

public class OpenInSteamOverlay : ResoniteMod {
	internal const string VERSION_CONSTANT = "1.0.0";
	public override string Name => "OpenInSteamOverlay";
	public override string Author => "Delta";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/XDelta/ResoniteOpenInSteamOverlay";

	[AutoRegisterConfigKey]
	private static readonly ModConfigurationKey<bool> Enabled = new("Enabled", "Open links in the steam overlay instead of your default browser", () => true);

	internal static ModConfiguration Config;

	public override void OnEngineInit() {
		Config = GetConfiguration();
		Config.Save(true);

		Harmony harmony = new("net.deltawolf.OpenInSteamOverlay");
		harmony.PatchAll();
	}

	[HarmonyPatch(typeof(HyperlinkOpenDialog), nameof(HyperlinkOpenDialog.Open))]
	class CopyHyperlinkPatch {
		public static bool Prefix(HyperlinkOpenDialog __instance) {
			if (!Config.GetValue(Enabled)) {
				return true;
			}
			if (__instance.World != Userspace.UserspaceWorld) {
				return false;
			}
			if (__instance.URL.Value != null && (__instance.URL.Value.Scheme == "http" || __instance.URL.Value.Scheme == "https")) {
				__instance.RunInBackground(delegate {
					SteamFriends.ActivateGameOverlayToWebPage(__instance.URL.Value.ToString());
				}, WorkType.Background);
			}
			return false;
		}
	}
}
