namespace Sudoku;


public class SudokuException : Exception
{
    public SudokuException(string message) : base(message) { }


    public static bool ValidateUserPuzzle(int[] puzzle)
    {
        if (puzzle.Length != 81)
        {
            throw new SudokuException("Puzzle not corrent length");
        }
        if (!IsValidUnsolvedSudoku(puzzle))
        {
            PrintSudoku(puzzle);
            throw new SudokuException("Invalid Sudoku rules");
        }
        return true;
    }

    public static void PrintSudoku (int[] puzzle)
    {
        for (int i = 0; i < 81; i++)
        {
            Console.Write(puzzle[i] == 0 ? ". " : puzzle[i] + " ");
            if ((i + 1) % 9 == 0)
                Console.WriteLine();
        }
    }
    
    public static bool IsValidUnsolvedSudoku(int[] sudoku)      //checks users input for valid sudoku puzzle
    {
    // Check rows
    for (int row = 0; row < 9; row++)
    {
        var seen = new bool[10];
        for (int col = 0; col < 9; col++)
        {
            int val = sudoku[row * 9 + col];
            if (val < 0 || val > 9) return false; // Only allow 0-9
            if (val != 0)
            {
                if (seen[val]) return false;
                seen[val] = true;
            }
        }
    }

    // Check columns
    for (int col = 0; col < 9; col++)
    {
        var seen = new bool[10];
        for (int row = 0; row < 9; row++)
        {
            int val = sudoku[row * 9 + col];
            if (val != 0)
            {
                if (seen[val]) return false;
                seen[val] = true;
            }
        }
    }

    // Check 3x3 boxes
    for (int boxRow = 0; boxRow < 3; boxRow++)
    {
        for (int boxCol = 0; boxCol < 3; boxCol++)
        {
            var seen = new bool[10];
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int idx = (boxRow * 3 + row) * 9 + (boxCol * 3 + col);
                    int val = sudoku[idx];
                    if (val != 0)
                    {
                        if (seen[val]) return false;
                        seen[val] = true;
                    }
                }
            }
        }
    }

    return true;
    }
}