using ValveModHub.Common.Model;
using ValveModHub.Common.Utils;
using ValveModHub.Desktop.Forms;
using ValveModHub.Desktop.Services;

namespace ValveModHub.Desktop.Controls;

public partial class ServerBrowser : UserControl
{
    private readonly ListView _serverList;

    public ServerBrowser()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    public ServerBrowser(Control parent) : this()
    {
        var grid = new TableLayoutPanel();
        grid.Parent = parent;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        var serverList = new ListView();
        serverList.Parent = grid;
        serverList.Dock = DockStyle.Fill;
        serverList.BorderStyle = BorderStyle.FixedSingle;
        serverList.View = View.Details;
        serverList.FullRowSelect = true;
        serverList.HoverSelection = serverList.MultiSelect = false;

        serverList.Columns.Add("Name");
        serverList.Columns.Add("Players");
        serverList.Columns.Add("Map");

        serverList.DoubleClick += (s1, e1) => GameServerApiService.ConnectToServer(GetActiveServerItem());
        serverList.ContextMenuStrip = CreateContextMenuStrip();

        var games = new ComboBox();
        games.Parent = grid;
        games.Dock = DockStyle.Fill;
        games.DropDownStyle = ComboBoxStyle.DropDownList;
        games.SelectedIndexChanged += async (s1, e1) => await RefreshServerList(games.SelectedIndex, serverList);

        foreach (var game in GameList.Games)
            games.Items.Add(game.Name);

        games.SelectedIndex = 0;

        grid.SetCellPosition(games, new TableLayoutPanelCellPosition(0, 0));
        grid.SetCellPosition(serverList, new TableLayoutPanelCellPosition(0, 1));

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));

        _serverList = serverList;
        UpdateListColumns(serverList);
    }

    public void OnUpdate()
    {
        UpdateListColumns(_serverList);
    }

    private ContextMenuStrip CreateContextMenuStrip()
    {
        var strip = new ContextMenuStrip();
        ToolStripItem stripItem = null;

        stripItem = strip.Items.Add("Connect");
        stripItem.Click += (s1, e1) =>
        {
            GameServerApiService.ConnectToServer(GetActiveServerItem());
        };

        stripItem = strip.Items.Add("Show Players");
        stripItem.Click += (s1, e1) =>
        {
            PlayerListForm.ShowPlayersForServer(_serverList, GetActiveServerItem());
        };

        return strip;
    }

    private GameServerItem? GetActiveServerItem()
    {
        if (_serverList.SelectedItems.Count == 0)
            return null;

        return _serverList.SelectedItems[0].Tag as GameServerItem;
    }

    private static void UpdateListColumns(ListView view)
    {
        if (view is null)
            return;

        int size = view.ClientSize.Width;
        foreach (ColumnHeader h in view.Columns)
            h.Width = (size / view.Columns.Count);
    }

    private static async Task RefreshServerList(int index, ListView serverList)
    {
        if (GameList.Games.Count == 0 || index < 0 || index >= GameList.Games.Count)
            return;

        serverList.SuspendLayout();
        serverList.Items.Clear();

        var game = GameList.Games[index];
        var items = await Query(game);

        foreach (var item in items)
        {
            serverList.Items.Add(new ListViewItem([item.Name, $"{item.CurrentPlayers} / {item.MaxPlayers}", item.Map])
            {
                Tag = item,
            });
        }

        serverList.ResumeLayout();
        UpdateListColumns(serverList);
    }

    private static async Task<List<GameServerItem>> Query(Game game)
    {
        return await GameServerApiService.GetServers(game);
    }
}