using System.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<int> inputVals = new List<int>();
        private List<int> inputDontCares = new List<int>();
        private List<char> inputVariables = new List<char>();

        private void display_Inputs(List<int> inputs)
        {
            for (short i = 0; i < inputs.Count; i++)
            {
                Console.Write("{0},", inputs[i]);
            }
            Console.WriteLine();
        }

        private void display_Inputs(List<char> inputs)
        {
            for (short i = 0; i < inputs.Count; i++)
            {
                Console.Write("{0},", inputs[i]);
            }
            Console.WriteLine();
        }

        private void check_CorrectNumberOfVariables()
        { 

        }

        private void check_AscendingValsAndNoDupInput()
        {
            for (short i = 0; i < inputVals.Count; i++)
                for (short j = (short)(i + 1); j < inputVals.Count; j++)
                    if (inputVals[i] > inputVals[j])
                        throw new Exception("[ERROR]: Hay nhap o <gia_tri> theo thu tu tang dan");
                    else if (inputVals[i] == inputVals[j])
                        throw new Exception("[ERROR]: O <gia_tri> ton tai gia tri giong nhau")
        }

        private void getNumbers_LineInputs(string line, ref List<int> currInputs)
        {
            int currNumb = 0;
            int currDigit = 1;
            if (line.Length <= 0) throw new Exception("[ERROR]: O <gia_tri/dont_care> khong duoc de trong");
            currInputs.Clear();
            for (short i = 0; i < line.Length; i++)
            {
                if (line[i] >= '0' && line[i] <= '9')
                {
                    currNumb = currNumb * currDigit + (line[i] - 48);
                    currDigit = 10;
                }
                else if (line[i] == ',')
                {
                    currInputs.Add(currNumb);
                    currNumb = 0;
                    currDigit = 1;
                }
                else throw new Exception("[ERROR]: O <gia_tri/dont_care> khong dung format");

                if (i + 1 >= line.Length && line[i] >= '0' && line[i] <= '9') currInputs.Add(currNumb);
            }
        }

        private void getChar_LineInputs(string line, ref List<char> currChars)
        {
            if (line.Length <= 0) throw new Exception("[ERROR]: O <ten_bien> khong duoc de trong");
            currChars.Clear();
            for (short i = 0; i < line.Length; i++)
            {
                if (i % 2 == 0 && (line[i] >= 'a' && line[i] <= 'z') || (line[i] >= 'A' && line[i] == 'Z'))
                {
                    currChars.Add(line[i]);
                }
                else if (line[i] == ',' && i % 2 != 0) continue;
                else throw new Exception("[ERROR]: O <ten_bien> khong dung format");
            }
        }

        private void display_Result(string message)
        {
            listView1.Items.Add(message);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string lineValue = maskedTextBox1.Text.Trim();
                string lineDontCare = maskedTextBox2.Text.Trim();
                string lineVariable = maskedTextBox3.Text.Trim();
                getNumbers_LineInputs(lineValue, ref inputVals);
                getNumbers_LineInputs(lineDontCare, ref inputDontCares);
                getChar_LineInputs(lineVariable, ref inputVariables);
                check_AscendingValsAndNoDupInput();
                display_Result("[INFO] - Gia tri - : " + lineValue);
                display_Result("[INFO] - Dont care - : " + lineDontCare);
                display_Result("[INFO] - Bien - : " + lineVariable);
            }
            catch (Exception ex)
            {
                display_Result(ex.Message);
                return;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}