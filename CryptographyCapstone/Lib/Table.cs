using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace CryptographyCapstone.Lib
{
    public class Table<T>
    {
        private T[][] table;

        public T this[int x, int y]
        {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }
        public T this[Point p]
        {
            get => GetValue(p);
            set => SetValue(p, value);
        }

        public Table(int rows, int cols, T initValue)
        {
            Init(rows, cols, initValue);
        }
        public Table(Rect size, T initValue)
        {
            Init((int)size.Width, (int)size.Height, initValue);
        }

        public void SetValue(int x, int y, T value)
        {
            table[x][y] = value;
        }
        public void SetValue(Point p, T value)
        {
            SetValue((int)p.X, (int)p.Y, value);
        }

        public T GetValue(int x, int y)
        {
            return table[x][y];
        }
        public T GetValue(Point p)
        {
            return GetValue((int) p.X, (int) p.Y);
        }

        public bool IsInTable(T value)
        {
            return IsInTable(table, value);
        }

        public static bool IsInTable<T1>(List<List<T1>> table, T1 item)
        {
            return table.Exists(column => column.Contains(item));
        }
        public static bool IsInTable<T1>(IEnumerable<IEnumerable<T1>> table, T1 item)
        {
            return table.ToList().Exists(column => column.Contains(item));
        }

        private void Init(int rows, int columns, T initValue)
        {
            var finalTable = new T[rows][];
            for (int i = 0; i < rows; i++)
            {
                finalTable[i] = new T[columns];
                for (int j = 0; j < columns; j++)
                {
                    finalTable[i][j] = initValue;
                }
            }
            table = finalTable;
        }

        public Dictionary<T, T> ToDictionary()
        {
            var output = new Dictionary<T, T>();
            
            return output;
        }
    }
}
