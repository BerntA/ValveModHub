using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using ValveModHub.Common.Model;
using ValveModHub.Common.Utils;
using ValveModHub.Desktop.Services;
using static System.Windows.Forms.ListView;

namespace ValveModHub.Desktop.Forms;

public partial class MainForm : Form
{
    private static IMemoryCache Cache { get; set; } = new MemoryCache(new MemoryCacheOptions());

    public MainForm()
    {
        InitializeComponent();

        DoubleBuffered = true;
        Text = "Valve Mod Hub";

        var tableLayout = new TableLayoutPanel();
        tableLayout.Parent = this;
        tableLayout.Dock = DockStyle.Fill;
        tableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

        var serverList = new ListView();
        serverList.Parent = tableLayout;
        serverList.Dock = DockStyle.Fill;
        serverList.BorderStyle = BorderStyle.FixedSingle;
        serverList.View = View.Details;
        serverList.FullRowSelect = true;
        serverList.HoverSelection = serverList.MultiSelect = false;

        serverList.Columns.Add("Name");
        serverList.Columns.Add("Players");
        serverList.Columns.Add("Map");

        serverList.DoubleClick += (s1, e1) => ConnectToServer(serverList.SelectedItems);

        var games = new ComboBox();
        games.Parent = tableLayout;
        games.Dock = DockStyle.Fill;
        games.DropDownStyle = ComboBoxStyle.DropDownList;
        games.SelectedIndexChanged += async (s1, e1) => await RefreshServerList(games.SelectedIndex, serverList);

        foreach (var game in GameList.Games)
            games.Items.Add(game.Name);

        games.SelectedIndex = 0;

        tableLayout.SetCellPosition(games, new TableLayoutPanelCellPosition(0, 0));
        tableLayout.SetCellPosition(serverList, new TableLayoutPanelCellPosition(0, 1));

        tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));

        UpdateListColumns(serverList);
        Resize += (s1, e1) => UpdateListColumns(serverList);
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

        serverList.Items.Clear();
        var game = GameList.Games[index];
        var items = await Query(game);

        foreach (var item in items)
        {
            serverList.Items.Add(new ListViewItem([item.Name, $"{item.CurrentPlayers} / {item.MaxPlayers}", item.Map])
            {
                Tag = item
            });
        }
    }

    private static async Task<List<GameServerItem>> Query(Game game, int timeoutServers = 500, int timeoutMasterServer = 15000)
    {
        if (game is null || game.MasterServer.HasValue == false)
        {
            MessageBox.Show("Invalid game - null or missing master server info!");
            return [];
        }

        return await Cache.GetOrCreateAsync<List<GameServerItem>>($"game-servers-{game.Name.ToLower()}",
            async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            var servers = await GameServerApiService.GetServers(game);
            return [.. servers.OrderByDescending(o => o.CurrentPlayers)];
        });
    }

    private static void ConnectToServer(SelectedListViewItemCollection selectedItems)
    {
        if (selectedItems.Count == 0)
            return;

        var server = selectedItems[0].Tag as GameServerItem;
        if (server is null)
            return;

        Process.Start("explorer.exe", $"steam://connect/{server.Address}");
    }
}
