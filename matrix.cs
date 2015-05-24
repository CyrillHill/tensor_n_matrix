    public class Matrix
    {
        static Random rnd = new Random();

        decimal[,] array;
        int row, column;

        public int Row { get { return row; } }
        public int Column { get { return column; } }

        public void Random()
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    array[i, j] = rnd.Next(10);
                }
            }
        }

        public void Random(int min, int max)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    array[i, j] = rnd.Next(min, max);
                }
            }
        }

        public Matrix(int row, int colunm)
        {
            this.row = row;
            this.column = colunm;
            array = new decimal[row, column];
        }

        public Matrix(int row, int colunm, decimal[,] m)
        {
            this.row = row;
            this.column = colunm;
            array = new decimal[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    array[i, j] = m[i, j];
                }
            }
        }

        public Matrix Transpose()
        {
            Matrix m = new Matrix(column, row);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m[j, i] = array[i, j];
                }
            }

            return m;
        }

        public void TransposeMyself()
        {
            array = Transpose().array;
        }

        public Matrix Inverse()
        {
            decimal det = Determinant();
            if (det == 0)
            {
                throw new Exception("Матрица вырождена");
            }

            Matrix m = new Matrix(row, column);
      
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    m[i, j] = Cofactor(array, i, j) / det;
                }
            }

            return m.Transpose();
        }

        public decimal Determinant()
        {
            if (column != row)
            {
                throw new Exception("Расчет определителя невозможен");
            }
            return Determinant(array);
        }

        private decimal Determinant(decimal[,] array)
        {
            int n = (int)Math.Sqrt(array.Length);

            if (n == 1)
            {
                return array[0, 0];
            }

            decimal det = 0;

            for (int k = 0; k < n; k++)
            {
                det += array[0, k] * Cofactor(array, 0, k);
            }

            return det;
        }

        private decimal Cofactor(decimal[,] array, int row, int column)
        {
            return Convert.ToInt32(Math.Pow(-1, column + row)) * Determinant(Minor(array, row, column));
        }

        private decimal[,] Minor(decimal[,] array, int row, int column)
        {
            int n = (int)Math.Sqrt(array.Length);
            decimal[,] minor = new decimal[n - 1, n - 1];

            int _i = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row)
                {
                    continue;
                }
                int _j = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }
                    minor[_i, _j] = array[i, j];
                    _j++;
                }
                _i++;
            }
            return minor;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Сложение невозможно");
            }

            Matrix m = new Matrix(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m[i, j] = m1[i, j] + m2[i, j];
                }
            }

            return m;
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            if (m1.row != m2.row || m1.column != m2.column)
            {
                throw new Exception("Вычитание невозможно");
            }

            Matrix m = new Matrix(m1.row, m1.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m[i, j] = m1[i, j] - m2[i, j];
                }
            }

            return m;
        }

        public static Matrix operator *(decimal a, Matrix m1)
        {
            Matrix m = new Matrix(m1.row, m1.column);
            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m1.column; j++)
                {
                    m[i, j] = a * m1[i, j];
                }
            }

            return m;
        }

        public static Matrix operator *(Matrix m1, decimal a)
        {
            return Convert.ToDecimal(a) * m1;
        }

        public static Matrix operator *(int a, Matrix m1)
        {
            return Convert.ToDecimal(a) * m1;
        }

        public static Matrix operator *(double a, Matrix m1)
        {
            return Convert.ToDecimal(a) * m1;
        }

        public static Matrix operator ^(Matrix m1, int a)
        {
            if (m1.column != m1.row || a < 2)
            {
                throw new Exception("Возведение в степень невозможно");
            } 

            Matrix m = new Matrix(m1.row, m1.column);

            m = m1 * m1;
            for (int i = 1; i < a - 1; i++)
            {
                m = m1 * m;
            }

            return m;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if (m1.column != m2.row)
            {
                throw new Exception("Умножение невозможно");
            }

            Matrix m = new Matrix(m1.row, m2.column);

            for (int i = 0; i < m1.row; i++)
            {
                for (int j = 0; j < m2.column; j++)
                {
                    decimal sum = 0;

                    for (int k = 0; k < m1.column; k++)
                    {
                        sum += m1[i, k] * m2[k, j];
                    }

                    m[i, j] = sum;
                }
            }

            return m;
        }

        public decimal this[int index1, int index2]
        {
            get { return array[index1, index2]; }
            private set { array[index1, index2] = value; }
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    str += array[i, j] + "\t";
                }
                str += "\n";
            }

            return str;
        }
    }
