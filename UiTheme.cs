using System;
using System.Drawing;
using System.Windows.Forms;

public static class UiTheme
{
    private const float BaseFontSize = 11.5f;
    private const float InputFontSize = 13f;
    private const float ButtonFontSize = 14.5f;
    private const float TitleFontSize = 16f;

    private static readonly Color PrimaryButtonBack = Color.FromArgb(176, 229, 176);
    private static readonly Color SecondaryButtonBack = Color.FromArgb(220, 239, 220);
    private static readonly Color DangerButtonBack = Color.FromArgb(229, 81, 81);
    private static readonly Color FormBack = Color.FromArgb(245, 247, 250);

    public static float GetAdaptiveScale(Form form, Size baseline)
    {
        if (form == null)
            return 1f;

        var screen = Screen.FromControl(form).WorkingArea;
        var screenScale = Math.Min(screen.Width / 1366f, screen.Height / 768f);
        var dpiScale = 1f;
        try
        {
            if (form.DeviceDpi > 0)
                dpiScale = form.DeviceDpi / 96f;
        }
        catch
        {
            dpiScale = 1f;
        }

        var baselineWidth = Math.Max(1, baseline.Width);
        var baselineHeight = Math.Max(1, baseline.Height);
        var currentWidth = Math.Max(form.ClientSize.Width, baselineWidth);
        var currentHeight = Math.Max(form.ClientSize.Height, baselineHeight);
        var formScale = Math.Min(currentWidth / (float)baselineWidth, currentHeight / (float)baselineHeight);

        var scale = Math.Min(screenScale, formScale) / Math.Max(1f, dpiScale);
        if (scale < 1f) scale = 1f;
        if (scale > 1.55f) scale = 1.55f;
        return scale;
    }

    public static int ScalePx(int value, float scale)
    {
        return Math.Max(1, (int)Math.Round(value * Math.Max(1f, scale)));
    }

    public static Font CreateFont(float baseSize, float scale, FontStyle style = FontStyle.Regular)
    {
        return new Font("Segoe UI", Math.Max(8.5f, baseSize * Math.Max(1f, scale)), style, GraphicsUnit.Point);
    }

    public static void ApplyBase(Form form, float scale = 1f)
    {
        if (form == null)
            return;

        form.Font = CreateFont(BaseFontSize, scale, FontStyle.Regular);
        form.BackColor = FormBack;
    }

    public static void StylePrimaryButton(Button button, float scale = 1f)
    {
        StyleButton(button, PrimaryButtonBack, Color.Black, scale);
    }

    public static void StyleSecondaryButton(Button button, float scale = 1f)
    {
        StyleButton(button, SecondaryButtonBack, Color.Black, scale);
    }

    public static void StyleDangerButton(Button button, float scale = 1f)
    {
        StyleButton(button, DangerButtonBack, Color.White, scale);
    }

    public static void StyleInput(Control control, float scale = 1f)
    {
        if (control == null)
            return;

        control.Font = CreateFont(InputFontSize, scale, FontStyle.Regular);
    }

    public static void StyleTitleLabel(Label label, float scale = 1f)
    {
        if (label == null)
            return;

        label.Font = CreateFont(TitleFontSize, scale, FontStyle.Bold);
    }

    public static void ApplyAdaptiveStartupSize(Form form, float widthRatio = 0.84f, float heightRatio = 0.84f)
    {
        if (form == null)
            return;

        var wa = Screen.FromControl(form).WorkingArea;
        var width = Math.Max(form.MinimumSize.Width, (int)(wa.Width * widthRatio));
        var height = Math.Max(form.MinimumSize.Height, (int)(wa.Height * heightRatio));
        form.Size = new Size(Math.Min(width, wa.Width - 20), Math.Min(height, wa.Height - 20));
    }

    public static void EnsureFormFitsOnScreen(Form form, bool center)
    {
        if (form == null || form.WindowState != FormWindowState.Normal)
            return;

        var wa = Screen.FromRectangle(form.Bounds).WorkingArea;
        var width = Math.Min(form.Width, wa.Width);
        var height = Math.Min(form.Height, wa.Height);
        var minWidth = form.MinimumSize.Width > 0 ? form.MinimumSize.Width : width;
        var minHeight = form.MinimumSize.Height > 0 ? form.MinimumSize.Height : height;
        width = Math.Max(Math.Min(minWidth, wa.Width), width);
        height = Math.Max(Math.Min(minHeight, wa.Height), height);

        if (form.Width != width || form.Height != height)
            form.Size = new Size(width, height);

        var outOfBounds =
            form.Left < wa.Left ||
            form.Top < wa.Top ||
            form.Right > wa.Right ||
            form.Bottom > wa.Bottom;

        var x = form.Left;
        var y = form.Top;

        if (center || outOfBounds)
        {
            x = wa.Left + (wa.Width - form.Width) / 2;
            y = wa.Top + (wa.Height - form.Height) / 2;
        }

        x = Math.Max(wa.Left, Math.Min(x, wa.Right - form.Width));
        y = Math.Max(wa.Top, Math.Min(y, wa.Bottom - form.Height));
        form.Location = new Point(x, y);
    }

    private static void StyleButton(Button button, Color backColor, Color foreColor, float scale)
    {
        if (button == null)
            return;

        button.FlatStyle = FlatStyle.Flat;
        button.Font = CreateFont(ButtonFontSize, scale, FontStyle.Bold);
        button.BackColor = backColor;
        button.ForeColor = foreColor;
        button.FlatAppearance.BorderColor = Color.FromArgb(88, 98, 108);
        button.FlatAppearance.BorderSize = 1;
    }
}
