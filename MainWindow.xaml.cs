using Dynastream.Fit;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Fit2Gpx;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void DropZone_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = HasFitFiles(e) ? DragDropEffects.Copy : DragDropEffects.None;
        DropZone.Background = new SolidColorBrush(Color.FromRgb(210, 231, 222));
        e.Handled = true;
    }

    private void DropZone_DragLeave(object sender, DragEventArgs e)
    {
        DropZone.Background = new SolidColorBrush(Color.FromRgb(231, 240, 236));
    }

    private async void DropZone_Drop(object sender, DragEventArgs e)
    {
        DropZone.Background = new SolidColorBrush(Color.FromRgb(231, 240, 236));
        if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
        {
            await ConvertFilesAsync(files);
        }
    }

    private async void SelectFiles_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Garmin FIT-filer (*.fit)|*.fit",
            Multiselect = true,
            Title = "Vælg FIT-filer"
        };

        if (dialog.ShowDialog(this) == true)
        {
            await ConvertFilesAsync(dialog.FileNames);
        }
    }

    private async Task ConvertFilesAsync(IEnumerable<string> files)
    {
        var fitFiles = files.Where(file => string.Equals(Path.GetExtension(file), ".fit", StringComparison.OrdinalIgnoreCase)).ToArray();
        if (fitFiles.Length == 0)
        {
            StatusText.Text = "Ingen .fit-filer fundet.";
            return;
        }

        ResultList.Items.Clear();
        StatusText.Text = $"Konverterer {fitFiles.Length} fil(er)...";

        var results = await Task.Run(() => fitFiles.Select(ConvertFile).ToArray());
        foreach (var result in results)
        {
            ResultList.Items.Add(result);
        }

        StatusText.Text = results.All(result => result.StartsWith("OK"))
            ? $"Færdig: {results.Length} fil(er) konverteret."
            : "Færdig med fejl. Se listen nedenfor.";
    }

    private static bool HasFitFiles(DragEventArgs e) =>
        e.Data.GetData(DataFormats.FileDrop) is string[] files && files.Any(file => string.Equals(Path.GetExtension(file), ".fit", StringComparison.OrdinalIgnoreCase));

    private static string ConvertFile(string inputPath)
    {
        try
        {
            var records = new List<RecordMesg>();
            var decoder = new Decode();
            var broadcaster = new MesgBroadcaster();
            decoder.MesgEvent += broadcaster.OnMesg;
            decoder.MesgDefinitionEvent += broadcaster.OnMesgDefinition;
            broadcaster.RecordMesgEvent += (_, eventArgs) =>
            {
                if (eventArgs.mesg is RecordMesg record)
                {
                    records.Add(record);
                }
            };

            using (var input = System.IO.File.OpenRead(inputPath))
            {
                decoder.Read(input);
            }

            var outputPath = Path.ChangeExtension(inputPath, ".gpx");
            var pointCount = WriteGpx(outputPath, records);
            return pointCount == 0
                ? $"FEJL  {Path.GetFileName(inputPath)} - ingen GPS-punkter fundet"
                : $"OK    {Path.GetFileName(inputPath)} -> {Path.GetFileName(outputPath)} ({pointCount} punkter)";
        }
        catch (Exception exception)
        {
            return $"FEJL  {Path.GetFileName(inputPath)} - {exception.Message}";
        }
    }

    private static int WriteGpx(string outputPath, IEnumerable<RecordMesg> records)
    {
        var settings = new XmlWriterSettings { Indent = true };
        var pointCount = 0;
        using var writer = XmlWriter.Create(outputPath, settings);
        writer.WriteStartDocument();
        writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
        writer.WriteAttributeString("version", "1.1");
        writer.WriteAttributeString("creator", "Fit2Gpx");
        writer.WriteStartElement("trk");
        writer.WriteElementString("name", Path.GetFileNameWithoutExtension(outputPath));
        writer.WriteStartElement("trkseg");

        foreach (var record in records)
        {
            var latitude = record.GetPositionLat();
            var longitude = record.GetPositionLong();
            var timestamp = record.GetTimestamp();
            if (!latitude.HasValue || !longitude.HasValue || timestamp is null)
            {
                continue;
            }

            writer.WriteStartElement("trkpt");
            writer.WriteAttributeString("lat", SemicirclesToDegrees(latitude.Value).ToString("F7", System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lon", SemicirclesToDegrees(longitude.Value).ToString("F7", System.Globalization.CultureInfo.InvariantCulture));
            var altitude = record.GetAltitude();
            if (altitude.HasValue)
            {
                writer.WriteElementString("ele", altitude.Value.ToString("F1", System.Globalization.CultureInfo.InvariantCulture));
            }
            writer.WriteElementString("time", timestamp.GetDateTime().ToUniversalTime().ToString("O"));
            writer.WriteEndElement();
            pointCount++;
        }

        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndElement();
        writer.WriteEndDocument();
        return pointCount;
    }

    private static double SemicirclesToDegrees(int semicircles) => semicircles * (180.0 / 2147483648.0);
}