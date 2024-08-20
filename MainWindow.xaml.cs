using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ClusteringApp
{
    /// <summary>
    /// Класс основного окна приложения
    /// </summary>
    public partial class MainWindow : Window
    {
        GridDrawer? _gridDrawer;
        Field? _field;

        /// <summary>
        /// Инициализцация окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            generateButton.Click += GenerateButtonClicked;
            undoButton.Click += UndoButtonClicked;
            redoButton.Click += RedoButtonClicked;
            matrixSizeBox.PreviewTextInput += CheckTextInput;
        }

        /// <summary>
        /// Метод позволяет выполнить основные действия по отрисовке генерируемой матрицы (поля) на сетке (таблице)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            _gridDrawer?.Dispose();
            _field = new Field(GetMatrixSize(), 0);
            _gridDrawer = new GridDrawer(digitGrid, _field);
            _gridDrawer.Update();
            if (_field.ClusterKeeper.Count > 1)
            {
                undoButton.IsEnabled = true;
                redoButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Метод получает из TextBox знаение размерности матрицы
        /// </summary>
        /// <returns>Размерность матрицы</returns>
        private int GetMatrixSize()
        {
            int matrixSizeValue;
            if (!int.TryParse(matrixSizeBox.Text, out matrixSizeValue) || int.Parse(matrixSizeBox.Text) == 0)
            {
                matrixSizeValue = 16;
                matrixSizeBox.Text = matrixSizeValue.ToString();
            }
            return matrixSizeValue;
        }

        /// <summary>
        /// Метод позволяет выполнить отмену последней отрисовки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButtonClicked(object sender, EventArgs e)
        {
            (bool, bool)? enabledData = _gridDrawer?.Undo();
            undoButton.IsEnabled = enabledData?.Item1 ?? undoButton.IsEnabled;
            redoButton.IsEnabled = enabledData?.Item2 ?? redoButton.IsEnabled;
        }

        /// <summary>
        /// Метод позволяет вернуть последнюю отрисовку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RedoButtonClicked(object sender, EventArgs e)
        {
            (bool, bool)? enabledData = _gridDrawer?.Redo();
            undoButton.IsEnabled = enabledData?.Item1 ?? undoButton.IsEnabled;
            redoButton.IsEnabled = enabledData?.Item2 ?? redoButton.IsEnabled;
        }

        /// <summary>
        /// Метод ограничивает ввод в TextBox только цифрами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)e.OriginalSource;
            e.Handled = !IsTextAllowed(e.Text) || (textBox.Text == "" && int.Parse(e.Text) == 0);
        }

        /// <summary>
        /// Метод проверяет разрешенность ввоимого текста
        /// </summary>
        /// <param name="text">Вводимый текст</param>
        /// <returns>Разрешен ли текст</returns>
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]");
            return !regex.IsMatch(text);
        }
    }
}