namespace ValveModHub.Desktop.Controls;

public partial class GameBrowser : UserControl
{
    public GameBrowser()
    {
        InitializeComponent();
        DoubleBuffered = true;
    }

    public GameBrowser(Control parent) : this()
    {
        var test = new Label();
        test.Text = "TODO";
        test.Parent = parent;
        test.Dock = DockStyle.Fill;
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