using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FuzzyAHP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string filePath;

        List<Table> rootList = new List<Table>();
        List<string> options = new List<string>();
        string alt = "\r\n";
        string cellText;
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rootList.Clear();
            options.Clear();
            treeView1.Nodes.Clear();
            textBox1.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PRJ Files(.prj)|*.prj|Text Files(.txt) |*.txt";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                ReadFile(filePath, options);

            }



        }
        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (rootList.Count != 0)
            {
                if (rootList[0].matrix.Count != 0)
                {
                    SaveFileDialog sv = new SaveFileDialog();
                    sv.Filter = "PRJ Files(.prj)|*.prj|Text Files(.txt) |*.txt";
                    if (sv.ShowDialog() == DialogResult.OK)
                    {
                        string path = sv.FileName;
                        StreamWriter sw = new StreamWriter(path);
                        for (int i = 0; i < options.Count; i++)
                        {
                            sw.WriteLine(options[i]);
                        }
                        WriteFile(rootList[0], sw);
                        sw.Close();

                    }
                }
                else
                {
                    MessageBox.Show("Please enter at least options");

                }

            }
            else
            {
                MessageBox.Show("Please open or create a file");
            }
        }
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            dataGridView1.Rows.Clear();
            rootList.Clear();
            options.Clear();
            textBox1.Clear();
            Table table = new Table();
            table.name = "G";
            rootList.Add(table);
            TreeViewAdd(rootList, treeView1.Nodes);
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            cellText = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != 0 && e.ColumnIndex != 0)
            {
                double r;
                Table t = new Table();
                Cell cell = new Cell();
                Cell cellMirror = new Cell();

                if (e.RowIndex != e.ColumnIndex)
                {
                    if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null && !dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Contains("."))
                    {

                        string temp = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                        if (temp.Split(' ').Count<string>() == 3)
                        {
                            if (double.TryParse(temp.Split(' ')[0], out r) && double.TryParse(temp.Split(' ')[1], out r) && double.TryParse(temp.Split(' ')[2], out r))
                            {
                                GetTablebyName(dataGridView1[0, 0].Value.ToString(), rootList[0], ref t);
                                cell.low = double.Parse(temp.Split(' ')[0]);
                                cell.middle = double.Parse(temp.Split(' ')[1]);
                                cell.up = double.Parse(temp.Split(' ')[2]);
                                cellMirror.low = 1 / cell.up;
                                cellMirror.middle = 1 / cell.middle;
                                cellMirror.up = 1 / cell.low;
                                t.matrix[e.RowIndex - 1][e.ColumnIndex - 1] = cell;
                                t.matrix[e.ColumnIndex - 1][e.RowIndex - 1] = cellMirror;
                                if ((double)(int)(cellMirror.low) == cellMirror.low)
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value = cellMirror.low.ToString("F0");
                                }
                                else
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value = cellMirror.low.ToString("F2");
                                }
                                if ((double)(int)(cellMirror.middle) == cellMirror.middle)
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value += " " + cellMirror.middle.ToString("F0");

                                }
                                else
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value += " " + cellMirror.middle.ToString("F2");

                                }
                                if ((double)(int)(cellMirror.up) == cellMirror.up)
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value += " " + cellMirror.up.ToString("F0");
                                }
                                else
                                {
                                    dataGridView1[e.RowIndex, e.ColumnIndex].Value += " " + cellMirror.up.ToString("F2");
                                }

                            }
                            else
                            {
                                MessageBox.Show("Invalid Value");
                                dataGridView1[e.ColumnIndex, e.RowIndex].Value = cellText;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Wrong Format");
                            dataGridView1[e.ColumnIndex, e.RowIndex].Value = cellText;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Wrong Format");
                        dataGridView1[e.ColumnIndex, e.RowIndex].Value = cellText;
                    }


                }
                else
                {
                    MessageBox.Show("You can't change same criterias!!");
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = cellText;
                }

            }
            else
                dataGridView1[e.ColumnIndex, e.RowIndex].Value = cellText;

        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            /*Treeview'de node'a tıklandığında datagridview'da verilerin gösterilmesi */

            dataGridView1.Rows.Clear();
            Table t = new Table();
            GetTablebyName(e.Node.Text, rootList[0], ref t);
            if (t.matrix.Count != 0)
            {
                if (t.subTables.Count == 0)
                {
                    dataGridView1.RowCount = int.Parse(options[0]) + 1;
                    dataGridView1.ColumnCount = int.Parse(options[0]) + 1;
                    dataGridView1[0, 0].Value = t.name;
                    for (int i = 1; i < dataGridView1.RowCount; i++)
                    {
                        dataGridView1[0, i].Value = options[i];
                        dataGridView1[i, 0].Value = options[i];

                    }
                }
                else
                {
                    dataGridView1.RowCount = t.subTables.Count + 1;
                    dataGridView1.ColumnCount = t.subTables.Count + 1;
                    dataGridView1[0, 0].Value = t.name;
                    for (int i = 1; i < dataGridView1.RowCount; i++)
                    {
                        dataGridView1[0, i].Value = t.subTables[i - 1].name;
                        dataGridView1[i, 0].Value = t.subTables[i - 1].name;

                    }
                }
                for (int i = 1; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 1; j < dataGridView1.ColumnCount; j++)
                    {
                        dataGridView1[j, i].Value = t.matrix[i - 1][j - 1].low + " " + t.matrix[i - 1][j - 1].middle + " " + t.matrix[i - 1][j - 1].up;
                    }
                }
            }


        }
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //Node'a subnode ekleme
            int number;
            if (rootList[0].matrix.Count == 0)
            {
                string optionCount = Microsoft.VisualBasic.Interaction.InputBox("Number of Options:");

                if (int.TryParse(optionCount, out number))
                {
                    if (number > 0)
                    {
                        options.Add(optionCount);
                    }
                    for (int i = 0; i < number; i++)
                    {
                        string OptionName = Microsoft.VisualBasic.Interaction.InputBox("Name of" + (i + 1) + "." + "Options:");
                        if (OptionName != "")
                        {
                            options.Add(OptionName);
                        }
                        else
                        {
                            MessageBox.Show("Do not leave options name blank");
                            i--;
                        }

                    }
                }

                for (int i = 0; i < number; i++)
                {
                    List<Cell> cellList = new List<Cell>();
                    for (int j = 0; j < number; j++)
                    {
                        Cell cell = new Cell();
                        if (i == j)
                        {
                            cell.low = 1;
                            cell.middle = 1;
                            cell.up = 1;

                        }
                        else
                        {
                            cell.low = 0;
                            cell.middle = 0;
                            cell.up = 0;

                        }
                        cellList.Add(cell);

                    }
                    rootList[0].matrix.Add(cellList);
                }
            }
            if (options.Count != 0)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Number of Sub Criteria:");
                if (int.TryParse(input, out number))
                {
                    for (int i = 0; i < number; i++)
                    {
                        string subName = Microsoft.VisualBasic.Interaction.InputBox("Name of" + (i + 1) + "." + "Criteria");
                        if (subName != "")
                        {
                            AddSubTable(e.Node.Text, rootList[0], subName);
                        }
                        else
                        {
                            MessageBox.Show("Do not leave criteria name blank");
                            i--;
                        }

                    }
                    treeView1.Nodes.Clear();
                    TreeViewAdd(rootList, treeView1.Nodes);
                }
            }
            treeView1.ExpandAll();

        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            if (rootList.Count != 0 && options.Count > 0)
            {
                FindWeights(rootList[0]);
                WriteSyntheticExtent(rootList[0]);
                FindingLastWeight(rootList[0]);
                for (int i = 0; i < rootList[0].weigth.Count; i++)
                {
                    textBox1.Text += "W(" + options[i + 1] + ")" + "=" + rootList[0].weigth[i].ToString("F4") + "\r\n";
                }

            }
            else
            {
                MessageBox.Show("Please open or create a file");
            }


        }
        private void ReadFile(string path, List<string> options)
        {
            /*
             * Dosyadan seçenek adedi ve isimlerini okuyan fonksiyon
             * */
            List<String> data = new List<string>(File.ReadAllLines(path));
            int optionCount;
            if (int.TryParse(data[0], out optionCount))
            {
                for (int i = 0; i < optionCount + 1; i++)
                {
                    options.Add(data[i]);

                }
                int x = int.Parse(options[0]) + 1;
                Read(data, rootList, ref x);
                if (options.Count > 0)
                {
                    TreeViewAdd(new List<Table>(new Table[] { rootList[0] }), treeView1.Nodes);
                    treeView1.ExpandAll();
                }

            }
            else
                MessageBox.Show("Input line was not correct");
        }
        private void Read(List<string> list, List<Table> root, ref int i)
        {
            /*
           * Seçenek özelliklerinin ve alt özelliklerinin okunması
           * */
            try
            {
                double t;
                Table table = new Table();
                table.name = list[i].Split(';')[0];
                int subCount = int.Parse(list[i].Split(';')[1]);
                while (double.TryParse((list[++i])[0].ToString(), out t) == true)
                {

                    Cell cell;
                    List<Cell> cellList = new List<Cell>();
                    string[] row = list[i].Split(';');
                    string[] cellTemp;
                    for (int j = 0; j < row.Length; j++)
                    {
                        cell = new Cell();
                        cellTemp = row[j].Split(' ');
                        cell.low = double.Parse(cellTemp[0]);
                        cell.middle = double.Parse(cellTemp[1]);
                        cell.up = double.Parse(cellTemp[2]);
                        cellList.Add(cell);
                    }
                    table.matrix.Add(cellList);

                    if (i == list.Count - 1 || list[i + 1] == "")
                    {
                        break;
                    }
                }
                root.Add(table);
                for (int k = 0; k < subCount; k++)
                {
                    Read(list, root[root.Count - 1].subTables, ref i);

                }
            }
            catch (Exception ex)
            {
                options.Clear();
                rootList.Clear();
                MessageBox.Show("File format is incorrect ERROR:" + ex.Message);
            }

        }
        private List<Cell> RowSum(Table rootTable)
        {
            /*
           * RS'lerin oluşturulması
           * S'lerin oluşturulması
           * */
            List<Cell> rowSumCellList = new List<Cell>();
            Cell rowSum;
            Cell allRowSum = new Cell();

            for (int i = 0; i < rootTable.matrix.Count; i++)
            {
                rowSum = new Cell();
                for (int j = 0; j < rootTable.matrix[i].Count; j++)
                {

                    rowSum.low += rootTable.matrix[i][j].low;
                    rowSum.middle += rootTable.matrix[i][j].middle;
                    rowSum.up += rootTable.matrix[i][j].up;

                    allRowSum.low += rootTable.matrix[i][j].low;
                    allRowSum.middle += rootTable.matrix[i][j].middle;
                    allRowSum.up += rootTable.matrix[i][j].up;
                }
                rowSumCellList.Add(rowSum);
            }
            Cell reverseAllRowSum = new Cell();
            reverseAllRowSum.low = 1 / allRowSum.up;
            reverseAllRowSum.middle = 1 / allRowSum.middle;
            reverseAllRowSum.up = 1 / allRowSum.low;

            List<Cell> cellList = new List<Cell>();
            Cell cell;
            for (int i = 0; i < rowSumCellList.Count; i++)
            {
                cell = new Cell();
                cell.low = rowSumCellList[i].low * reverseAllRowSum.low;
                cell.middle = rowSumCellList[i].middle * reverseAllRowSum.middle;
                cell.up = rowSumCellList[i].up * reverseAllRowSum.up;
                cellList.Add(cell);
            }
            return cellList;
        }
        private double[] CompareCell(List<Cell> cellList)
        {
            /*
             * S'lerin karşılaştırılması
             * W değerlerinin oluşturulması
             * */
            double[] mins = new double[cellList.Count];
            List<double> temp = new List<double>();
            double[] weigth = new double[cellList.Count];
            for (int i = 0; i < cellList.Count; i++)
            {

                for (int j = 0; j < cellList.Count; j++)
                {
                    if (i != j)
                    {
                        if (cellList[i].middle >= cellList[j].middle)
                        {
                            temp.Add(1);

                        }
                        else
                        if (cellList[j].low <= cellList[i].up)
                        {
                            temp.Add((cellList[i].up - cellList[j].low) / ((cellList[i].up - cellList[i].middle) + (cellList[j].middle - cellList[j].low)));

                        }
                        else
                            temp.Add(0);

                    }
                }
                if (temp.Count > 0)
                {
                    mins[i] = temp.Min();
                }
                else
                    mins[i] = cellList[i].low;

                temp.Clear();
            }
            double sum = 0;
            for (int i = 0; i < mins.Length; i++)
            {
                sum += mins[i];
            }
            for (int i = 0; i < weigth.Length; i++)
            {
                weigth[i] = mins[i] / sum;

            }
            return weigth;
        }
        private void FindWeights(Table rootTable)
        {
            /*
             * Tüm ağırlıkların oluşturulması.
             * */
            List<Cell> rowSumList = RowSum(rootTable);
            rootTable.normalize = rowSumList;
            rootTable.weigth = new List<double>(CompareCell(rowSumList));

            for (int i = 0; i < rootTable.subTables.Count; i++)
            {

                FindWeights(rootTable.subTables[i]);

            }

        }
        private void TreeViewAdd(List<Table> rootTable, TreeNodeCollection treeNode)
        {
            /*
             * TreeView arayüzünde gösteren fonksiyon
             * Recursive olarak dizin isimleri ve alt dizinleri göstermek.
             * */
            for (int i = 0; i < rootTable.Count; i++)
            {
                treeNode.Add(rootTable[i].name);
                TreeViewAdd(rootTable[i].subTables, treeNode[i].Nodes);
            }


        }
        private void GetTablebyName(string name, Table rootTable, ref Table result)
        {

            /*
             * TreeView den nodeClick event çalıştıgında alınan name table döndürür
             */

            if (rootTable.name == name)
            {
                result = rootTable;
            }
            else
                for (int i = 0; i < rootTable.subTables.Count; i++)
                {

                    GetTablebyName(name, rootTable.subTables[i], ref result);

                }


        }
        private void WriteSyntheticExtent(Table rootTable)
        {
            /*
             * synthetic extent textbox yazırma
             * */
            textBox1.Text += rootTable.name + alt;
            textBox1.Text += "Values of fuzzy synthetic extent" + alt;
            for (int i = 0; i < rootTable.normalize.Count; i++)
            {
                if ((double)(int)(rootTable.normalize[i].low) == rootTable.normalize[i].low)
                {
                    textBox1.Text += "(" + rootTable.normalize[i].low + " ";
                }
                else
                {
                    textBox1.Text += "(" + rootTable.normalize[i].low.ToString("F4") + " ";
                }
                if ((double)(int)(rootTable.normalize[i].middle) == rootTable.normalize[i].middle)
                {
                    textBox1.Text += rootTable.normalize[i].middle + " ";
                }
                else
                {
                    textBox1.Text += rootTable.normalize[i].middle.ToString("F4") + " ";
                }
                if ((double)(int)(rootTable.normalize[i].up) == rootTable.normalize[i].up)
                {
                    textBox1.Text += rootTable.normalize[i].up + ")";
                }
                else
                {
                    textBox1.Text += rootTable.normalize[i].up.ToString("F4") + ")";
                }
                textBox1.Text += alt;
            }
            textBox1.Text += "Normalized weight vector" + alt;
            textBox1.Text += "(";
            for (int i = 0; i < rootTable.weigth.Count; i++)
            {
                if ((double)(int)(rootTable.weigth[i]) == rootTable.weigth[i])
                {
                    textBox1.Text += rootTable.weigth[i] + " ";
                }
                else
                    textBox1.Text += rootTable.weigth[i].ToString("F4") + " ";
            }
            textBox1.Text += ")";
            textBox1.Text += alt + alt;

            for (int i = 0; i < rootTable.subTables.Count; i++)
            {

                WriteSyntheticExtent(rootTable.subTables[i]);

            }


        }
        private List<double> FindingLastWeight(Table rootTable)
        {
            /*
             * Sonuç agırlıgın bulunması
             */
            List<List<double>> temp = new List<List<double>>();
            List<double> product = new List<double>();
            for (int i = 0; i < Convert.ToInt32(options[0]); i++)
            {
                product.Add(0);
            }
            for (int i = 0; i < rootTable.subTables.Count; i++)
            {
                temp.Add(FindingLastWeight(rootTable.subTables[i]));
            }
            if (temp.Count != 0)
            {

                for (int i = 0; i < product.Count; i++)
                {
                    for (int j = 0; j < rootTable.weigth.Count; j++)
                    {
                        product[i] += rootTable.weigth[j] * temp[j][i];
                    }
                }

                rootTable.weigth = product;
            }
            return rootTable.weigth;


        }
        private void AddSubTable(string name, Table rootTable, string subName)
        {
            /*
             * Herhangi bir kritere alt kriter eklenmesi 
             * */
            dataGridView1.Rows.Clear();
            if (rootTable.name == name)
            {
                List<Cell> temp = new List<Cell>();
                if (rootTable.subTables.Count == 0)
                {
                    rootTable.matrix.Clear();
                    temp.Add(new Cell(1, 1, 1));
                    rootTable.matrix.Add(temp);
                }
                else
                {
                    for (int i = 0; i < rootTable.subTables.Count; i++)
                    {
                        rootTable.matrix[i].Add(new Cell(0, 0, 0));
                    }

                    for (int i = 0; i < rootTable.matrix[0].Count; i++)
                    {
                        if (i == rootTable.matrix[0].Count - 1)
                        {
                            temp.Add(new Cell(1, 1, 1));
                        }
                        else
                            temp.Add(new Cell(0, 0, 0));
                    }
                    rootTable.matrix.Add(temp);
                }

                Table table = new Table();
                table.name = subName;
                int subCount = int.Parse(options[0]);
                for (int i = 0; i < subCount; i++)
                {
                    List<Cell> cellList = new List<Cell>();
                    for (int j = 0; j < subCount; j++)
                    {
                        Cell cell = new Cell();
                        if (i == j)
                        {
                            cell.low = 1;
                            cell.middle = 1;
                            cell.up = 1;

                        }
                        else
                        {
                            cell.low = 0;
                            cell.middle = 0;
                            cell.up = 0;

                        }
                        cellList.Add(cell);

                    }
                    table.matrix.Add(cellList);
                }
                rootTable.subTables.Add(table);

            }
            else
                for (int i = 0; i < rootTable.subTables.Count; i++)
                {

                    AddSubTable(name, rootTable.subTables[i], subName);

                }


        }
        private void WriteFile(Table rootTable, StreamWriter sw)
        {
            /*
             * Save işleminde dosyaya yazma
             * */
            sw.WriteLine(rootTable.name + ";" + rootTable.subTables.Count);
            for (int i = 0; i < rootTable.matrix.Count; i++)
            {
                for (int j = 0; j < rootTable.matrix[i].Count; j++)
                {
                    sw.Write(rootTable.matrix[i][j].low + " ");
                    sw.Write(rootTable.matrix[i][j].middle + " ");
                    if (j != rootTable.matrix[i].Count - 1)
                        sw.Write(rootTable.matrix[i][j].up + ";");
                    else
                        sw.Write(rootTable.matrix[i][j].up);
                }
                sw.WriteLine();
            }


            for (int i = 0; i < rootTable.subTables.Count; i++)
            {

                WriteFile(rootTable.subTables[i], sw);

            }


        }


    }

}

