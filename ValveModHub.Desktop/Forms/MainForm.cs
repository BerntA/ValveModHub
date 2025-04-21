using ValveModHub.Desktop.Controls;

namespace ValveModHub.Desktop.Forms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        DoubleBuffered = true;

        var grid = new TableLayoutPanel();
        grid.Parent = this;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        var tabControl = new TabControl();
        tabControl.Parent = grid;
        tabControl.Dock = DockStyle.Fill;

        grid.SetCellPosition(tabControl, new TableLayoutPanelCellPosition(0, 0));

        var gamesPage = new TabPage("Games & Mods ");
        var serversPage = new TabPage("Servers");

        tabControl.TabPages.Add(gamesPage);
        tabControl.TabPages.Add(serversPage);

        var gameBrowser = new GameBrowser(gamesPage);
        var serverBrowser = new ServerBrowser(serversPage);

        Resize += (s1, e1) =>
        {
            gameBrowser.OnUpdate();
            serverBrowser.OnUpdate();
        };
    }
}