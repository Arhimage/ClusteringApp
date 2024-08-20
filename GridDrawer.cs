using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Point = System.Drawing.Point;

namespace ClusteringApp
{
    /// <summary>
    /// Класс занимается отрисовкой поля (матрицы с кластерами)
    /// </summary>
    internal class GridDrawer : IDisposable
    {
        private ObservableCollection<Dictionary<string, object>>? _grid;
        private DataGrid _digitGrid;
        private Field _field;
        private int _step;

        /// <summary>
        /// Содает класс отрисовки поля
        /// </summary>
        /// <param name="digitGrid">Сетка для отрисовки</param>
        /// <param name="field">Отрисовываемое поле</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GridDrawer(DataGrid digitGrid, Field field)
        {
            if (field == null || digitGrid == null)
            {
                throw new ArgumentNullException();
            }
            _field = field;
            _digitGrid = digitGrid;
            _step = _field.ClusterKeeper.Count;
        }

        /// <summary>
        /// Деструктор класса отрисовки поля
        /// </summary>
        public void Dispose()
        {
            _digitGrid.ItemsSource = null;
            _digitGrid.Columns.Clear();
        }

        /// <summary>
        /// Отмена последнего этапа отрисовки
        /// </summary>
        /// <returns>Состояние отмены, состояние возврата</returns>
        public (bool, bool) Undo()
        {
            if (_step - 1 >= 0)
            {
                _step--;
                _grid?.Clear();
                Update();
            }
            return (_step - 1 >= 0, _step + 1 <= _field.ClusterKeeper.Count);
        }

        /// <summary>
        /// Возврат последнего этапа отрисовки
        /// </summary>
        /// <returns><Состояние отмены, состояние возврата/returns>
        public (bool, bool) Redo()
        {
            if (_step + 1 <= _field.ClusterKeeper.Count)
            {
                _step++;
                _grid?.Clear();
                Update();
            }
            return (_step - 1 >= 0, _step + 1 <= _field.ClusterKeeper.Count);
        }

        /// <summary>
        /// Обновление отрисовываемой области
        /// </summary>
        public void Update()
        {
            _digitGrid.ItemsSource = null;
            _digitGrid.Columns.Clear();
            _grid = new ObservableCollection<Dictionary<string, object>>();
            CreateColumns();
            FillColums();
            DrawSteps();
            _digitGrid.ItemsSource = _grid;
        }

        /// <summary>
        /// Заполнение сетки столбцами
        /// </summary>
        private void CreateColumns()
        {
            int length = _field.Cells.GetLength(1);
            _digitGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = $"",
                Binding = new Binding($"[item0]")
            });
            for (int i = 1; i <= length; i++)
            {
                _digitGrid.Columns.Add(new DataGridTextColumn()
                {
                    Header = $"{i - 1}",
                    Binding = new Binding($"[item{i}]")
                });
            }
        }

        /// <summary>
        /// Метод заполняет сетку значениями из поля
        /// </summary>
        private void FillColums()
        {
            int length0 = _field.Cells.GetLength(0);
            int length1 = _field.Cells.GetLength(1);
            for (int i = 0; i < length0; i++)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>() { { "item0", $"{i}" } };
                for (int j = 1; j <= length1; j++)
                {
                    dict[$"item{j}"] = (_field.Cells[i, j - 1] == 0) ? "*" : "1";
                }
                _grid?.Add(dict);
            }
        }

        /// <summary>
        /// Метод пошагово отрисовывает области поля
        /// </summary>
        private void DrawSteps()
        {
            if (_grid == null) return;
            for (int i = 0; i < _step; i++)
            {
                for (int j = 0; j < _field.ClusterKeeper[i].Count; j++)
                {
                    Point p = _field.ClusterKeeper[i][j];
                    _grid[p.X][$"item{p.Y + 1}"] = i + 2;
                }

            }
        }
    }
}
