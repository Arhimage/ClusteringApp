using System.Drawing;

namespace ClusteringApp
{
    /// <summary>
    /// Класс управляющей "полем" (матрицей) с группами
    /// </summary>
    internal class Field
    {
        public readonly int[,] Cells;
        public List<List<Point>> ClusterKeeper { get; private set; } = new();

        private Random _rand;
        private List<Point> _aimCells = new();

        /// <summary>
        /// Создает поле с данными о группировке
        /// </summary>
        /// <param name="matrixSize">Размер генерируемой матрицы</param>
        /// <param name="randSeed">Сид генерируемой матрицы</param>
        /// <exception cref="ArgumentException"></exception>
        public Field(int matrixSize)
        {
            if (matrixSize <= 0)
            {
                throw new ArgumentException("Размерность матрицы должна быть натуральным числом!");
            }
            _rand = new Random();
            Cells = FillCells(matrixSize);
            FillClusters();
        }

        /// <summary>
        /// Заолпнет ячейки матрицы нулями (30%) и единицами (70%0
        /// </summary>
        /// <param name="matrixSize">Размер матрицы</param>
        /// <returns></returns>
        private int[,] FillCells(int matrixSize)
        {
            int[,] cells = new int[matrixSize, matrixSize];
            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = 0; j < matrixSize; j++)
                {
                    if (_rand.NextSingle() >= 0.3f)
                    {
                        cells[i, j] = 1;
                        _aimCells.Add(new Point(i, j));
                    }
                    else
                    {
                        cells[i, j] = 0;
                    }
                }
            }
            return cells;
        }

        /// <summary>
        /// Заполняет массив областей (кластеров) ClusterKeeper
        /// </summary>
        private void FillClusters()
        {
            int[,] matrix = (int[,])Cells.Clone();
            do
            {
                FillNextCluster(_aimCells.Last(), matrix);
            } while (_aimCells.Count > 0);
        }

        /// <summary>
        /// Группирует точки в кластер, при соблюдении условий кластиризации
        /// </summary>
        /// <param name="clusterCore">Точка начала расчета кластера</param>
        private void FillNextCluster(Point clusterCore, int[,] matrix)
        {
            ClusterKeeper.Add(new List<Point> { clusterCore });
            int cValue = ClusterKeeper.Count + 1;
            Stack<Point> internalStack = new Stack<Point>();
            matrix[clusterCore.X, clusterCore.Y] = cValue;

            do
            {
                AddSurroundPoints(clusterCore, matrix, internalStack, cValue);
                _aimCells.Remove(clusterCore);
                if (internalStack.Count <= 0)
                {
                    break;
                }
                else
                {
                    clusterCore = internalStack.Pop();
                }
            } while (true);
        }

        /// <summary>
        /// Выполняет отслежива=ние состояниея соседних точек-кандидатов в кластер
        /// </summary>
        /// <param name="clusterCore">Добавляемая в кластер точка</param>
        /// <param name="matrix">Копия сгенерированной матрицы</param>
        /// <param name="internalStack">Стэк точек-кандидатов в кластер</param>
        /// <param name="cValue">Индекс кластера</param>
        private void AddSurroundPoints(Point clusterCore, int[,] matrix, Stack<Point> internalStack, int cValue)
        {
            Point up = new Point(clusterCore.X, clusterCore.Y + 1);
            if (up.Y < matrix.GetLength(1))
            {
                AddNextPoint(ref matrix, internalStack, up, cValue);
            }
            Point right = new Point(clusterCore.X + 1, clusterCore.Y);
            if (right.X < matrix.GetLength(0))
            {
                AddNextPoint(ref matrix, internalStack, right, cValue);
            }
            Point down = new Point(clusterCore.X, clusterCore.Y - 1);
            if (down.Y >= 0)
            {
                AddNextPoint(ref matrix, internalStack, down, cValue);
            }
            Point left = new Point(clusterCore.X - 1, clusterCore.Y);
            if (left.X >= 0)
            {
                AddNextPoint(ref matrix, internalStack, left, cValue);
            }
        }

        /// <summary>
        /// Выполняет процедуру добавления точки-кандидата в кластер
        /// </summary>
        /// <param name="matrix">Копия сгенерированной матрицы</param>
        /// <param name="internalStack">Стэк точек-кандидатов в кластер</param>
        /// <param name="point">Точка-кандидат</param>
        /// <param name="cValue">Индекс кластера</param>
        private void AddNextPoint(ref int[,] matrix, Stack<Point> internalStack, Point point, int cValue)
        {
            if (matrix[point.X, point.Y] != cValue && matrix[point.X, point.Y] != 0)
            {
                matrix[point.X, point.Y] = cValue;
                internalStack.Push(point);
                ClusterKeeper.Last().Add(point);
            }
        }
    }
}
