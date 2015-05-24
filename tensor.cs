class Tensor
    {
        int row, column, dim;

        public int Row { get { return row; } }
        public int Column { get { return column; } }
        public int Dim { get { return dim; } }

        List<List<List<List<decimal>>>> tensor;
        public Tensor(int row, int column, int dim)
        {
            this.row = row;
            this.column = column;
            this.dim = dim;
            tensor = new List<List<List<List<decimal>>>>();
            for (int i = 0; i < row; i++)
            {
                var _row = new List<List<List<decimal>>>();
                for (int j = 0; j < column; j++)
                {
                    var row2 = new List<List<decimal>>();
                    for (int k = 0; k < dim; k++)
                    {
                        var row3 = new List<decimal>();
                        for (int m = 0; m < dim; m++)
                        {
                            row3.Add(0);
                        }
                        row2.Add(row3);
                    }

                    _row.Add(row2);
                }
                tensor.Add(_row);
            }

        }

        public override string ToString()
        {
            string str = "";

            foreach (var item in tensor)
            {
                foreach (var item2 in item)
                {
                    foreach (var item3 in item2)
                    {
                        foreach (var item4 in item3)
                        {
                            str += item4 + "\t";
                        }
                        str += "\n";
                    }
                    str += "\n";
                }
                str += "\n";
            }

            return str;
        }

        public string WriteToLaTex(string name)
        {
            string str = "\n"+@"$$" + name + @" =\ \large\left(\begin{array}{";

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    str += "c";
                }
                str += "|";
            }
            str = str.Substring(0, str.Length - 1);

            str += "}";
            string z = "";
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    for (int k = 0; k < column; k++)
                    {
                        for (int m = 0; m < dim; m++)
                        {
                            z += tensor[i][k][j][m] + "&";
                        }
                    }
                    str += z.Substring(0,z.Length-1)+"\\\\";
                    z = "";
                }
                str += @"\hline";
            }
            str=str.Substring(0,str.Length-8);
            str += @"\end{array}\right)$$";

            return str;
        }

        public decimal this[int index1, int index2, int index3, int index4]
        {
            get { return tensor[index1][index2][index3][index4]; }
            set { tensor[index1][index2][index3][index4] = value; }
        }

        public Tensor ToNewBasis(Matrix transition_matrix)
        {
            Matrix D = new Matrix(dim, dim);
            D = transition_matrix.Inverse();
            var temp = new Tensor(row, column, dim);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        for (int m = 0; m < dim; m++)
                        {
                            for (int p = 0; p < row; p++)
                            {
                                for (int q = 0; q < column; q++)
                                {
                                    for (int r = 0; r < dim; r++)
                                    {
                                        for (int s = 0; s < dim; s++)
                                        {
                                            temp[i, j, k, m] += tensor[p][q][r][s] * transition_matrix[s, m] * transition_matrix[p, i] * transition_matrix[q, j] * D[k, r];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return temp;
        }

        public Tensor ToNewBasis(Matrix C, Matrix D, out string Solve)
        {
            string str = ""; string[] z = new string[]{"","",""};
            var temp = new Tensor(row, column, dim);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    for (int k = 0; k < dim; k++)
                    {
                        for (int m = 0; m < dim; m++)
                        {
                            str += "\n"+@"$$\overline{\tau} ^{"+(k+1)+"}_{"+(m+1)+(i+1)+(j+1)+"}=";
                            for (int p = 0; p < row; p++)
                            {
                                for (int q = 0; q < column; q++)
                                {
                                    for (int r = 0; r < dim; r++)
                                    {
                                        for (int s = 0; s < dim; s++)
                                        {
                                            temp[i, j, k, m] += tensor[p][q][r][s] * C[s, m] * C[p, i] * C[q, j] * D[k, r];
                                            
                                            z[0] += @"+{\tau}^{" + (r + 1) + "}_{" + (s + 1) + (p + 1) + (q + 1) + "} C^{" + (s + 1) + "}_{" + (m + 1) + "} C^{" + (p + 1) + "}_{" + (i + 1) + "} C^{" + (q + 1) + "}_{" + (j + 1) + "} D^{" + (k + 1) + "}_{" + (r + 1) + "}";
                                            z[1] += "+" + tensor[p][q][r][s] + "*" + C[s, m] + "*" + C[p, i] + "*" + C[q, j] + "*" + D[k, r];
                                            z[2] += "+" + tensor[p][q][r][s] * C[s, m] * C[p, i] * C[q, j] * D[k, r];
                                        }
                                    }
                                }
                            }
                            str += z[0].Substring(1, z[0].Length - 1) + " = " + z[1].Substring(1, z[1].Length - 1) + " = " + z[2].Substring(1, z[2].Length - 1) + " = " + temp[i, j, k, m] + "$$\n";
                            z[0] = ""; z[1]= ""; z[2] = "";
                        }
                    }
                }
            }
            Solve = str;
            return temp;
        }

        public void LoadFromFile(string path)
        {
            tensor = new List<List<List<List<decimal>>>>();
            if (!File.Exists(path)) throw new Exception("Файл " + path + " отсутствует!");
            else
            {
                string textFromFile = "";
                using (StreamReader sr = File.OpenText(path))
                {
                    textFromFile = sr.ReadToEnd();
                }
                string[] temp = textFromFile.Split(new Char[] { '*' });
                //запись
                foreach (var item in temp)
                {
                    var row = new List<List<List<decimal>>>();
                    string[] temp2 = item.Split(new Char[] { '.' });
                    foreach (var item2 in temp2)
                    {
                        var row2 = new List<List<decimal>>();
                        string[] temp3 = item2.Split(new Char[] { ';' });
                        foreach (var item3 in temp3)
                        {
                            var row3 = new List<decimal>();
                            string[] temp4 = item3.Split(new Char[] { ' ' });
                            foreach (var item4 in temp4)
                            {
                                row3.Add(Convert.ToInt32(item4));
                            }
                            row2.Add(row3);
                        }
                        row.Add(row2);
                    }
                    tensor.Add(row);
                }
            }
        }

        public static Tensor operator +(Tensor t1, Tensor t2)
        {
            if (t1.row != t2.row || t1.column != t2.column)
            {
                throw new Exception("Сложение невозможно");
            }

            Tensor t = new Tensor(t1.row, t1.column, t1.dim);


            for (int i = 0; i < t.row; i++)
            {
                for (int j = 0; j < t.column; j++)
                {
                    for (int k = 0; k < t.dim; k++)
                    {
                        for (int m = 0; m < t.dim; m++)
                        {
                            t[i, j, k, m] = t1[i, j, k, m] + t2[i, j, k, m];
                        }
                    }
                }
            }

            return t;
        }

        public static Tensor operator -(Tensor t1, Tensor t2)
        {
            if (t1.row != t2.row || t1.column != t2.column)
            {
                throw new Exception("Сложение невозможно");
            }

            Tensor t = new Tensor(t1.row, t1.column, t1.dim);


            for (int i = 0; i < t.row; i++)
            {
                for (int j = 0; j < t.column; j++)
                {
                    for (int k = 0; k < t.dim; k++)
                    {
                        for (int m = 0; m < t.dim; m++)
                        {
                            t[i, j, k, m] = t1[i, j, k, m] - t2[i, j, k, m];
                        }
                    }
                }
            }

            return t;
        }
    }
