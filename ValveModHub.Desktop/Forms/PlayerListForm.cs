using ValveModHub.Common.Model;
using ValveModHub.Desktop.Services;

namespace ValveModHub.Desktop.Forms;

public partial class PlayerListForm : Form
{
    private readonly ListView _playerList;

    public PlayerListForm()
    {
        InitializeComponent();
        DoubleBuffered = true;

        var grid = new TableLayoutPanel();
        grid.Parent = this;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        var playerList = new ListView();
        playerList.Parent = grid;
        playerList.Dock = DockStyle.Fill;
        playerList.BorderStyle = BorderStyle.FixedSingle;
        playerList.View = View.Details;
        playerList.FullRowSelect = true;
        playerList.HoverSelection = playerList.MultiSelect = false;

        playerList.Columns.Add("Name");
        playerList.Columns.Add("Score");
        playerList.Columns.Add("Time");

        grid.SetCellPosition(playerList, new TableLayoutPanelCellPosition(0, 0));

        _playerList = playerList;

        _playerList.Columns[0].Width = -2;
        _playerList.Columns[1].Width = -2;
        _playerList.Columns[2].Width = -2;
    }

    public PlayerListForm(GameServerItem server, List<PlayerItem> players) : this()
    {
        Text = server.Name;

        foreach (var player in players)
        {
            _playerList.Items.Add(
                new ListViewItem([player.Name, player.Score.ToString(), player.Duration])
            );
        }
    }

    public static async Task ShowPlayersForServer(Control caller, GameServerItem? server)
    {
        if (server is null)
        {
            MessageBox.Show("No server has been selected!", "Error");
            return;
        }

        var data = await GameServerApiService.GetPlayers(server);
        if (data.Count == 0)
            return;

        var form = new PlayerListForm(server, data);
        form.Show(caller);
    }
}