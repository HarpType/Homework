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
        // Поле отображения игры
        public string[, ] Field { get; private set; }

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

        /// <summary>
        /// Метод посылает запрос crossZero на изменение поля
        /// </summary>
        /// <param name="row">номер строки, на которую нажали</param>
        /// <param name="column">номер столбца, на которую нажали</param>
        public void GetData(int row, int column)
        {
            Field = crossZero.GetInfo(row, column);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Field)));
        }

    }
}
