namespace ValveModHub.Desktop.UI;

public class Texture : IDisposable
{
    public string? File { get; set; }
    public Image? Image { get; set; }

    public Texture() { }

    public Texture(string file)
    {
        if (!System.IO.File.Exists(file))
            throw new ApplicationException($"Unable to load texture: {file}");

        File = file;
        Image = Image.FromFile(file);
    }

    ~Texture()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (Image is null)
            return;

        Image.Dispose();
        Image = null;
    }
}