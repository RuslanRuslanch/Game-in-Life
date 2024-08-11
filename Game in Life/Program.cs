using System.Diagnostics;

namespace Game_in_Life
{
    public class Program
    {
        private static void Main(string[] args)
        {
            int width = 100;
            int height = 30;

            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);

            Constants constants = new Constants();
            Stopwatch stopwatch = new Stopwatch();

            Map map = new Map(width, height);

            map.Spawn(300);

            while (true)
            {
                stopwatch.Restart();

                map.Calculate();

                Console.Title = $"{(int)(1d / stopwatch.Elapsed.TotalSeconds)}";

                stopwatch.Stop();
            }
        }
    }

    public class Map
    {
        public Map(int width, int height)
        {
            _width = width;
            _height = height;

            _cells = new Cell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _cells[x, y] = new Cell(x, y);
                }
            }
        }

        private readonly int _width, _height;

        private readonly Cell[,] _cells;

        public void Set(int x, int y, CellState state)
        {
            _cells[x, y].SetState(state);
        }

        public bool IsAlive(int x, int y)
        {
            if (x >= 0 && x < _width &&
                y >= 0 && y < _height)
            {
                return _cells[x, y].State == CellState.Alive;
            }
            else
            {
                return false;
            }
        }

        public void Spawn(int amount)
        {
            Random random = new Random();

            for (int i = 0; i < amount; i++)
            {
                int x = random.Next(0, _width);
                int y = random.Next(0, _height);

                Set(x, y, CellState.Alive);
            }
        }

        public void Calculate()
        {
            int aliveCellNear = 0;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (IsAlive(x + 1, y))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x - 1, y))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x, y + 1))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x, y - 1))
                    {
                        aliveCellNear++;
                    }

                    if (IsAlive(x + 1, y + 1))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x - 1, y + 1))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x + 1, y - 1))
                    {
                        aliveCellNear++;
                    }
                    if (IsAlive(x - 1, y - 1))
                    {
                        aliveCellNear++;
                    }

                    if (IsAlive(x, y))
                    {
                        if (aliveCellNear < 2 || aliveCellNear > 3)
                        {
                            Set(x, y, CellState.Died);
                        }
                    }
                    else
                    {
                        if (aliveCellNear == 3)
                        {
                            Set(x, y, CellState.Alive);
                        }
                    }

                    aliveCellNear = 0;
                }
            }
        }
    }

    public class Constants
    {
        public readonly static Dictionary<CellState, string> CellFilter = new Dictionary<CellState, string>();

        public Constants()
        {
            CellFilter.Add(CellState.Died, " ");
            CellFilter.Add(CellState.Alive, "#");
        }
    }

    public class Cell
    {
        public Cell(int x, int y)
        {
            X = x;
            Y = y;

            CellView = new CellView(this);
        }

        public CellState State { get; private set; }

        public CellView CellView { get; }

        public int X { get; }
        public int Y { get; }

        public event Action StateSwitched;

        public void SetState(CellState state)
        {
            State = state;

            StateSwitched?.Invoke();
        }
    }

    public class CellView
    {
        public CellView(Cell cell)
        {
            _cell = cell;

            _cell.StateSwitched += UpdateValue;
        }

        ~CellView()
        {
            _cell.StateSwitched -= UpdateValue;
        }

        private readonly Cell _cell;

        public void UpdateValue()
        {
            Console.SetCursorPosition(_cell.X, _cell.Y);
            Console.Write(Constants.CellFilter[_cell.State]);
        }
    }

    public enum CellState : byte
    {
        Died,
        Alive,
    }
}
