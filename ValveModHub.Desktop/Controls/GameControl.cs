using ValveModHub.Common.Model;
using ValveModHub.Desktop.Services;

namespace ValveModHub.Desktop.Controls;

public partial class GameControl : UserControl
{
    private readonly Game _game;
    private readonly Image _gameIcon;
    private bool _hover;

    public GameControl()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    public GameControl(Game game) : this()
    {
        _game = game;
        _gameIcon = Image.FromFile($"{Application.StartupPath}assets/{game.Icon}.jpg");
    }

    ~GameControl()
    {
        _gameIcon.Dispose();
    }

    protected override void OnDoubleClick(EventArgs e)
    {
        base.OnDoubleClick(e);
        SteamBrowserProtocolService.LaunchGame(_game);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        if (!Enabled)
            return;

        _hover = true;
        base.OnMouseEnter(e);

        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        if (!Enabled)
            return;

        _hover = false;
        base.OnMouseLeave(e);

        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (DesignMode || _game is null)
            return;

        e.Graphics.DrawImage(_gameIcon, new Rectangle(0, 0, Size.Height, Size.Height));
        e.Graphics.DrawString(_game.Name, SystemFonts.DefaultFont, SystemBrushes.Control, new Point(Size.Height + 1, 0));

        if (_hover)
            e.Graphics.DrawRectangle(SystemPens.Control, new Rectangle(0, 0, Size.Width, Size.Height));
    }
}