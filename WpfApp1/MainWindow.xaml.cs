using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int rowCount = 2;
        private int columnCount = 2;
        private List<List<int>> adjacencyMatrix;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public MainWindow()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            InitializeComponent();
            PopulateMatrix();
        }
        //save file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<List<int>> matrixValues = GetMatrixValues();

            StringBuilder sb = new StringBuilder();

            foreach (List<int> row in matrixValues)
            {
                foreach (int value in row)
                {
                    sb.Append(value);
                    sb.Append(" ");
                }
                sb.AppendLine(); // Новая строка для каждой строки матрицы
            }

            string filePath = "C:\\Users\\rocks\\OneDrive\\Документы\\matrix_values.txt";

            try
            {
                File.WriteAllText(filePath, sb.ToString());
                MessageBox.Show($"Матрица сохранена в файл: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}");
            }

        }
        private List<List<int>> GetMatrixValues()
        {
            List<List<int>> matrixValues = new List<List<int>>();

            foreach (UIElement element in matrixGrid.Children)
            {
                if (element is TextBox textBox)
                {
                    if (int.TryParse(textBox.Text, out int value))
                    {
                        int row = Grid.GetRow(textBox);
                        int column = Grid.GetColumn(textBox);

                        while (matrixValues.Count <= row)
                        {
                            matrixValues.Add(new List<int>());
                        }

                        while (matrixValues[row].Count <= column)
                        {
                            matrixValues[row].Add(0);
                        }

                        matrixValues[row][column] = value;
                    }
                    else
                    {
                        // Обработка случая, когда в ячейке не число
                        throw new FormatException("Введено недопустимое значение в матрицу.");
                    }
                }
            }

            return matrixValues;
        }
        private void PopulateMatrix()
        {
            matrixGrid.Children.Clear();
            matrixGrid.RowDefinitions.Clear();
            matrixGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < rowCount; i++)
            {
                matrixGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < columnCount; j++)
            {
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Text = "0";
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j);
                    matrixGrid.Children.Add(textBox);
                }
            }
        }
       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    List<List<int>> matrixValues = ReadMatrixFromFile(openFileDialog.FileName);
                    PopulateMatrix(matrixValues);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}");
                }
            }

        }
        private List<List<int>> ReadMatrixFromFile(string filePath)
        {
            List<List<int>> matrixValues = new List<List<int>>();

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                List<int> rowValues = new List<int>();

                foreach (char c in line)
                {
                    if (int.TryParse(c.ToString(), out int value))
                    {
                        rowValues.Add(value);
                    }
                    else
                    {
                        // Обработка ситуации, когда символ не является числом
                        throw new FormatException("Файл содержит недопустимые символы.");
                    }
                }

                matrixValues.Add(rowValues);
            }

            return matrixValues;
        }
        private void PopulateMatrix(List<List<int>> matrixValues)
        {
            int rowCount = matrixValues.Count;
            int columnCount = matrixValues[0].Count;

            matrixGrid.Children.Clear();
            matrixGrid.RowDefinitions.Clear();
            matrixGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < rowCount; i++)
            {
                matrixGrid.RowDefinitions.Add(new RowDefinition());
            }

            for (int j = 0; j < columnCount; j++)
            {
                matrixGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    TextBox textBox = new TextBox();
                    textBox.Text = matrixValues[i][j].ToString();
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    Grid.SetRow(textBox, i);
                    Grid.SetColumn(textBox, j);
                    matrixGrid.Children.Add(textBox);
                }
            }
        }
        //Add row
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            rowCount++;
            PopulateMatrix();
        }
        //Add column
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            columnCount++;
            PopulateMatrix();
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        //обход в глубину
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // Преобразование матрицы в граф
            adjacencyMatrix = GetMatrixValues();

            // Выбор начальной вершины (узла) для обхода
            int startNode = 0; // Например, начнем с вершины 0

            // Список для хранения посещенных вершин
            List<bool> visited = Enumerable.Repeat(false, adjacencyMatrix.Count).ToList();

            // Список для хранения результатов обхода
            List<int> traversalResult = new List<int>();

            // Вызов рекурсивной функции для обхода в глубину
            DFS(startNode, visited, traversalResult);

            // Вывод результатов обхода в MessageBox
            MessageBox.Show("Результат обхода в глубину: " + string.Join(", ", traversalResult));
        }
    
    private void DFS(int node, List<bool> visited, List<int> traversalResult)
    {
        // Посетите текущую вершину и пометьте ее как посещенную
        visited[node] = true;

        // Добавьте вершину в список результатов обхода
        traversalResult.Add(node);

        // Получите соседей текущей вершины
        List<int> neighbors = GetNeighbors(node);

        // Рекурсивно посетите непосещенных соседей
        foreach (int neighbor in neighbors)
        {
            if (!visited[neighbor])
            {
                DFS(neighbor, visited, traversalResult);
            }
        }
    }

    private List<int> GetNeighbors(int node)
        {
            List<int> neighbors = new List<int>();

            for (int i = 0; i < adjacencyMatrix[node].Count; i++)
            {
                if (adjacencyMatrix[node][i] != 0) // Вершина связана с текущей, если значение не равно 0
                {
                    neighbors.Add(i);
                }
            }

            return neighbors;
        }
        //обход в ширину
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            // Преобразование матрицы в граф
            adjacencyMatrix = GetMatrixValues();

            // Выбор начальной вершины (узла) для обхода
            int startNode = 0; // Например, начнем с вершины 0

            // Список для хранения посещенных вершин
            List<bool> visited = Enumerable.Repeat(false, adjacencyMatrix.Count).ToList();

            // Список для хранения результатов обхода
            List<int> traversalResult = new List<int>();

            // Создание очереди для обхода в ширину
            Queue<int> queue = new Queue<int>();

            // Поместите начальную вершину в очередь и отметьте ее как посещенную
            queue.Enqueue(startNode);
            visited[startNode] = true;

            // Обход в ширину
            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();
                traversalResult.Add(currentNode);

                // Получение соседей текущей вершины
                List<int> neighbors = GetNeighbors(currentNode);

                // Добавление непосещенных соседей в очередь и пометка их как посещенные
                foreach (int neighbor in neighbors)
                {
                    if (!visited[neighbor])
                    {
                        queue.Enqueue(neighbor);
                        visited[neighbor] = true;
                    }
                }
            }

            // Вывод результатов обхода в MessageBox
            MessageBox.Show("Результат обхода в ширину: " + string.Join(", ", traversalResult));
        }
        //topologic sort
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            // Преобразование матрицы в граф
            adjacencyMatrix = GetMatrixValues();

            // Список для хранения результата топологической сортировки
            List<int> topologicalOrder = new List<int>();

            // Список для хранения посещенных вершин
            List<bool> visited = Enumerable.Repeat(false, adjacencyMatrix.Count).ToList();

            // Выполнение топологической сортировки для каждой вершины, если она еще не была посещена
            for (int i = 0; i < adjacencyMatrix.Count; i++)
            {
                if (!visited[i])
                {
                    TopologicalSortUtil(i, visited, topologicalOrder);
                }
            }

            // Переворачиваем результат, так как алгоритм добавляет вершины в обратном порядке
            topologicalOrder.Reverse();

            // Вывод результатов топологической сортировки в MessageBox
            MessageBox.Show("Результат топологической сортировки: " + string.Join(", ", topologicalOrder));
        }

        private void TopologicalSortUtil(int node, List<bool> visited, List<int> topologicalOrder)
        {
            visited[node] = true;

            // Получение соседей текущей вершины
            List<int> neighbors = GetNeighbors(node);

            // Рекурсивный обход соседей
            foreach (int neighbor in neighbors)
            {
                if (!visited[neighbor])
                {
                    TopologicalSortUtil(neighbor, visited, topologicalOrder);
                }
            }

            // Добавление текущей вершины в результат только после обработки всех соседей
            topologicalOrder.Add(node);
        }
        //дейкстры алгоритм
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            adjacencyMatrix = GetMatrixValues();

            // Выбор начальной вершины
            int startNode = 0; // Ваша начальная вершина

            // Список для хранения кратчайших расстояний до каждой вершины
            List<int> distances = Enumerable.Repeat(int.MaxValue, adjacencyMatrix.Count).ToList();

            // Массив, хранящий информацию о том, посещена ли вершина
            bool[] visited = new bool[adjacencyMatrix.Count];

            // Начальная вершина имеет расстояние 0
            distances[startNode] = 0;

            // Алгоритм Дейкстры
            for (int count = 0; count < adjacencyMatrix.Count - 1; count++)
            {
                // Выбираем вершину с минимальным расстоянием из непосещенных вершин
                int u = MinimumDistanceVertex(distances, visited);

                // Помечаем вершину как посещенную
                visited[u] = true;

                // Обновляем расстояния до всех вершин через текущую вершину
                for (int v = 0; v < adjacencyMatrix.Count; v++)
                {
                    if (!visited[v] && adjacencyMatrix[u][v] != 0 && distances[u] != int.MaxValue &&
                        distances[u] + adjacencyMatrix[u][v] < distances[v])
                    {
                        distances[v] = distances[u] + adjacencyMatrix[u][v];
                    }
                }
            }

            // Вывод результатов алгоритма Дейкстры в MessageBox
            string result = "Результат алгоритма Дейкстры:" + Environment.NewLine;
            for (int i = 0; i < distances.Count; i++)
            {
                result += $"Расстояние от вершины {startNode} до вершины {i}: {distances[i]}" + Environment.NewLine;
            }

            MessageBox.Show(result);
        }
        private int MinimumDistanceVertex(List<int> distances, bool[] visited)
        {
            int min = int.MaxValue, minIndex = -1;

            for (int v = 0; v < distances.Count; v++)
            {
                if (visited[v] == false && distances[v] <= min)
                {
                    min = distances[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }
        //раскраска графа
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            adjacencyMatrix = GetMatrixValues();

            // Создаем массив для хранения цветов вершин
            // Инициализируем все цвета как 0 (неопределенный)
            int[] colors = new int[adjacencyMatrix.Count];

            // Начинаем с произвольной вершины графа
            int startNode = 0;

            // Вызываем рекурсивную функцию для раскраски графа в два цвета
            bool isBipartite = TwoColoringUtil(startNode, colors);

            // Выводим результат в MessageBox
            if (isBipartite)
            {
                MessageBox.Show("Граф можно раскрасить в два цвета.");
            }
            else
            {
                MessageBox.Show("Граф нельзя раскрасить в два цвета.");
            }
        }
        private bool TwoColoringUtil(int node, int[] colors)
        {
            // Мы будем раскрашивать текущую вершину в цвет 1
            colors[node] = 1;

            // Создаем очередь для обхода графа
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(node);

            while (queue.Count > 0)
            {
                int currentNode = queue.Dequeue();

                // Получаем соседей текущей вершины
                List<int> neighbors = GetNeighbors(currentNode);

                // Перебираем соседей
                foreach (int neighbor in neighbors)
                {
                    // Если сосед еще не раскрашен, раскрашиваем его в противоположный цвет
                    if (colors[neighbor] == 0)
                    {
                        colors[neighbor] = -colors[currentNode]; // Противоположный цвет
                        queue.Enqueue(neighbor);
                    }
                    // Если сосед уже раскрашен в тот же цвет, что и текущая вершина, граф нельзя раскрасить в два цвета
                    else if (colors[neighbor] == colors[currentNode])
                    {
                        return false;
                    }
                }
            }

            // Если мы успешно обошли все вершины и не нашли противоречий, то граф можно раскрасить в два цвета
            return true;
        }
    }
}