using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Control2
{
    class ViewModelCrossZero
    {
        public string[, ] Field { get; private set; }

        public string Name = "TestName";

        private CrossZero crossZero;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelCrossZero(CrossZero crossZero)
        {
            this.crossZero = crossZero;

            Field = new string[3, 3];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Field[i, j] = "0";
        }

        public void GetData(int row, int column)
        {
            Field = crossZero.GetInfo(row, column);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Field)));
        }

    }
}
