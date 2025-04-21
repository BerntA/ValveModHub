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
}