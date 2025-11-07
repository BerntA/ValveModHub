using ValveModHub.Common.Utils;
using ValveModHub.Desktop.Utils;

namespace ValveModHub.Desktop.Controls;

public partial class GameBrowserControl : UserControl
{
    public GameBrowserControl()
    {
        InitializeComponent();
        DoubleBuffered = true;
        ColorThemeHelper.SetSteamDefaultLayout(this);
    }

    public GameBrowserControl(Control parent) : this()
    {
        Parent = parent;
        Dock = DockStyle.Fill;

        var grid = new TableLayoutPanel();
        grid.Parent = this;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
        grid.AutoScroll = true;

        var sourceGames = new GameListControl(
            "Source",
            [.. GameList.Games.Where(g => g.MasterServer == Common.Model.A2S.MasterServer.Source)]
        );
        sourceGames.Parent = grid;
        sourceGames.Dock = DockStyle.Fill;

        var goldSourceGames = new GameListControl(
            "GoldSrc",
            [.. GameList.Games.Where(g => g.MasterServer == Common.Model.A2S.MasterServer.GoldSrc)]
        );
        goldSourceGames.Parent = grid;
        goldSourceGames.Dock = DockStyle.Fill;

        grid.SetCellPosition(sourceGames, new TableLayoutPanelCellPosition(0, 0));
        grid.SetCellPosition(goldSourceGames, new TableLayoutPanelCellPosition(0, 1));
    }

    public void OnUpdate()
    {
    }

    protected override CreateParams CreateParams // Prevent flickering, make the control more consistent...
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
            return cp;
        }
    }
}