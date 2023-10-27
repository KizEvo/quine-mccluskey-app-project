using System.Linq;

namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class PrimeImplicant
        {
            public int valueBin;
            public bool isCurrStageMatched;
            public int dashBin;
            public List<bool> isDontCare = new List<bool>();
            public List<int> numbList = new List<int>();

            public PrimeImplicant(int value, bool dontCare)
            {
                valueBin = value;
                isCurrStageMatched = false;
                dashBin = 0;
                isDontCare.Add(dontCare);
                numbList.Add(value);
            }
        }

        // Global variables
        private List<int> inputVals = new List<int>();
        private List<int> inputDontCares = new List<int>();
        private List<char> inputVariables = new List<char>();
        // Global UI variables
        Color normalColor = Color.White;
        Color successColor = Color.LimeGreen;
        Color errorColor = Color.Red;
        string lineBreak = "--------------------------------------------------------------";

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

        private void check_DontCaresNotInVals()
        {
            for (short i = 0; i < inputDontCares.Count; i++)
                if (inputVals.Exists(x => x == inputDontCares[i]))
                    throw new Exception("[ERROR]: O <dont_care> ton tai gia tri trong o <gia_tri>");
        }

        private void check_CorrectNumberOfVariables()
        {
            int maxValue = inputVals[inputVals.Count - 1];
            int maxBit = (int)(Math.Floor(Math.Log2(maxValue))) + 1;
            if (maxBit != inputVariables.Count)
            {
                string message =
                    String.Format("[ERROR]: bit max({0})={1} != <ten_bien>={2}", maxValue, maxBit, inputVariables.Count);
                throw new Exception(message);
            }
        }

        private void check_AscendingValsAndNoDupInput()
        {
            for (short i = 0; i < inputVals.Count; i++)
                for (short j = (short)(i + 1); j < inputVals.Count; j++)
                    if (inputVals[i] > inputVals[j])
                        throw new Exception("[ERROR]: Hay nhap o <gia_tri> theo thu tu tang dan");
                    else if (inputVals[i] == inputVals[j])
                        throw new Exception("[ERROR]: O <gia_tri> ton tai gia tri giong nhau");
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

        private void display_Prime(PrimeImplicant obj)
        {
            string numListString = String.Join(",", obj.numbList);
            string isDontCareString = String.Join(",", obj.isDontCare);
            string message = String.Format(numListString + " - {0} - Matched: {1} - " + isDontCareString, 
                obj.dashBin, obj.isCurrStageMatched ? 'T' : 'F');
            display_Result(message);
        }

        private void display_PrimeGroups(Dictionary<int, List<PrimeImplicant>> groups)
        {
            Dictionary<int, List<PrimeImplicant>>.KeyCollection keyColl = groups.Keys;
            foreach (int key in keyColl)
            {
                List<PrimeImplicant> primeList = groups[key];
                string additionalInfo = String.Format("-- Key: {0} --", key);
                display_Result(additionalInfo);
                foreach (PrimeImplicant prime in primeList) 
                    display_Prime(prime);
            }
        }

        private void display_ClearResult()
        {
            listView1.Items.Clear();
        }

        private void set_ButtonColor(Color color)
        {
            button1.BackColor = color;
        }

        private void set_ButtonState(bool state)
        {
            button1.Enabled = state;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                display_ClearResult();
                set_ButtonState(false);
                // Get user inputs and check is inputs are in the pre-defined format
                string lineValue = maskedTextBox1.Text.Trim();
                string lineDontCare = maskedTextBox2.Text.Trim();
                string lineVariable = maskedTextBox3.Text.Trim();
                getNumbers_LineInputs(lineValue, ref inputVals);
                getNumbers_LineInputs(lineDontCare, ref inputDontCares);
                getChar_LineInputs(lineVariable, ref inputVariables);
                // Second checks if inputs are in the correct format to start using quine mccluskey algo
                check_AscendingValsAndNoDupInput();
                check_CorrectNumberOfVariables();
                check_DontCaresNotInVals();
                // Display success messages
                set_ButtonColor(successColor);
                display_Result("[INFO] - Gia tri - : " + lineValue);
                display_Result("[INFO] - Dont care - : " + lineDontCare);
                display_Result("[INFO] - Bien - : " + lineVariable);
                display_Result(lineBreak);
                // Main function call to quine mccluskey algo
                main_QuineMcCluskeyAlgo();
                // Wait 500ms before existing
                await Task.Delay(500);
                set_ButtonState(true);
                set_ButtonColor(normalColor);
            }
            catch (Exception ex)
            {
                display_Result(ex.Message);
                set_ButtonColor(errorColor);
                await Task.Delay(500);
                set_ButtonColor(normalColor);
                set_ButtonState(true);
                return;
            }
        }
        
        private int count_OnesOfInputBinary(int value)
        {
            int count = 0;
            while (value != 0)
            {
                int lsb = value & 1;
                if (lsb == 1) count++;
                value >>= 1;
            }
            return count;
        }

        private void arrange_InputGroup(Dictionary<int, List<PrimeImplicant>> groups)
        {
            for (short i = 0; i < inputVals.Count; i++)
            {
                int key = count_OnesOfInputBinary(inputVals[i]);
                if (!groups.ContainsKey(key))
                    groups.Add(key, new List<PrimeImplicant>());
                bool isDontCare = inputDontCares.Exists(x => x == inputVals[i]);
                PrimeImplicant minTerm = new PrimeImplicant(inputVals[i], isDontCare);
                groups[key].Add(minTerm);
            }
        }

        private void main_QuineMcCluskeyAlgo()
        {
            // Sorting
            inputDontCares.Sort();
            inputVals.AddRange(inputDontCares);
            inputVals.Sort();
            display_Inputs(inputVals);
            // Main
            Dictionary<int, List<PrimeImplicant>>groups = new Dictionary<int, List<PrimeImplicant>>();
            arrange_InputGroup(groups);
            display_PrimeGroups(groups);
            //PrimeImplicant obj = new PrimeImplicant(10, true);
            //display_Prime(obj);
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