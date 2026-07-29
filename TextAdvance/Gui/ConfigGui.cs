using ECommons.Funding;
using ECommons.SimpleGui;

namespace TextAdvance.Gui;

public class ConfigGui : ConfigWindow
{
    Overlay Overlay = new();
    WaitOverlay WaitOverlay = new();
    ProgressOverlay ProgressOverlay = new();
    private ConfigGui()
    {
        EzConfigGui.Init(this);
        EzConfigGui.WindowSystem.AddWindow(Overlay);
        EzConfigGui.WindowSystem.AddWindow(WaitOverlay);
        EzConfigGui.WindowSystem.AddWindow(ProgressOverlay);
    }

    public override void Draw()
    {
        if (ImGui.BeginChild("Child", new(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - ImGui.GetFrameHeightWithSpacing())))
        {
            PatreonBanner.DrawRight();
            ImGuiEx.EzTabBar("TextAdvanceTab", PatreonBanner.Text,
                ("一般設定", TabConfig.Draw, null, true),
                ("目標指示器", TabSplatoon.Draw, null, true),
                ("自動啟用", TabChars.Draw, null, true),
                ("區域個別設定", TabTerritory.Draw, null, true),
                InternalLog.ImGuiTab(),
                ("除錯", TabDebug.Draw, ImGuiColors.DalamudGrey3, true)
                );
        }
        ImGui.EndChild();
    }

    public override void PreDraw()
    {
        base.PreDraw();
    }
}
