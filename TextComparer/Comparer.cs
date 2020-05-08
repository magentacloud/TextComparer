using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TextComparer
{
    class Comparer
    {
        private string referencePath;
        private string resultPath;
        private string[] referenceArray;
        private string[] resultArray;
        private int code;
        public class Parameters
        {
            public int? initialLineIndex;
            public int? terminalLineIndex;
            public List<int> passedLineIndexes;
        }
        public void Process()
        {
            Console.WriteLine("Введите путь к эталонному файлу");
            this.referencePath = Console.ReadLine();
            referenceArray = this.ReadFile(referencePath);
            Console.WriteLine("Введите путь к результирующему файлу");
            this.resultPath = Console.ReadLine();
            resultArray = this.ReadFile(resultPath);
            Parameters parameters = null;
            while(parameters == null)
            {
                parameters = SetParameters();
            }
            code = this.CompareTexts(referenceArray, resultArray, parameters);
            if(code == -1)
            {
                Console.WriteLine("Файлы идентичны");
            }
            else if(code == -2)
            {
                Console.WriteLine("В файлах разное количество строк");
            }
            else
            {
                Console.WriteLine($"Расхождение в строке {code}\nСтрока в эталонном файле:\n{referenceArray[code]}\nСтрока в результирующем файле:\n{resultArray[code]}");
            }
        }
        private string[] ReadFile(string path)
        {
            string[] res = null;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    String line = sr.ReadToEnd();
                    res = line.Split('\n');
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Невозможно считать файл\n");
            }
            return res;
        }

        private Parameters SetParameters()
        {
            Parameters parameters = new Parameters();
            Console.WriteLine("Введите номер строки, с которой следует начинать сравнение.");
            string l = Console.ReadLine();
            if (l != "")
            {
                try
                {
                    parameters.initialLineIndex = Int32.Parse(l);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неправильный номер");
                    return null;
                }
            }
            Console.WriteLine("Введите номер строки, после которой следует остановить сравнение.");
            l = Console.ReadLine();
            if (l != "")
            {
                try
                {
                    parameters.terminalLineIndex = Int32.Parse(l);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неправильный номер");
                    return null;
                }
            }
            if(parameters.initialLineIndex != null && parameters.terminalLineIndex != null)
            {
                if (parameters.initialLineIndex > parameters.terminalLineIndex)
                {
                    Console.WriteLine("Номер строки, с которой начинать сравнение больше, чем номер строки, после которой сравнение остановится");
                    return null;
                }
            }
            Console.WriteLine("Введите через запятую номера строк, которые необходимо исключить из сравнения.");
            l = Console.ReadLine();
            if (l != "")
            {
                string[] temp = l.Split(',');
                List<int> indexes = new List<int>();
                int i = 0;
                while (i < temp.Length)
                {
                    try
                    {
                        indexes.Add(Int32.Parse(temp[i]));
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Неправильный номер");
                        return null;
                    }
                    i++;
                }
                parameters.passedLineIndexes = indexes;
            }
            return parameters;
        }
        private int CompareTexts(string[] reference, string[] result, Parameters parameters)
            // Возвращает номер строки, на которой находится первое расхождение. Если расхождений нет, возвращает -1. Если в текстах разное количество строк, возвращается -2
        {
            if(reference.Length != result.Length)
            {
                return -2;
            }
            int initial, terminal;
            if (parameters.initialLineIndex == null)
            {
                initial = 0;
            }
            else
            {
                initial = (int)parameters.initialLineIndex;
            }
            if(parameters.terminalLineIndex == null){
                terminal = reference.Length;
            }
            else
            {
                terminal = (int)parameters.terminalLineIndex;
            }
            for (int i = initial; i < terminal; i++)
            {
                if (parameters.passedLineIndexes != null)
                {
                    if (parameters.passedLineIndexes.Contains(i))
                    {
                        continue;
                    }
                }
                char[] referenceArr = reference[i].ToCharArray();
                char[] resultArr = result[i].ToCharArray();
                bool isAsterisk = false;
                char next = ' ';
                int k = 0;
                int counter = 0;
                for (int j = 0; j < referenceArr.Length; j++)
                    {
                        if (referenceArr[k] == '*')
                        {
                            isAsterisk = true;
                            next = referenceArr[k + 1];
                            k++;
                        }
                        else if (isAsterisk && resultArr[j] == next)
                        {
                            isAsterisk = false;
                        k = j + 1;
                         }
                        else if (isAsterisk)
                        {
                            counter++;
                        }
                         else if (referenceArr[k - counter] != resultArr[j] && !isAsterisk)
                        {
                            return i;
                        }
                        else
                        {
                        k = j + 1;
                        }
                    }
            }
            return -1;
        }
    }
}
