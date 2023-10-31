using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

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
            public PrimeImplicant() { }

            public static bool operator ==(PrimeImplicant obj1, PrimeImplicant obj2)
            {
                if (obj1.numbList.SequenceEqual(obj2.numbList) && obj1.isDontCare.SequenceEqual(obj2.isDontCare)
                    && obj1.valueBin == obj2.valueBin && obj1.isCurrStageMatched == obj2.isCurrStageMatched
                    && obj1.dashBin == obj2.dashBin)
                    return true;
                return false;
            }

            public static bool operator !=(PrimeImplicant obj1, PrimeImplicant obj2)
            {
                if (obj1.numbList.SequenceEqual(obj2.numbList) && obj1.isDontCare.SequenceEqual(obj2.isDontCare)
                    && obj1.valueBin == obj2.valueBin && obj1.isCurrStageMatched == obj2.isCurrStageMatched
                    && obj1.dashBin == obj2.dashBin)
                    return false;
                return true;
            }

            public bool setMatchStatus { set { isCurrStageMatched = value; } }
            public int setDashPosition { set { dashBin = dashBin | (1 << value); } }
        }

        // Global variables
        private List<int> inputVals = new List<int>();
        private List<int> inputDontCares = new List<int>();
        private List<char> inputVariables = new List<char>();
        // Global UI variables
        Color normalColor = Color.White;
        Color successColor = Color.LimeGreen;
        Color errorColor = Color.Red;
        string lineBreak = "----------------------------------------------";

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
            int maxBit = get_BinaryLengthOfNumb(maxValue);
            if (inputDontCares.Count > 0)
            {
                inputDontCares.Sort();
                int maxValueDontCare = inputDontCares[inputDontCares.Count - 1];
                int maxBitDontCare = get_BinaryLengthOfNumb(maxValueDontCare);
                if (maxBit != inputVariables.Count && maxBitDontCare != inputVariables.Count)
                {
                    string message =
                        String.Format("[ERROR]: bit max({0})={1} != <ten_bien>={2}, hay nhap them hoac bo <ten_bien>", maxValue > maxValueDontCare ? maxValue : maxValueDontCare, maxBit > maxBitDontCare ? maxBit : maxBitDontCare, inputVariables.Count);
                    throw new Exception(message);
                }
            } else
            {
                if (maxBit != inputVariables.Count)
                {
                    string message =
                        String.Format("[ERROR]: bit max({0})={1} != <ten_bien>={2}, hay nhap them hoac bo <ten_bien>", maxValue, maxBit, inputVariables.Count);
                    throw new Exception(message);
                }
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
            try
            {
                String[] strList = line.Split(',');
                foreach (String str in strList)
                    currInputs.Add(int.Parse(str));
            }
            catch (Exception e)
            {
                throw new Exception("[ERROR]: O <gia_tri>/<dont_care> khong dung format/hoac dang de trong");
            }
        }

        private void getChar_LineInputs(string line, ref List<char> currChars)
        {
            if (line.Length <= 0) throw new Exception("[ERROR]: O <ten_bien> khong duoc de trong");
            for (short i = 0; i < line.Length; i++)
            {
                if (i % 2 == 0 && ((line[i] >= 'a' && line[i] <= 'z') || (line[i] >= 'A' && line[i] <= 'Z')))
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
                // Clear previous inputs
                inputVals.Clear();
                inputDontCares.Clear();
                inputVariables.Clear();
                // Get user entered inputs, <dont_care> input can be empty
                getNumbers_LineInputs(lineValue, ref inputVals);
                if (lineDontCare.Length > 0) getNumbers_LineInputs(lineDontCare, ref inputDontCares);
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
                display_Result("[INFO] Ten bien ghi dau tien la MSB -> LSB");
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

        private int get_BinaryLengthOfNumb(int numb)
        {
            return (int)(Math.Floor(Math.Log2(numb))) + 1;
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

        private Dictionary<int, List<PrimeImplicant>> find_NextPrimeGroups(Dictionary<int, List<PrimeImplicant>> prevGroups, int maxBitLength)
        {
            Dictionary<int, List<PrimeImplicant>> newGroups = new Dictionary<int, List<PrimeImplicant>>();
            List<PrimeImplicant> noMatchPrimes = new List<PrimeImplicant>();
            foreach (KeyValuePair<int, List<PrimeImplicant>> prevItem in prevGroups)
            {
                foreach (PrimeImplicant currKeyObjPrime in prevItem.Value)
                {
                    if (!prevGroups.ContainsKey(prevItem.Key + 1)) break;
                    foreach (PrimeImplicant cmpKeyObjPrime in prevGroups[prevItem.Key + 1])
                    {
                        PrimeImplicant dcCurrObjPrime = deepCopy_PrimeImplicant(currKeyObjPrime);
                        PrimeImplicant dcCmpObjPrime = deepCopy_PrimeImplicant(cmpKeyObjPrime);
                        update_CurrDcPrimeImplicantPair(ref dcCurrObjPrime, ref dcCmpObjPrime, maxBitLength);
                        if (dcCurrObjPrime == currKeyObjPrime)
                            continue;
                        currKeyObjPrime.setMatchStatus = true;
                        cmpKeyObjPrime.setMatchStatus = true;
                        update_NewGroups(ref newGroups, ref dcCurrObjPrime, maxBitLength);
                    }
                }
                // Check all prime obj that wasn't matched in this iteration and add them to newGroups Dict
                for (short i = 0; i < prevItem.Value.Count; i++)
                {
                    PrimeImplicant prime = prevItem.Value[i];
                    if (!prime.isCurrStageMatched)
                        update_NewGroups(ref newGroups, ref prime, maxBitLength);
                }
            }
            return newGroups;
        }

        private PrimeImplicant deepCopy_PrimeImplicant(PrimeImplicant primeObjToBeCopied)
        {
            PrimeImplicant newPrime = new PrimeImplicant();
            // Perform manual copy
            newPrime.valueBin = primeObjToBeCopied.valueBin;
            newPrime.dashBin = primeObjToBeCopied.dashBin;
            newPrime.isCurrStageMatched = primeObjToBeCopied.isCurrStageMatched;
            // Loop through list
            for (short i = 0; i < primeObjToBeCopied.numbList.Count; i++)
            {
                newPrime.numbList.Add(primeObjToBeCopied.numbList[i]);
                newPrime.isDontCare.Add(primeObjToBeCopied.isDontCare[i]);
            }
            return newPrime;
        }

        private void update_CurrDcPrimeImplicantPair(ref PrimeImplicant dcCurrPrime, ref PrimeImplicant dcCmpPrime, int maxBitLength)
        {
            int newDashPosition = get_OneBitDiffPosition(dcCurrPrime.valueBin, dcCmpPrime.valueBin,
                dcCurrPrime.dashBin, dcCmpPrime.dashBin, maxBitLength);
            if (newDashPosition != -1)
            {
                dcCurrPrime.setMatchStatus = true;
                dcCurrPrime.setDashPosition = newDashPosition;
                for (short i = 0; i < dcCmpPrime.numbList.Count; i++)
                {
                    dcCurrPrime.numbList.Add(dcCmpPrime.numbList[i]);
                    dcCurrPrime.isDontCare.Add(dcCmpPrime.isDontCare[i]);
                }
            }
        }

        private int get_OneBitDiffPosition(int currValue, int cmpValue, int currDash, int cmpDash, int maxBitLength)
        {
            int shiftAmount = 0;
            int countDiff = 0;
            int diffPosition = 0;
            if (currDash != cmpDash) return -1;
            while (shiftAmount < maxBitLength)
            {
                int currValueShifted = (currValue >> shiftAmount) & 1;
                int cmpValueShifted = (cmpValue >> shiftAmount) & 1;
                if (currValueShifted != cmpValueShifted)
                {
                    countDiff += 1;
                    diffPosition = shiftAmount;
                }
                shiftAmount += 1;
            }
            return countDiff == 1 ? diffPosition : -1;
        }

        private void update_NewGroups(ref Dictionary<int, List<PrimeImplicant>> newGroups, ref PrimeImplicant newPrime, int maxBitLength)
        {
            int newDcCurrObjValueBin = remove_ValueBinBitBasedOnDashBin(newPrime.valueBin, newPrime.dashBin, maxBitLength);
            int newKey = count_OnesOfInputBinary(newDcCurrObjValueBin);
            add_PrimeToNextGroups(ref newGroups, ref newPrime, newKey);
        }

        private int remove_ValueBinBitBasedOnDashBin(int currPrimeValBin, int currPrimeDashBin, int maxBitLength)
        {
            for (int shiftAmount = 0; shiftAmount < maxBitLength; shiftAmount++)
            {
                int lsbOfDashBin = (currPrimeDashBin >> shiftAmount) & 1;
                if (lsbOfDashBin == 1) currPrimeValBin &= ~(1 << shiftAmount);
            }
            return currPrimeValBin;
        }

        private void add_PrimeToNextGroups(ref Dictionary<int, List<PrimeImplicant>> newGroups, ref PrimeImplicant newPrime, int newKey)
        {
            if (!newGroups.ContainsKey(newKey))
                newGroups.Add(newKey, new List<PrimeImplicant>());
            newGroups[newKey].Add(newPrime);
        }

        private bool check_GroupsEquality(ref Dictionary<int, List<PrimeImplicant>> group1, ref Dictionary<int, List<PrimeImplicant>> group2)
        {
            if (group1.Count != group2.Count) return false;
            foreach (int keyGroup1 in group1.Keys)
                if (!group2.ContainsKey(keyGroup1)) return false;
            foreach (KeyValuePair<int, List<PrimeImplicant>> item in group1)
                if (group2[item.Key].Count != item.Value.Count) return false;
            return true;
        }

        private Dictionary<int, List<PrimeImplicant>> clear_PrimeMatchStatusInGroups(Dictionary<int, List<PrimeImplicant>> groups)
        {
            foreach (List<PrimeImplicant> primeList in groups.Values)
                foreach (PrimeImplicant prime in primeList)
                    prime.setMatchStatus = false;
            return groups;
        }

        private List<PrimeImplicant> get_PrimesNoDupInGroups(ref Dictionary<int, List<PrimeImplicant>> groups)
        {
            List<PrimeImplicant> noDupPrimes = new List<PrimeImplicant>();
            foreach (List<PrimeImplicant> primesList in groups.Values)
            {
                foreach (PrimeImplicant prime in primesList)
                {
                    if (!noDupPrimes.Exists(x => x.numbList.Sum() == prime.numbList.Sum()))
                        noDupPrimes.Add(prime);
                }
            }
            return noDupPrimes;
        }

        private (List<List<int>>, List<int>) create_PrimeChart(List<PrimeImplicant> primeList)
        {
            List<int> cols = new List<int>();
            foreach (PrimeImplicant prime in primeList)
            {
                for (int i = 0; i < prime.numbList.Count; i++)
                {
                    if (!cols.Exists(x => x == prime.numbList[i]) && !(prime.isDontCare[i]))
                        cols.Add(prime.numbList[i]);
                }
            }
            cols.Sort();
            List<List<int>> primeChart = create_TwoDimMatrix(cols.Count, primeList.Count);
            for (int i = 0; i < primeList.Count; i++)
            {
                foreach (int colValue in primeList[i].numbList)
                {
                    int col = cols.FindIndex(x => x == colValue);
                    if (col != -1) primeChart[col][i] = i;
                }
            }
            return (primeChart, cols);
        }

        private List<List<int>> create_TwoDimMatrix(int rowLength, int colLength)
        {
            List<List<int>> matrix = new List<List<int>>();
            for (short r = 0; r < rowLength; r++)
            {
                matrix.Add(new List<int>());
                for (short c = 0; c < colLength; c++)
                    matrix[r].Add(-1);
            }
            return matrix;
        }

        private List<int> get_EssentialPrimes(ref List<List<int>> primeChart, ref List<int> colsPrimeChart)
        {
            List<int> essentialPrimes = new List<int>();
            HashSet<int> removeCols = new HashSet<int>();
            HashSet<int> removeRows = new HashSet<int>();
            bool isSpRow;
            bool isSpCol;
            bool isSpNonSinglePrime;
            int count = 0;
            while (true)
            {
                isSpRow = check_RowDominance(ref primeChart, ref removeCols, ref removeRows);
                isSpCol = check_ColDominance(ref primeChart, ref removeCols, ref removeRows);
                isSpNonSinglePrime = check_SinglePrimeInCol(ref primeChart, ref removeCols, ref removeRows, ref essentialPrimes);
                if (isSpRow && isSpCol && isSpNonSinglePrime) handle_SpecialCaseChart(ref primeChart, ref removeCols, ref removeRows, ref essentialPrimes);
                if (!check_LoopCondition(ref colsPrimeChart, ref removeCols)) break;
                if (count > 100)
                    throw new Exception("[ERROR] Qua nhieu tinh toan hoac vong lap vo han");
                count++;
            }
            return essentialPrimes;
        }

        private bool check_LoopCondition(ref List<int> colsPrimeChart, ref HashSet<int> removeCols)
        {
            for (int col = 0; col < colsPrimeChart.Count; col++)
                if (!removeCols.Contains(col)) return true;
            return false;
        }

        private void handle_SpecialCaseChart(ref List<List<int>> primeChart, ref HashSet<int> removeCols, ref HashSet<int> removeRows, ref List<int> essentialPrimes)
        {
            List<List<int>> rowFirstPrimeChart = transform_ColFirstPrimeChartToRowFirst(ref primeChart);
            int currCount = 0;
            int prevCount = 0;
            int rowToRemove = 0;
            int prime = 0;
            for (int row = 0; row < rowFirstPrimeChart.Count; row++)
            {
                if (removeRows.Contains(row))
                    continue;
                for (int col = 0; col < rowFirstPrimeChart[row].Count; col++)
                {
                    if (rowFirstPrimeChart[row][col] != -1 && !removeCols.Contains(col))
                    {
                        currCount++;
                        prime = rowFirstPrimeChart[row][col];
                    }
                }
                if (prevCount < currCount)
                {
                    prevCount = currCount;
                    rowToRemove = prime;
                }
                currCount = 0;
            }
            display_Result("[INFO] Tinh toan truong hop dac biet khi khong the loai bo row/column");
            essentialPrimes.Add(rowToRemove);
            remove_ColInPrimeChart(ref primeChart, rowToRemove, ref removeCols, ref removeRows);
            removeRows.Add(rowToRemove);
        }

        private bool check_SinglePrimeInCol(ref List<List<int>> primeChart, ref HashSet<int> removeCols, ref HashSet<int> removeRows, ref List<int> essentialPrimes)
        {
            bool isSpecialCase = true;
            int count = 0;
            int foundRow = 0;
            for (int col = 0; col < primeChart.Count; col++)
            {
                if (removeCols.Contains(col))
                    continue;
                for (int row = 0; row < primeChart[col].Count; row++)
                {
                    if (primeChart[col][row] != -1 && !removeRows.Contains(primeChart[col][row]))
                    {
                        count++;
                        foundRow = primeChart[col][row];
                    }
                }
                if (count == 1)
                {
                    essentialPrimes.Add(foundRow);
                    remove_ColInPrimeChart(ref primeChart, foundRow, ref removeCols, ref removeRows);
                    removeRows.Add(foundRow);
                    isSpecialCase = false;
                }
                count = 0;
            }
            return isSpecialCase;
        }

        private void remove_ColInPrimeChart(ref List<List<int>> primeChart, int compareValue, ref HashSet<int> removeCols, ref HashSet<int> removeRows)
        {
            for (int col = 0; col < primeChart.Count; col++)
            {
                if (removeCols.Contains(col))
                    continue;
                for (int row = 0; row < primeChart[col].Count; row++)
                {
                    if (primeChart[col][row] == compareValue && !removeRows.Contains(row))
                        removeCols.Add(col);
                }
            }
        }

        private bool check_ColDominance(ref List<List<int>> primeChart, ref HashSet<int> removeCols, ref HashSet<int> removeRows)
        {
            bool isSpecialCase = true;
            List<int> currColCheckPos = new List<int>();
            List<int> cmpColCheckPos = new List<int>();
            for (int currCol = 0; currCol < primeChart.Count; currCol++)
            {
                for (int cmpCol = 0; cmpCol < primeChart.Count; cmpCol++)
                {
                    if (currCol == cmpCol || removeCols.Contains(currCol) || removeCols.Contains(cmpCol))
                        continue;
                    for (int row = 0; row < primeChart[currCol].Count; row++)
                    {
                        if (primeChart[currCol][row] != -1 && !removeRows.Contains(row))
                            currColCheckPos.Add(row);
                        if (primeChart[cmpCol][row] != -1 && !removeRows.Contains(row))
                            cmpColCheckPos.Add(row);
                    }
                    int count = 0;
                    if (currColCheckPos.Count > cmpColCheckPos.Count)
                    {
                        foreach (int rowCheckIdx in cmpColCheckPos)
                            if (primeChart[currCol][rowCheckIdx] != -1
                                && !removeRows.Contains(primeChart[currCol][rowCheckIdx]))
                                count++;
                        if (count >= cmpColCheckPos.Count)
                        {
                            removeCols.Add(currCol);
                            isSpecialCase = false;
                        }
                    }
                    else if (currColCheckPos.Count < cmpColCheckPos.Count)
                    {
                        foreach (int rowCheckIdx in currColCheckPos)
                            if (primeChart[cmpCol][rowCheckIdx] != -1
                                && removeRows.Contains(primeChart[cmpCol][rowCheckIdx]))
                                count++;
                        if (count >= currColCheckPos.Count)
                        {
                            removeCols.Add(cmpCol);
                            isSpecialCase = false;
                        }
                    }
                    currColCheckPos.Clear();
                    cmpColCheckPos.Clear();
                }
            }
            if (!isSpecialCase)
                display_Result("[INFO] Loai bo column");
            return isSpecialCase;
        }

        private bool check_RowDominance(ref List<List<int>> primeChart, ref HashSet<int> removeCols, ref HashSet<int> removeRows)
        {
            bool isSpecialCase = true;
            List<int> currRowCheckPos = new List<int>();
            List<int> cmpRowCheckPos = new List<int>();
            List<List<int>> rowFirstPrimeChart = transform_ColFirstPrimeChartToRowFirst(ref primeChart);
            for (int currRow = 0; currRow < rowFirstPrimeChart.Count; currRow++)
            {
                for (int cmpRow = 0; cmpRow < rowFirstPrimeChart.Count; cmpRow++)
                {
                    if (currRow == cmpRow || removeRows.Contains(cmpRow) || removeRows.Contains(currRow))
                        continue;
                    for (int col = 0; col < rowFirstPrimeChart[currRow].Count; col++)
                    {
                        if (rowFirstPrimeChart[currRow][col] != -1 && !removeCols.Contains(col))
                            currRowCheckPos.Add(col);
                        if (rowFirstPrimeChart[cmpRow][col] != -1 && !removeCols.Contains(col))
                            cmpRowCheckPos.Add(col);
                    }
                    int count = 0;
                    if (currRowCheckPos.Count > cmpRowCheckPos.Count)
                    {
                        foreach (int colCheckIdx in cmpRowCheckPos)
                            if (rowFirstPrimeChart[currRow][colCheckIdx] != -1
                                && !removeRows.Contains(rowFirstPrimeChart[currRow][colCheckIdx]))
                                count++;
                        if (count >= cmpRowCheckPos.Count)
                        {
                            removeRows.Add(cmpRow);
                            isSpecialCase = false;
                        }
                    }
                    else if (currRowCheckPos.Count < cmpRowCheckPos.Count)
                    {
                        foreach (int colCheckIdx in currRowCheckPos)
                            if (rowFirstPrimeChart[cmpRow][colCheckIdx] != -1
                                && !removeRows.Contains(rowFirstPrimeChart[cmpRow][colCheckIdx]))
                                count++;
                        if (count >= currRowCheckPos.Count)
                        {
                            removeRows.Add(currRow);
                            isSpecialCase = false;
                        }
                    }
                    currRowCheckPos.Clear();
                    cmpRowCheckPos.Clear();
                }
            }
            if (!isSpecialCase)
                display_Result("[INFO] Loai bo row");
            return isSpecialCase;
        }

        private List<List<int>> transform_ColFirstPrimeChartToRowFirst(ref List<List<int>> primeChart)
        {
            List<List<int>> newPrimeChartRowFirst = new List<List<int>>();
            for (int i = 0; i < primeChart[0].Count; i++)
            {
                newPrimeChartRowFirst.Add(new List<int>());
                for (int j = 0; j < primeChart.Count; j++)
                {
                    newPrimeChartRowFirst[i].Add(primeChart[j][i]);
                }
            }
            return newPrimeChartRowFirst;
        }

        private (string, string) get_FormattedPrimeString(PrimeImplicant obj)
        {
            string numbListString = String.Join(",", obj.numbList);
            string bits = "";
            int shift = inputVariables.Count - 1;
            for (int i = 0; i < inputVariables.Count; i++)
            {
                int currBitCheck = (obj.valueBin >> shift) & 1;
                int currDashBitCheck = (obj.dashBin >> shift) & 1;
                char currChar = inputVariables[i];
                if (currBitCheck == 1 && currDashBitCheck != 1) bits += currChar;
                else if (currBitCheck != 1 && currDashBitCheck != 1) bits += currChar + "'";
                shift--;
            }
            if (bits.Length <= 0) bits = "1";
            return (numbListString, bits);
        }

        private void main_QuineMcCluskeyAlgo()
        {
            // Sorting
            inputVals.AddRange(inputDontCares);
            inputVals.Sort();
            display_Inputs(inputVals);
            // Main
            Dictionary<int, List<PrimeImplicant>> groups = new Dictionary<int, List<PrimeImplicant>>();
            arrange_InputGroup(groups);
            int maxNumbBitLength = get_BinaryLengthOfNumb(inputVals[inputVals.Count - 1]);
            int iteration = 0;
            while (true)
            {
                Dictionary<int, List<PrimeImplicant>> newGroups = find_NextPrimeGroups(groups, maxNumbBitLength);
                if (check_GroupsEquality(ref newGroups, ref groups)) break;
                groups = clear_PrimeMatchStatusInGroups(newGroups);
                display_Result($"[INFO] Sau lan lap thu: {iteration + 1}");
                iteration++;
                if (iteration >= 100)
                    throw new Exception("[ERROR] Qua nhieu tinh toan hoac vong lap vo han");
            }
            List<PrimeImplicant> finalPrimeList = new List<PrimeImplicant>();
            finalPrimeList = get_PrimesNoDupInGroups(ref groups);
            display_Result("[INFO] Cac gia tri tich cuc tieu sau khi bat cap");
            foreach (PrimeImplicant prime in finalPrimeList)
            {
                (string numbsList, string variables) = get_FormattedPrimeString(prime);
                display_Result($"{numbsList} - {variables}");
            }
            display_Result("[INFO] Lap bang de rut gon: ");
            (List<List<int>> primeChart, List<int> colsOfPrimeChart) = create_PrimeChart(finalPrimeList);
            List<int> essentialPrimeListIdx = get_EssentialPrimes(ref primeChart, ref colsOfPrimeChart);
            display_Result("[INFO] Bieu thuc rut gon Y: ");
            string numbsListShowCase = "Cac cap gia tri: ";
            string variablesShowCase = "Y = ";
            for (int idx = 0; idx < essentialPrimeListIdx.Count; idx++)
            {
                (string numbsList, string variables) = get_FormattedPrimeString(finalPrimeList[essentialPrimeListIdx[idx]]);
                numbsListShowCase += numbsList;
                variablesShowCase += variables;
                if (idx + 1 < essentialPrimeListIdx.Count)
                {
                    numbsListShowCase += " - ";
                    variablesShowCase += " + ";
                }
            }
            display_Result(numbsListShowCase);
            display_Result(variablesShowCase);
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