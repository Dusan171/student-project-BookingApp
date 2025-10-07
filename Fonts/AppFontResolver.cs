using System;
using System.IO;
using PdfSharp.Fonts;

public sealed class AppFontResolver : IFontResolver
{
    private readonly byte[] _regular;
    private readonly byte[] _bold;

    public AppFontResolver()
    {
        // Putanja do Resources/Fonts foldera u bin/
        string fontsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fonts");

        string regularPath = Path.Combine(fontsDir, "arial.ttf");
        string boldPath = Path.Combine(fontsDir, "arialbd.ttf");

        if (!File.Exists(regularPath))
            throw new FileNotFoundException("Nedostaje arial.ttf u Resources/Fonts (Build Action=Content, Copy if newer)");

        _regular = File.ReadAllBytes(regularPath);
        _bold = File.Exists(boldPath) ? File.ReadAllBytes(boldPath) : _regular;
    }

    public string DefaultFontName => "App Arial";

    public byte[] GetFont(string faceName) => faceName switch
    {
        "AppArial#Regular" => _regular,
        "AppArial#Bold" => _bold,
        _ => _regular
    };

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Prepoznaj imena fontova koja možeš koristiti u XFont
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase) ||
            familyName.Equals("Helvetica", StringComparison.OrdinalIgnoreCase) ||
            familyName.Equals("App Arial", StringComparison.OrdinalIgnoreCase))
        {
            if (isBold) return new FontResolverInfo("AppArial#Bold");
            return new FontResolverInfo("AppArial#Regular");
        }

        // Fallback: koristi regular
        return new FontResolverInfo("AppArial#Regular");
    }
}
