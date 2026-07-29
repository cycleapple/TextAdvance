namespace TextAdvance.Gui;

internal static class TabChars
{
    private static string Name = "";
    internal static void Draw()
    {

        ImGui.Text("登入下列角色時自動啟用插件：");
        string dele = null;
        foreach (var s in C.AutoEnableNames)
        {
            ImGui.Text(s);
            ImGui.SameLine();
            if (ImGui.SmallButton("刪除##" + s))
            {
                dele = s;
            }
        }
        if (ImGui.Button("新增目前角色") && Svc.ClientState.LocalPlayer != null)
        {
            C.AutoEnableNames.Add(Svc.ClientState.LocalPlayer.Name.ToString() + "@" + Svc.ClientState.LocalPlayer.HomeWorld.Value.Name.ToString());
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(200f);
        ImGui.InputTextWithHint("##charname", "角色名稱", ref Name, 50);
        ImGui.SameLine();
        if (ImGui.Button("新增"))
        {
            C.AutoEnableNames.Add(Name);
            Name = "";
        }
        if (dele != null)
        {
            C.AutoEnableNames.Remove(dele);
        }
    }
}
