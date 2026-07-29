using Dalamud.Interface.Components;
using ECommons.SplatoonAPI;
using Lumina.Excel.Sheets;
using NightmareUI.PrimaryUI;

namespace TextAdvance.Gui;

internal static class TabConfig
{
    public static string Filter = "";
    internal static void Draw()
    {
        if (P.BlockList.Count > 0)
        {
            ImGuiEx.TextWrapped(EColor.RedBright, $"TextAdvance 已被下列插件暫停：{string.Join(",", P.BlockList)}");
            if (ImGui.SmallButton("移除所有鎖定"))
            {
                P.BlockList.Clear();
            }
        }

        if (S.IPCProvider.IsInExternalControl())
        {
            ImGuiEx.TextWrapped(EColor.RedBright, $"TextAdvance 正由 {S.IPCProvider.Requester} 外部控制，目前會忽略你的所有設定。");
            if (ImGui.SmallButton("取消外部控制"))
            {
                S.IPCProvider.Requester = null;
                S.IPCProvider.ExternalConfig = null;
            }
        }

        new NuiBuilder()
        .Section("插件運作")
        .Widget(() =>
        {
            ImGui.Checkbox("啟用插件（不保存狀態）", ref P.Enabled);
            ImGui.Checkbox("登出時不要自動停用插件", ref C.DontAutoDisable);
            ImGui.Checkbox("啟用任務目標指示器（全域、保存狀態）", ref C.QTIEnabled);
            ImGui.Indent();
            ImGui.Checkbox("插件停用時仍啟用指示器", ref C.QTAEnabledWhenTADisable);
            ImGui.Checkbox("插件停用時仍啟用搜尋器", ref C.QTAFinderEnabledWhenTADisable);
            if (C.QTIEnabled)
            {
                if (!Splatoon.IsConnected())
                {
                    ImGuiEx.TextWrapped(EColor.PurpleBright, "任務目標指示器需要安裝並啟用 Splatoon 插件才能運作");
                }
            }
            ImGui.Unindent();
            ImGui.SetNextItemWidth(150f);
            ImGui.SliderFloat("自動互動最大半徑", ref C.AutoInteractMaxRadius, 3f, 10f);
            ImGuiEx.HelpMarker("降低此半徑可能造成問題", ImGuiColors.DalamudOrange, FontAwesomeIcon.ExclamationTriangle.ToIconString());
        })

        .Section("按鍵綁定")
        .Widget(() =>
        {
            ImGui.Text("插件啟用時，按住此鍵可暫時停用：");
            ImGui.SetNextItemWidth(200f);
            ImGuiEx.EnumCombo("##HoldDisable", ref C.TempDisableButton);
            ImGui.Text("插件停用時，按住此鍵可暫時啟用：");
            ImGui.SetNextItemWidth(200f);
            ImGuiEx.EnumCombo("##HoldEnable", ref C.TempEnableButton);
        })

        .Section("功能")
        .Widget(() =>
        {
            ImGui.Checkbox("自動接受任務（QA）", ref C.MainConfig.EnableQuestAccept);
            ImGuiComponents.HelpMarker("與任務起始 NPC 對話時自動接受新任務");
            ImGui.Checkbox("自動完成任務（QC）", ref C.MainConfig.EnableQuestComplete);
            ImGuiComponents.HelpMarker("與任務完成 NPC 對話時自動完成任務");
            ImGui.Checkbox("自動選擇報酬（RP）", ref C.MainConfig.EnableRewardPick);
            ImGuiComponents.HelpMarker("依照下方設定的簡易規則，自動選擇任務完成報酬");
            ImGui.Checkbox("自動略過對話（TS）", ref C.MainConfig.EnableTalkSkip);
            ImGuiComponents.HelpMarker("自動推進大多數字幕；部分字幕可能仍需手動推進。");
            ImGui.Checkbox("自動確認繳交物品（RH）", ref C.MainConfig.EnableRequestHandin);
            ImGuiComponents.HelpMarker("放入物品後自動確認大多數繳交視窗；部分繳交可能無法自動確認。");
            ImGui.Checkbox("自動放入所需物品（RF）", ref C.MainConfig.EnableRequestFill);
            ImGuiComponents.HelpMarker("自動在物品需求視窗中放入物品；部分需求可能無法自動處理。");
            if (C.MainConfig.EnableRequestFill)
            {
                ImGui.Indent();
                ImGui.SetNextItemWidth(200f);
                if (ImGui.BeginCombo("##RequestFillQuality", C.MainConfig.RequestFillQualityPreference.ToString()))
                {
                    if (ImGui.Selectable("NQ", C.MainConfig.RequestFillQualityPreference == RequestFillQualityPreference.NQ))
                        C.MainConfig.RequestFillQualityPreference = RequestFillQualityPreference.NQ;
                    if (ImGui.Selectable("HQ", C.MainConfig.RequestFillQualityPreference == RequestFillQualityPreference.HQ))
                        C.MainConfig.RequestFillQualityPreference = RequestFillQualityPreference.HQ;
                    if (ImGui.Selectable("任意品質", C.MainConfig.RequestFillQualityPreference == RequestFillQualityPreference.Any))
                        C.MainConfig.RequestFillQualityPreference = RequestFillQualityPreference.Any;
                    ImGui.EndCombo();
                }
                ImGui.SameLine();
                ImGuiComponents.HelpMarker("NQ：優先使用普通品質物品，保留較珍貴的 HQ 物品。\nHQ：優先使用高品質物品。\n任意品質：使用第一個可用物品（遊戲預設）。");
                ImGui.Unindent();
            }
            ImGui.Checkbox("過場動畫期間自動按 ESC（CS）", ref C.MainConfig.EnableCutsceneEsc);
            ImGuiComponents.HelpMarker("過場動畫可略過時自動按下 ESC；不會略過原本不可略過的過場動畫。");
            ImGui.Checkbox("自動確認略過過場動畫（CC）", ref C.MainConfig.EnableCutsceneSkipConfirm);
            ImGuiComponents.HelpMarker("按下 ESC 後自動確認略過過場動畫。");
            ImGui.Checkbox("自動與任務相關物件互動（IN）", ref C.MainConfig.EnableAutoInteract);
            ImGuiComponents.HelpMarker("自動與附近的任務相關 NPC 和物件互動。");
        })

        .Section("浮動資訊")
        .Widget(() =>
        {
            ImGui.Checkbox("插件啟用時顯示浮動資訊", ref C.EnableOverlay);
            if (C.EnableOverlay)
            {
                ImGui.SetNextItemWidth(100f);
                ImGui.DragFloat2("浮動資訊偏移", ref C.OverlayOffset);
            }
        })

        .Section("通知")
        .Widget(() =>
        {
            ImGui.Checkbox("停用手動切換插件狀態的通知", ref C.NotifyDisableManualState);
            ImGui.Checkbox("角色設為自動啟用時，停用登入通知", ref C.NotifyDisableOnLogin);
            ImGui.Checkbox("停用選擇報酬的聊天訊息", ref C.PickRewardSilent);
        })

        .Section("報酬選擇順序")
        .Widget(() =>
        {
            ImGuiEx.TextWrapped("請注意：無法保證判斷完全準確。");
            ImGuiEx.EnumOrderer("", C.PickRewardOrder);
        })

        .Section("導航")
        .Widget(() =>
        {
            ImGui.Checkbox("啟用##autointeract", ref C.Navmesh);
            ImGuiEx.PluginAvailabilityIndicator([new("vnavmesh")]);
            ImGui.Checkbox("抵達後自動互動", ref C.NavmeshAutoInteract);
            ImGui.SetNextItemWidth(200f);
            var current = C.Mount == -1 ? "不使用坐騎" : (C.Mount == 0 ? "隨機坐騎" : $"{Utils.GetMountName(C.Mount)}");
            if (ImGui.BeginCombo($"##mount", current))
            {
                ImGui.InputTextWithHint("##fltr", "搜尋", ref Filter, 50);
                if (ImGui.Selectable("不使用坐騎", C.Mount == -1)) C.Mount = -1;
                if (ImGui.Selectable("隨機坐騎", C.Mount == 0)) C.Mount = 0;
                foreach (var x in Svc.Data.GetExcelSheet<Mount>())
                {
                    var n = x.Singular.ExtractText();
                    if (n == "") continue;
                    if (Filter != "" && !n.Contains(Filter, StringComparison.OrdinalIgnoreCase)) continue;
                    if (C.Mount == x.RowId && ImGui.IsWindowAppearing()) ImGui.SetScrollHereY();
                    if (ImGui.Selectable($"{n}##{x.RowId}", C.Mount == x.RowId))
                    {
                        C.Mount = (int)x.RowId;
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.Checkbox("可用時使用衝刺與速行", ref C.UseSprintPeloton);
            ImGui.Checkbox("在聊天視窗顯示導航狀態", ref C.NavStatusChat);
            ImGui.Checkbox("允許飛行（高度實驗性）", ref C.EnableFlight);
            //ImGui.Checkbox($"Allow teleporting to the nearest Aetheryte to a flag", ref C.EnableTeleportToFlag);
        })

        .Draw();
    }
}
