using ValveModHub.Common.Model;

namespace ValveModHub.Desktop.Controls;

public partial class GameListControl : UserControl
{
    private readonly List<Game> _games;

    public GameListControl()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    public GameListControl(string category, List<Game> games) : this()
    {
        _games = games;

        var grid = new TableLayoutPanel();
        grid.Parent = this;
        grid.Dock = DockStyle.Fill;
        grid.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        var row = 0;
        var size = 20;

        var labelSection = new Label();
        labelSection.Parent = grid;
        labelSection.Dock = DockStyle.Fill;
        labelSection.Text = category;

        grid.SetCellPosition(labelSection, new TableLayoutPanelCellPosition(0, 0));

        foreach (var game in _games)
        {
            var gameUiControl = new GameControl(game);

            gameUiControl.Parent = grid;
            gameUiControl.Dock = DockStyle.Fill;

            grid.SetCellPosition(gameUiControl, new TableLayoutPanelCellPosition(0, row + 1));
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, size));

            row++;
        }

        Height = (row * size);
    }
}