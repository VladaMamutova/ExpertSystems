using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GraphAndOrTraversal.Helpers;
using GraphAndOrTraversal.Logic;
using GraphAndOrTraversal.Model;
using Microsoft.Win32;

namespace GraphAndOrTraversal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Graph _graph;
        private IGraphTraverser _graphTraverser;
        private int[] _sourceVertices;
        private int _targetVertex;

        public MainWindow()
        {
            InitializeComponent();
            _sourceVertices = new int[0];
        }

        private async void DepthFirstSearch_OnClick(object sender,
            RoutedEventArgs e)
        {
            if (ValidateData())
            {
                try
                {
                    _graphTraverser = new DepthFirstSearch(_graph,
                        _sourceVertices, _targetVertex);
                    
                    IEnumerable<GraphItem> traversal = _graphTraverser.Traverse();

                    if (traversal == null)
                    {
                        MessageBox.Show(
                            "Решение не найдено. Измените исходные данные " +
                            "или целевую и повторите попытку.",
                            "Поиск в глубину", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        ResultGraph.Source = null;
                        return;
                    }

                    var graphEdges =
                        traversal.Select(item => (Edge) item).ToList();

                    await DisplayGraph(ResultGraph, graphEdges);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message,
                        "Поиск в глубину", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private async void LoadGraph_OnClick(object sender, RoutedEventArgs e)
        {
            LoadGraph(GetOpenFileName());
            await DisplayGraph(SourceGraph, _graph.Edges);
        }
        private void SourceGraph_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SaveGraph(SourceGraph);
        }
        private void ResultGraph_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SaveGraph(ResultGraph);
        }

        private void LoadGraph(string filename)
        {
            if (filename == null)
            {
                return;
            }

            try
            {
                _graph = Graph.FromFile(filename);
                if (_graph.Vertices.Count == 0 && _graph.Edges.Count == 0)
                {
                    throw new Exception("Не удалось загрузить граф из файла. " +
                                        "Проверьте формат.");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message,
                    "Ошибка загрузки графа из файла", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task DisplayGraph(Image image, List<Edge> graphEdges)
        {
            if (graphEdges == null || graphEdges.Count == 0)
            {
                return;
            }

            BitmapImage bitmap = new BitmapImage();

            // run a method in another thread
            await GenerateImage(bitmap, graphEdges);

            // modify UI object in UI thread
            image.Source = bitmap;
        }

        private async Task GenerateImage(BitmapImage bitmap, List<Edge> graphEdges)
        {
            using (var stream = new MemoryStream())
            {
                await GraphViz.GenerateImage(graphEdges, stream);

                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }
        }

        private void SaveGraph(Image image)
        {
            var filename = GetSaveFileName(GenerateDefaultFileName());
            SaveImage((BitmapImage)image.Source, filename);
        }

        private string GetOpenFileName()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                CheckFileExists = true,
                Filter = "TXT Files (*.txt)|*.txt"
            };
            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        private string GetSaveFileName(string defaultFileName = "")
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                Filter = "PNG Files (*.png)|*.png",
                FileName = defaultFileName
            };
            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }

        void SaveImage(BitmapImage bitmap, string filename)
        {
            if (filename == null)
            {
                return;
            }

            BitmapFrame frame = BitmapFrame.Create(bitmap);
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(frame);

            using (var stream = File.Create(filename))
            {
                encoder.Save(stream);
            }
        }

        void SaveUsingEncoder(FrameworkElement visual, string filename,
            BitmapEncoder encoder)
        {
            if (filename == null)
            {
                return;
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (int) visual.ActualWidth, (int) visual.ActualHeight, 96, 96,
                PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(filename))
            {
                encoder.Save(stream);
            }
        }

        private int[] ExtractNumbers(string text)
        {
            string pattern = @"(\d+)";
            MatchCollection matches = Regex.Matches(text, pattern);
            return (from Match match in matches select int.Parse(match.Groups[1].Value)).ToArray();
        }

        private void SourceVertices_OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (SourceVertices.Text.Trim().Length > 0)
            {
                SetSourceVertices();
            }
        }

        private bool ValidateData()
        {
            return ValidateGraph() && SetSourceVertices() && SetTargetVertex();
        }
        private bool ValidateGraph()
        {
            if (_graph == null || _graph.Vertices.Count < 2 ||
                _graph.Edges.Count < 1)
            {
                MessageBox.Show("Граф должен иметь хотя бы две вершины и одно ребро.",
                    "Некорректные исходные данные", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            return true;
        }

        private void TargetVertex_OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (TargetVertex.Text.Trim().Length > 0)
            {
                SetTargetVertex();
            }
        }

        private bool SetSourceVertices()
        {
            _sourceVertices = ExtractNumbers(SourceVertices.Text);
            if (_sourceVertices.Length == 0)
            {
                MessageBox.Show("Не удалось добавить исходные вершины. " +
                                "Проверьте формат введённых данных. " + Environment.NewLine +
                                "Необходимо ввести числа, разделённые знаками пунктуации. " +
                                "Пример: \"1, 4, 7\"",
                    "Некорректные исходные данные", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return false;
            }

            return true;
        }

        private bool SetTargetVertex()
        {
            if (int.TryParse(TargetVertex.Text, out int targetLabel))
            {
                _targetVertex = targetLabel;
                return true;
            }

            MessageBox.Show("Не удалось добавить целевую вершину. " +
                            "Проверьте формат введённых данных." + Environment.NewLine +
                            "Необходимо ввести число. Пример: \"1\".",
                "Некорректные исходные данные", MessageBoxButton.OK,
                MessageBoxImage.Information);
            return false;
        }

        private string GenerateDefaultFileName()
        {
            string filename = "graph";
            if (_sourceVertices.Length > 0)
            {
                filename += " (" + string.Join(", ", _sourceVertices) +
                            $"; {_targetVertex})";
            }

            return filename;
        }
    }
}
