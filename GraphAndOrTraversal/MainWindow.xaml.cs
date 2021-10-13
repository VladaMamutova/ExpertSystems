using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GraphAndOrTraversal.Helpers;
using GraphAndOrTraversal.Model;

namespace GraphAndOrTraversal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Graph _graph;

        public MainWindow()
        {
            InitializeComponent();
            _graph = Graph.FromFile("graph.txt");
        }

        private async void DepthFirstSearch_OnClick(object sender,
            RoutedEventArgs e)
        {
            BitmapImage bitmap = new BitmapImage();
            // run a method in another thread
            await GenerateImage(bitmap);

            // modify UI object in UI thread
            SourceGraph.Source = bitmap;
        }

        private async Task GenerateImage(BitmapImage bitmap)
        {
            using (var stream = new MemoryStream())
            {
                await GraphViz.GenerateImage(_graph.Edges, stream);

                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                //bitmap.Freeze();
            }
        }
    }
}
