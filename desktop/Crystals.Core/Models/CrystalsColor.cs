using System.Text.Json.Serialization;
using System.Windows.Media;
using Wacton.Unicolour;

namespace Crystals.Core.Models;

public readonly struct CrystalsColor : IEquatable<CrystalsColor>
{
    public float H { get; }
    public float S { get; }
    public float V { get; }

    private readonly Color? _cachedRgb = null;

    private readonly float _vibrance;
    
    [JsonConstructor]
    public CrystalsColor(float h, float s, float v)
    {
        H = Math.Clamp(h, 0f, 360f);
        S = Math.Clamp(s, 0f, 1f);
        V = Math.Clamp(v, 0f, 1f);

        _vibrance = S * V;
    }

    public CrystalsColor(byte r, byte g, byte b)
    {
        var rgbColor = Color.FromArgb(255, r, g, b);
        _cachedRgb = rgbColor;

        var uniColor = new Unicolour(ColourSpace.Rgb255, r, g, b).Hsb;

        H = (float)uniColor.H;
        S = (float)uniColor.S;
        V = (float)uniColor.B;
        _vibrance = S * V;
    }

    public Color ToRgb()
    {
        if (_cachedRgb.HasValue)
        {
            return _cachedRgb.Value;
        }

        double sector = H / 60.0;
        int sectorIndex = (int)Math.Floor(sector);
        double sectorFraction = sector - sectorIndex;

        double p = V * (1.0 - S);
        double q = V * (1.0 - (S * sectorFraction));
        double t = V * (1.0 - (S * (1.0 - sectorFraction)));

        double r = 0, g = 0, b = 0;

        switch (sectorIndex % 6)
        {
            case 0:
                r = V;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = V;
                b = p;
                break;
            case 2:
                r = p;
                g = V;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = V;
                break;
            case 4:
                r = t;
                g = p;
                b = V;
                break;
            case 5:
                r = V;
                g = p;
                b = q;
                break;
        }

        var computedColor = Color.FromArgb(
            255,
            (byte)Math.Round(r * 255),
            (byte)Math.Round(g * 255),
            (byte)Math.Round(b * 255)
        );

        return computedColor;
    }

    public bool IsVibrant()
    {
        const float vibranceMonochromeThreshold = 0.15f;
        return _vibrance is > vibranceMonochromeThreshold and < 1f - vibranceMonochromeThreshold;
    }

    public CrystalsColor WithH(float h) => new(h, S, V);
    public CrystalsColor WithS(float s) => new(H, s, V);
    public CrystalsColor WithV(float v) => new(H, S, v);

    public override string ToString()
    {
        var rgb = ToRgb();
        return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}mHSV({H:F0}°, {S * 100:F0}%, {V * 100:F0}%)\u001b[0m";
    }

    public string ToStringRGB()
    {
        var rgb = ToRgb();
        return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}mRGB({rgb.R}, {rgb.G}, {rgb.B})\u001b[0m";
    }

    public bool Equals(CrystalsColor other)
    {
        return Math.Abs(H - other.H) < 0.01f &&
               Math.Abs(S - other.S) < 0.01f &&
               Math.Abs(V - other.V) < 0.01f;
    }

    public override bool Equals(object? obj) => obj is CrystalsColor other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(H, S, V);

    public static bool operator ==(CrystalsColor left, CrystalsColor right) => left.Equals(right);
    public static bool operator !=(CrystalsColor left, CrystalsColor right) => !left.Equals(right);

    public static CrystalsColor White => new(0f, 0f, 1f);
}