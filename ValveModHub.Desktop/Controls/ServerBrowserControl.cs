using ValveModHub.Common.Model;
using ValveModHub.Common.Utils;
using ValveModHub.Desktop.Forms;
using ValveModHub.Desktop.Services;

namespace ValveModHub.Desktop.Controls;

public partial class ServerBrowserControl : UserControl
{
    private readonly ListView _serverList;
    private readonly ComboBox _games;
    private readonly TextBox _serverFilterBox;
    private readonly CheckBox _checkNoFull;
    private readonly CheckBox _checkNoEmpty;
    private readonly Label _totalServersLabel;
    private readonly System.Windows.Forms.Timer _refreshTimer;

    public ServerBrowserControl()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    public ServerBrowserControl(Control parent) : this()
    {
        Parent = parent;
        Dock = DockStyle.Fill;

        var grid = new TableLayoutPanel();
        grid.Parent = this;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        _serverList = new ListView();
        _serverList.Parent = grid;
        _serverList.Dock = DockStyle.Fill;
        _serverList.BorderStyle = BorderStyle.FixedSingle;
        _serverList.View = View.Details;
        _serverList.FullRowSelect = true;
        _serverList.HoverSelection = _serverList.MultiSelect = false;

        _serverList.Columns.Add("Name");
        _serverList.Columns.Add("Players");
        _serverList.Columns.Add("Map");

        _serverList.DoubleClick += (s1, e1) => SteamBrowserProtocolService.ConnectToServer(GetActiveServerItem());
        _serverList.ContextMenuStrip = CreateContextMenuStrip();

        _games = new ComboBox();
        _games.Parent = grid;
        _games.Dock = DockStyle.Fill;
        _games.DropDownStyle = ComboBoxStyle.DropDownList;
        _games.SelectedIndexChanged += async (s1, e1) => await RefreshServerList();

        foreach (var game in GameList.Games)
            _games.Items.Add(game.Name);

        _games.SelectedIndex = 0;

        var filterLayout = new FlowLayoutPanel();
        filterLayout.Parent = grid;
        filterLayout.Dock = DockStyle.Fill;
        filterLayout.FlowDirection = FlowDirection.RightToLeft;

        _serverFilterBox = new TextBox();
        _serverFilterBox.Parent = filterLayout;
        _serverFilterBox.Width = 150;
        _serverFilterBox.TextChanged += async (s1, e1) => await RefreshServerList();

        var serverFilterLabel = new Label();
        serverFilterLabel.Parent = filterLayout;
        serverFilterLabel.Text = "Filter:";
        serverFilterLabel.AutoSize = false;
        serverFilterLabel.TextAlign = ContentAlignment.MiddleRight;
        serverFilterLabel.Width = 45;

        _checkNoEmpty = new CheckBox();
        _checkNoEmpty.Parent = filterLayout;
        _checkNoEmpty.Text = "Hide Empty";
        _checkNoEmpty.AutoSize = false;
        _checkNoEmpty.Width = 100;
        _checkNoEmpty.CheckAlign = _checkNoEmpty.ImageAlign = _checkNoEmpty.TextAlign = ContentAlignment.MiddleRight;
        _checkNoEmpty.CheckStateChanged += async (s1, e1) => await RefreshServerList();

        _checkNoFull = new CheckBox();
        _checkNoFull.Parent = filterLayout;
        _checkNoFull.Text = "Hide Full";
        _checkNoFull.AutoSize = false;
        _checkNoFull.Width = 80;
        _checkNoFull.CheckAlign = _checkNoFull.ImageAlign = _checkNoFull.TextAlign = ContentAlignment.MiddleRight;
        _checkNoFull.CheckStateChanged += async (s1, e1) => await RefreshServerList();

        _totalServersLabel = new Label();
        _totalServersLabel.Parent = filterLayout;
        _totalServersLabel.Text = "";
        _totalServersLabel.AutoSize = false;
        _totalServersLabel.TextAlign = ContentAlignment.MiddleRight;
        _totalServersLabel.Width = 300;

        grid.SetCellPosition(_games, new TableLayoutPanelCellPosition(0, 0));
        grid.SetCellPosition(_serverList, new TableLayoutPanelCellPosition(0, 1));
        grid.SetCellPosition(filterLayout, new TableLayoutPanelCellPosition(0, 2));

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
        grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 40.0f));

        _refreshTimer = new System.Windows.Forms.Timer();
        _refreshTimer.Interval = 150;
        _refreshTimer.Enabled = false;
        _refreshTimer.Tick += async (s1, e1) =>
        {
            await RefreshServerList();
            _refreshTimer.Stop();
        };

        OnUpdate();
    }

    public void OnUpdate()
    {
        UpdateListColumns();
    }

    private ContextMenuStrip CreateContextMenuStrip()
    {
        var strip = new ContextMenuStrip();
        ToolStripItem stripItem = null;

        stripItem = strip.Items.Add("Connect");
        stripItem.Click += (s1, e1) =>
        {
            SteamBrowserProtocolService.ConnectToServer(GetActiveServerItem());
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

    private void UpdateListColumns()
    {
        int size = _serverList.ClientSize.Width;
        foreach (ColumnHeader h in _serverList.Columns)
            h.Width = (size / _serverList.Columns.Count);
    }

    private async Task RefreshServerList()
    {
        var index = _games.SelectedIndex;

        if (GameList.Games.Count == 0 || index < 0 || index >= GameList.Games.Count)
            return;

        _serverList.SuspendLayout();
        _serverList.Items.Clear();

        var game = GameList.Games[index];
        var items = await GameServerApiService.GetServers(game);
        var filter = _serverFilterBox.Text;
        var players = 0;

        foreach (var item in items)
        {
            if (_checkNoEmpty.Checked && item.CurrentPlayers <= 0)
                continue; // hide empty

            if (_checkNoFull.Checked && item.CurrentPlayers >= item.MaxPlayers)
                continue; // hide full

            if ((filter.Length > 0) &&
                !((item.Map?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false) || (item.Name?.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false)))
                continue; // filter based on map / server name

            players += (item.CurrentPlayers ?? 0);
            _serverList.Items.Add(new ListViewItem([item.Name, $"{item.CurrentPlayers} / {item.MaxPlayers}", item.Map])
            {
                Tag = item,
            });
        }

        _totalServersLabel.Text = $"{_serverList.Items.Count} servers found, with {players} active players";

        _serverList.ResumeLayout();
        OnUpdate();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F5)
        {
            _refreshTimer.Start();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
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