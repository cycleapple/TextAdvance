namespace TextAdvance.Gui
{
    public static class TabSplatoon
    {
        public static void Draw()
        {
            ImGuiEx.TextWrapped("這些功能需要安裝並啟用 Splatoon 插件。");
            if (Svc.PluginInterface.InstalledPlugins.TryGetFirst(x => x.InternalName == "Splatoon", out var plugin))
            {
                if (plugin.IsLoaded)
                {
                    ImGuiEx.TextWrapped(EColor.Green, $"已安裝並啟用 Splatoon v{plugin.Version}。");
                }
                else
                {
                    ImGuiEx.TextWrapped(EColor.Red, $"已安裝 Splatoon v{plugin.Version}，但尚未啟用。");
                }
            }
            else
            {
                ImGuiEx.TextWrapped(EColor.Red, "尚未安裝 Splatoon。");
                if (ImGui.Button("取得 Splatoon")) ShellStart("https://puni.sh/plugin/Splatoon");
            }
            ImGui.Checkbox("顯示任務目標指示器", ref C.MainConfig.QTIQuestEnabled);
            ImGui.ColorEdit4("任務目標指示器顏色", ref C.MainConfig.QTIQuestColor, ImGuiColorEditFlags.NoInputs);
            ImGui.Checkbox("任務目標指示器連線", ref C.MainConfig.QTIQuestTether);
            ImGuiEx.SetNextItemWidthScaled(60f);
            ImGui.DragFloat("任務目標指示器粗細", ref C.MainConfig.QTIQuestThickness, 0.02f, 1f, 10f);
            ImGui.Separator();
            ImGui.Checkbox("啟用事件物件搜尋器", ref C.EObjFinder);
            ImGui.Checkbox("啟用事件 NPC 搜尋器", ref C.ENpcFinder);
            ImGuiEx.SetNextItemWidthScaled(150f);
            ImGuiEx.EnumCombo("僅在按住按鍵時顯示", ref C.FinderKey);
        }
    }
}
