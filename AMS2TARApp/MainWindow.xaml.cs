using System.IO;
using System.Windows;
using System.Xml.Linq;
using MaterialDesignThemes.Wpf;

namespace AMS2ToApexRivals;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private static readonly string OutputDir =  Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
        "ApexRivals", 
        "Rivals", 
        "Generated");
    
    private string _xmlFilePath = string.Empty;
    
    public SnackbarMessageQueue SnackbarMessageQueue { get; }
    
    public MainWindow()
    {
        InitializeComponent();
        
        SnackbarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
        DataContext = this;
    }
    
    private void Border_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                DropText.Text = "Release to drop the file";
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Border_DragLeave(object sender, DragEventArgs e)
        {
            DropText.Text = "Drag and drop your XML file here";
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            DropText.Text = "Drag and drop your XML file here";

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            
            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);

            if (files?.Length > 0 && Path.GetExtension(files[0]).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                _xmlFilePath = files[0];
                PreviewFiles();
                ConvertButton.Visibility = Visibility.Visible;
            }
            else
            {
                SnackbarMessageQueue.Enqueue("Please drop a valid XML file.");
            }
        }

        private void PreviewFiles()
        {
            try
            {
                var root = XElement.Load(_xmlFilePath);

                PreviewListBox.Items.Clear();

                foreach (var driver in root.Elements("driver"))
                {
                    var name = driver.Element("name")?.Value ?? "Error";
                    var outputPath = Path.Combine(OutputDir, name, $"{name}.json");
                    PreviewListBox.Items.Add(outputPath);
                }

                PreviewCard.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                SnackbarMessageQueue.Enqueue($"An error occurred while previewing the files:\n{ex.Message}");
            }
        }

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new Converter(_xmlFilePath, OutputDir).Convert();

                SnackbarMessageQueue.Enqueue("Conversion completed successfully!");
            }
            catch (Exception ex)
            {
                SnackbarMessageQueue.Enqueue($"Error: {ex.Message}");
            }
        }
}