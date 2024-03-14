﻿using Microsoft.Win32;
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
        public MainWindow()
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
    }
}