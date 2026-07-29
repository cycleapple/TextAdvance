using ECommons.Automation;
using ECommons.Automation.LegacyTaskManager;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using TextAdvance.Executors;
using Callback = ECommons.Automation.Callback;

namespace TextAdvance.Gui;

internal static unsafe class TabDebug
{
    private static TaskManager TestTaskManager;
    internal static void Draw()
    {
        if (ImGui.CollapsingHeader("IPC 測試"))
        {
            ImGuiEx.Text($"""
                IsEnabled {S.IPCTester.IsEnabled()}
                GetEnableQuestAccept {S.IPCTester.GetEnableQuestAccept()}
                GetEnableQuestComplete {S.IPCTester.GetEnableQuestComplete()}
                GetEnableRewardPick {S.IPCTester.GetEnableRewardPick()}
                GetEnableCutsceneEsc {S.IPCTester.GetEnableCutsceneEsc()}
                GetEnableCutsceneSkipConfirm {S.IPCTester.GetEnableCutsceneSkipConfirm()}
                GetEnableRequestHandin {S.IPCTester.GetEnableRequestHandin()}
                GetEnableRequestFill {S.IPCTester.GetEnableRequestFill()}
                GetEnableTalkSkip {S.IPCTester.GetEnableTalkSkip()}
                GetEnableAutoInteract {S.IPCTester.GetEnableAutoInteract()}
                IsPaused {S.IPCTester.IsPaused()}
                """);
        }
        if (ImGui.CollapsingHeader("外部控制測試"))
        {
            var opts = Ref<ExternalTerritoryConfig>.Get("", () => new());
            ImGuiEx.Checkbox("EnableAutoInteract", ref opts.EnableAutoInteract);
            ImGuiEx.Checkbox("EnableCutsceneEsc", ref opts.EnableCutsceneEsc);
            ImGuiEx.Checkbox("EnableCutsceneSkipConfirm", ref opts.EnableCutsceneSkipConfirm);
            ImGuiEx.Checkbox("EnableQuestAccept", ref opts.EnableQuestAccept);
            ImGuiEx.Checkbox("EnableQuestComplete", ref opts.EnableQuestComplete);
            ImGuiEx.Checkbox("EnableRequestFill", ref opts.EnableRequestFill);
            ImGuiEx.Checkbox("EnableRequestHandin", ref opts.EnableRequestHandin);
            ImGuiEx.Checkbox("EnableRewardPick", ref opts.EnableRewardPick);
            ImGuiEx.Checkbox("EnableTalkSkip", ref opts.EnableTalkSkip);
            ImGuiEx.Text($"目前受外部控制：{S.IPCTester.IsInExternalControl()}");
            if (ImGui.Button("啟用外部控制（Plugin1）")) DuoLog.Information(S.IPCTester.EnableExternalControl("Plugin1", opts).ToString());
            if (ImGui.Button("啟用外部控制（Plugin2）")) DuoLog.Information(S.IPCTester.EnableExternalControl("Plugin2", opts).ToString());
            if (ImGui.Button("停用外部控制（Plugin1）")) DuoLog.Information(S.IPCTester.DisableExternalControl("Plugin1").ToString());
            if (ImGui.Button("停用外部控制（Plugin2）")) DuoLog.Information(S.IPCTester.DisableExternalControl("Plugin2").ToString());
        }
        if (ImGui.CollapsingHeader("過場動畫"))
        {

        }
        if (ImGui.CollapsingHeader("物品需求"))
        {
            if (TryGetAddonByName<AddonRequest>("Request", out var request) && IsAddonReady((AtkUnitBase*)request))
            {
                ImGuiEx.Text($"{request->EntryCount}");
            }
        }
        if (ImGui.Button("安裝 Hook")) Callback.InstallHook();
        if (ImGui.Button("解除安裝 Hook")) Callback.UninstallHook();
        if (ImGui.CollapsingHeader("防卡住"))
        {
            ImGuiEx.Text($"""
                上次位置：{S.MoveManager.LastPosition}
                上次更新：{S.MoveManager.LastPositionUpdate}（{Environment.TickCount64 - S.MoveManager.LastPositionUpdate} 毫秒前）
                執行中：{P.NavmeshManager.IsRunning()}
                動畫鎖定：{Player.IsAnimationLocked} / {Player.AnimationLock}
                """);
        }
        if (ImGui.CollapsingHeader("任務標記"))
        {
            var markers = AgentHUD.Instance()->MapMarkers.AsSpan();
            for (var i = 0; i < markers.Length; i++)
            {
                var marker = markers[i];
                if (ThreadLoadImageHandler.TryGetIconTextureWrap(marker.IconId, false, out var tex))
                {
                    ImGui.Image(tex.Handle, tex.Size);
                }
                ImGuiEx.Text($"{marker.IconId} / {marker.Position} / {Vector3.Distance(Player.Position, marker.Position)}");
                ImGui.Separator();
            }
        }
        if (ImGui.Button("複製目標描述"))
        {
            if (Svc.Targets.Target != null) Copy(new ObjectDescriptor(Svc.Targets.Target, true).AsCtorString());
        }
        if (ImGui.CollapsingHeader("自動互動"))
        {
            ImGuiEx.Text($"目標：{ExecAutoInteract.WasInteracted(Svc.Targets.Target)}");
            ImGuiEx.Text($"已自動互動的物件：{ExecAutoInteract.InteractedObjects.Print("\n")}");
        }
        if (ImGui.CollapsingHeader("任務"))
        {
            ImGuiEx.Text($"{Utils.GetQuestArray().Print("\n")}");
        }
        if (ImGui.CollapsingHeader("報酬選擇"))
        {
            if (TryGetAddonByName<AtkUnitBase>("JournalResult", out var addon) && IsAddonReady(addon))
            {
                var canvas = addon->UldManager.NodeList[7];
                var r = new ReaderJournalResult(addon);
                ImGuiEx.Text($"報酬：\n{r.OptionalRewards.Select(x => $"ID:{x.ItemID} / 圖示:{x.IconID} / 數量:{x.Amount} / 名稱:{x.Name} ").Print("\n")}");
                for (var i = 0; i < 5; i++)
                {
                    if (ImGui.Button($"{i}"))
                    {
                        S.Memory.PickRewardItemUnsafe((nint)canvas->GetComponent(), i);
                    }
                }
                if (ImGui.Button("壓力測試"))
                {
                    TestTaskManager ??= new();
                    for (var i = 0; i < 1000; i++)
                    {
                        var x = i % 5;
                        TestTaskManager.Enqueue(() => S.Memory.PickRewardItemUnsafe((nint)canvas->GetComponent(), x));
                    }
                }
                if (TestTaskManager != null)
                {
                    ImGuiEx.Text($"工作 {TestTaskManager.MaxTasks - TestTaskManager.NumQueuedTasks}/{TestTaskManager.MaxTasks}");
                }
                if (ImGui.Button("停止壓力測試"))
                {
                    TestTaskManager.Abort();
                }
            }
        }
    }
}
