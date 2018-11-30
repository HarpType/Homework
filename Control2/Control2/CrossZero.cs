using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control2
{
    class CrossZero
    {
        private bool crossTurn = true;

        private string[,] field = new string[3, 3];
        private bool[,] defender = new bool[3, 3];

        private int counter;

        public CrossZero()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = "0";
                    defender[i, j] = false;
                }

            counter = 0;
        }

        public string[, ] GetInfo(int row, int column)
        {
            if (defender[row, column])
                return field;

            if (counter == 9)
                clearField();


            counter += 1;

            if (crossTurn)
                field[row, column] = "1";
            else
                field[row, column] = "2";

            crossTurn = true ? false : true;

            return field;
        }

        private void clearField()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    field[i, j] = "0";
                    defender[i, j] = false;
                }
        }
    }
}
