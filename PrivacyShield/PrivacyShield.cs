using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System;
using System.Threading.Tasks;

namespace PrivacyShield
{
    public class PrivacyShield : NeosMod
    {
        public override string Name => "PrivacyShield";
        public override string Author => "ljoonal, eia485";
        public override string Version => "0.3.1";
        public override string Link => "https://github.com/EIA485/NeosPrivacyShield/";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("xyz.ljoonal.neos.privacyshield");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(AssetManager), nameof(AssetManager.RequestGather))]
        private class PrivacyShieldPatch
        {

            private static bool Prefix(AssetManager __instance, AssetGatherer ___assetGatherer, ref ValueTask<string> __result, Uri __0, FrooxEngine.Priority __1, CloudX.Shared.NeosDB_Endpoint? __2)
            {
                __result = HandleRequest(__instance, ___assetGatherer, __0, __1, __2);
                return false;
            }
        }

        private static async ValueTask<string> HandleRequest(AssetManager assetManager, AssetGatherer assetGatherer, Uri uri, FrooxEngine.Priority priority, CloudX.Shared.NeosDB_Endpoint? endpointOverwrite)
        {
            if (uri.Scheme == "neosdb" || uri.Scheme == "local" || await AskForPermission(assetManager.Engine, uri, "PrivacyShield generic request"))
                return await assetGatherer.Gather(uri, priority, endpointOverwrite);
            else
                throw new Exception("No permissions to load asset");
        }

        private static async Task<bool> AskForPermission(Engine engine, Uri target, String accessReason)
        {
            Debug("Asking permissions for", target);
            HostAccessPermission perms = await
            engine.Security.RequestAccessPermission(target.Host, target.Port, accessReason);
            return perms == HostAccessPermission.Allowed;
        }
    }
}
