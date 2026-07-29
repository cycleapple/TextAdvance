using Dalamud.Interface.Components;

namespace TextAdvance.Gui;

internal static class TabTerritory
{
    private static uint SelectedKey = uint.MaxValue;
    private static bool OnlyModded = false;
    private static string Filter = string.Empty;
    internal static void Draw()
    {
        ImGui.Checkbox("全域啟用狀態優先於區域設定", ref C.GlobalOverridesLocal);
        ImGuiEx.TextWrapped("勾選後，使用 /at 指令啟用插件時將忽略各區域設定，改用全域設定。\n" +
            "若未勾選，無論插件的全域狀態為何，都會使用各區域設定。");
        ImGuiEx.Text("目前插件狀態：全域");
        ImGui.SameLine(0, 0);
        ImGuiEx.Text(P.Enabled ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, P.Enabled ? "已啟用" : "已停用");
        ImGui.SameLine(0, 0);
        ImGuiEx.Text("，目前區域");
        ImGui.SameLine(0, 0);
        ImGuiEx.Text(P.IsTerritoryEnabled() ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed, P.IsTerritoryEnabled() ? "已啟用" : "已停用");
        ImGuiEx.SetNextItemFullWidth();
        if (ImGui.BeginCombo("##terrselect", P.TerritoryNames.TryGetValue(SelectedKey, out var selected) ? selected : "選擇區域……"))
        {
            ImGui.SetNextItemWidth(200f);
            ImGui.InputTextWithHint("##selectflts", "篩選", ref Filter, 50);
            ImGui.SameLine();
            ImGui.Checkbox("僅顯示已修改", ref OnlyModded);
            if (P.TerritoryNames.TryGetValue(Svc.ClientState.TerritoryType, out var current) && ImGui.Selectable($"目前區域：{current}"))
            {
                SelectedKey = Svc.ClientState.TerritoryType;
            }
            ImGui.Separator();
            foreach (var x in P.TerritoryNames)
            {
                if (Filter != string.Empty && !x.Value.Contains(Filter, StringComparison.OrdinalIgnoreCase)) continue;
                if (OnlyModded && !C.TerritoryConditions.ContainsKey(x.Key)) continue;
                if (ImGui.Selectable(x.Value, C.TerritoryConditions.ContainsKey(x.Key)))
                {
                    SelectedKey = x.Key;
                }
                if (ImGui.IsWindowAppearing() && SelectedKey == x.Key)
                {
                    ImGui.SetScrollHereY();
                }
            }
            ImGui.EndCombo();
        }
        if (P.TerritoryNames.ContainsKey(SelectedKey))
        {
            if (C.TerritoryConditions.TryGetValue(SelectedKey, out var settings))
            {
                if (ImGui.Button("移除自訂設定"))
                {
                    C.TerritoryConditions.Remove(SelectedKey);
                }
                ImGui.Checkbox("自動接受任務", ref settings.EnableQuestAccept);
                ImGui.Checkbox("自動完成任務", ref settings.EnableQuestComplete);
                ImGui.Checkbox("自動選擇報酬（RP，測試版）", ref settings.EnableRewardPick);
                ImGui.Checkbox("自動略過對話", ref settings.EnableTalkSkip);
                ImGui.Checkbox("半自動繳交物品", ref settings.EnableRequestHandin);
                ImGui.Checkbox("自動放入所需物品（RF，新功能）", ref settings.EnableRequestFill);
                ImGui.Checkbox("過場動畫期間自動按 ESC", ref settings.EnableCutsceneEsc);
                ImGui.Checkbox("自動確認略過過場動畫", ref settings.EnableCutsceneSkipConfirm);
                ImGui.Checkbox("自動與任務相關物件互動（IN）", ref settings.EnableAutoInteract);
                ImGuiComponents.HelpMarker("自動與附近的任務相關 NPC 和物件互動。");
                ImGui.Separator();
                ImGui.Checkbox("顯示任務目標指示器", ref settings.QTIQuestEnabled);
                ImGui.ColorEdit4("任務目標指示器顏色", ref settings.QTIQuestColor, ImGuiColorEditFlags.NoInputs);
                ImGui.Checkbox("任務目標指示器連線", ref settings.QTIQuestTether);
                ImGui.SetNextItemWidth(60f);
                ImGui.DragFloat("任務目標指示器粗細", ref settings.QTIQuestThickness, 0.02f, 1f, 10f);
            }
            else
            {
                ImGuiEx.Text("此區域尚無自訂設定。");
                if (ImGui.Button("建立自訂設定"))
                {
                    C.TerritoryConditions[SelectedKey] = new();
                }
            }
        }
    }
}
