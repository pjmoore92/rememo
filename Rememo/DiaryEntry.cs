using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

[DataContract]
public class DiaryEntry
{
    private DateTime date;
    private TimeOfDay time;
    private DateTime timeSelected;
    private String text;
    private bool underline;
    private bool asterisk;
    private bool circle;
    private Reminder reminder;

    #region Constructors
    public DiaryEntry()
    {
        date = DateTime.Now;
        time = TimeOfDay.None;
        timeSelected = DateTime.Now;
        text = "";
        underline = false;
        asterisk = false;
        circle = false;
        reminder = null;
    }

    public DiaryEntry(DateTime date, TimeOfDay time, DateTime selected, String text, bool underline, bool asterisk, bool circle, Reminder reminder)
    {
        Date = date;
        this.time = time;
        this.timeSelected = selected;
        Text = text;
        Underline = underline;
        Asterisk = asterisk;
        Circle = circle;
        this.reminder = reminder;
        reminder.Entry = this;
    }
    #endregion

    #region Fields
    [DataMember]
    public DateTime Date
    {
        get { return date; }
        set { date = value; }
    }

    [DataMember]
    public TimeOfDay Time
    {
        get { return time; }
        set { time = value; }
    }

    [DataMember]
    public DateTime TimeSelected
    {
        get { return timeSelected; }
        set { timeSelected = value; }
    }

    [DataMember]
    public String Text
    {
        get { return text; }
        set { text = value; }
    }

    [DataMember]
    public bool Underline
    {
        get { return underline; }
        set { underline = value; }
    }

    [DataMember]
    public bool Asterisk
    {
        get { return asterisk; }
        set { asterisk = value; }
    }

    [DataMember]
    public bool Circle
    {
        get { return circle; }
        set { circle = value; }
    }

    [DataMember]
    public Reminder Reminder
    {
        get { return reminder; }
        set { reminder = value; }
    }
    #endregion

    #region Rendering
    public void Render(Canvas canvas, double x, double y, double width, double height, bool clear)
    {
        if (clear)
           canvas.Children.Clear();

        if (text.Length == 0)
            return;

        String renderText = text + (asterisk ? "*" : "");

        bool done = false;

        if (reminder != null)
            done = reminder.Done;
       

        /* Render text */
        TextBlock tb = new TextBlock();
        tb.Text = renderText;
        tb.HorizontalAlignment = HorizontalAlignment.Center;
        tb.FontSize = GetFontSize(tb, width, height);
        tb.Tag = "Reminder";

        // If the reminder has been dismissed through "Done It", strike it out
       /* if (done)
        {
            TextDecoration td = new TextDecoration(TextDecorationLocation.Strikethrough, new Pen(Brushes.Black, 1.0), 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended);
            tb.TextDecorations.Add(td);
        }*/

        Canvas.SetLeft(tb, x);
        Canvas.SetTop(tb, y);

        FormattedText ft = new FormattedText(tb.Text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
            new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch), tb.FontSize, tb.Foreground);

        
        canvas.Children.Add(tb);
        /* Draw underline */
        if (underline)
        {
            Line l = new Line();
            l.X1 = 0;
            l.Y1 = ft.Baseline + 5;
            l.X2 = ft.WidthIncludingTrailingWhitespace;
            l.Y2 = ft.Baseline + 5;
            l.StrokeThickness = 1;
            l.Stroke = Brushes.Black;
            l.Tag = "Reminder";

            Canvas.SetLeft(l, x);
            Canvas.SetTop(l, y);

            canvas.Children.Add(l);
        }

        /* Draw circle */
        if (circle)
        {
            Border b = new Border();
            b.CornerRadius = new CornerRadius(20);
            b.Width = ft.Width;
            b.Height = ft.Height;
            b.BorderBrush = Brushes.Black;
            b.BorderThickness = new Thickness(0.5);
            b.Tag = "Reminder";

            Canvas.SetLeft(b, x);
            Canvas.SetTop(b, y);

            canvas.Children.Add(b);
        }
    }
    #endregion

    #region Utility
    /// <summary>
    /// Calculates the optimal font size for a given area by calculating
    /// how much area required to render the text, and then increases
    /// by 2pt at a time until it no longer fits in the area.
    /// </summary>
    /// 
    /// <param name="textBlock">TextBlock whose text is being rendered.</param>
    /// <param name="width">Width of the widget which the textblock should fit into.</param>
    /// <param name="height">Height of the widget which the textblock should fit into.</param>
    /// 
    /// <returns>Near-optimal font size to ensure best use of available space.</returns>
    private static double GetFontSize(TextBlock textBlock, double width, double height)
    {
        if (textBlock == null || textBlock.Text.Length == 0)
            return 0;

        double fontSize = 12;

        FormattedText ft = new FormattedText(
            textBlock.Text, CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight,
            new Typeface(textBlock.FontFamily, textBlock.FontStyle,
                textBlock.FontWeight, textBlock.FontStretch),
                fontSize, textBlock.Foreground);

        double area = ft.Height * ft.Width;
        double tbArea = width * height;

        /* While area required to format text exceeds area available. */
        // Note: I don't know why, but 0.7 seems to make things work.
        // Otherwise text overflows and gets a bit unpleasant.
        // If you find a better way to do this, let me know and I'll
        // buy you a drink.
        while (area < 0.7 * tbArea && ft.Height < height && ft.Width < width)
        {
            fontSize += 2;

            ft = new FormattedText(
                textBlock.Text, CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(textBlock.FontFamily, textBlock.FontStyle,
                    textBlock.FontWeight, textBlock.FontStretch),
                    fontSize, textBlock.Foreground);

            area = ft.Height * ft.Width;
        }

        fontSize -= 2; // Go back to the last size that fit

        return fontSize;
    }
    #endregion
}