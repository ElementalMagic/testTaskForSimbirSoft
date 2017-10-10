using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConsoleApplication21
{   // Класс файла_с_словарем.
    // При инициализации объекта класса Slovar указывается его полная директория
    // относительно исполняемого файла программы, и исполняется метод GetRegex().
    internal class Slovar
    {
        public string path { get; set; }
        public List<Regex> shablon = new List<Regex>();
        public List<Regex> SecondShablon = new List<Regex>();
        private Encoding enc = Encoding.GetEncoding(1251);
        private int countstrings;

        public Slovar()
        {
            try
            {
                path = @"словарь.txt";
                GetRegex();
                if (countstrings > 100000)
                {
                    Console.WriteLine("Количество строк словаря не должно превышать ста тысяч");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
                FileInfo file = new FileInfo(path);
                if (file.Length > 1024 * 1024 * 2)
                {
                    Console.WriteLine("Входной размер текст.txt не может быть больше 2 МБ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            catch
            {
                Console.WriteLine("Файл с текстом должен находиться в директории программы" +
                                  "\nФайл должен быть доступен для чтения" +
                                  "\nИмя файла должно быть: словарь.txt");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        // Метод GetRegex() служит для построчного считывания файла словарь.txt
        // В list<regex> shablon записываются регулярные выражение для будущего
        // парса слов файла_с_текстом и нахождения совпадений по форме.
        // List<regex> SecondShablon содержит в себе построчносчитанные,
        // неизмененные данные из словарь.txt.
        public void GetRegex()
        {
            try
            {
                StreamReader reader = new StreamReader(path, enc);
                while (!reader.EndOfStream)
                {
                    countstrings++;
                    string slovo = reader.ReadLine();
                    shablon.Add(new Regex(@"^\W?" + slovo + @"\W?$", RegexOptions.IgnoreCase));
                    SecondShablon.Add(new Regex(slovo, RegexOptions.IgnoreCase));
                }
                reader.Close();
            }
            catch
            {
                Console.WriteLine("В файле словаря обнаружены недопустимые символы");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }
    }

    // Класс файла_с_текстом.
    // При инициализации объекта класса Text указывается его полная директория
    // относительно испольняемого файла программы и исполняется метод ToCountStrings().
    internal class Text
    {
        public string path { get; set; }
        public int countstrings;

        public Text()
        {
            try
            {
                path = @"текст.txt";
                ToCountStrings();
                FileInfo file = new FileInfo(path);
                if (file.Length > 1024 * 1024 * 2)
                {
                    Console.WriteLine("Входной размер текст.txt не может быть больше 2 МБ");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            catch
            {
                Console.WriteLine(
                    "Файл с текстом должен находиться в директории программы\n" +
                    "Файл должен быть доступен для чтения\n" +
                    "Имя файла должно быть: словарь.txt");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        // Определние количества строк в файле с текстом.
        private void ToCountStrings()
        {
            StreamReader read = new StreamReader(path, Encoding.GetEncoding(1251));
            while (!read.EndOfStream)
            {
                countstrings++;
                read.ReadLine();
            }
            read.Close();
        }
    }

    internal class Program
    {
        private static int count = 0;
        private static int countprocess = Environment.ProcessorCount;
        private static Text text = new Text();
        private static Slovar slovar = new Slovar();
        private static bool[] flags = new bool[countprocess];
        private static string[][] textArray = new string[countprocess][];
        private static int shag = text.countstrings / countprocess;
        private static int ostatok = text.countstrings - (shag * countprocess);
        private static int global = 1000;
        private static int local = 0;
        private static int NumName = 0;
        private static bool gotoCheck = false;
        private static string MainFileName;
        private static string FileName;

        private static void Main(string[] args)
        {
            Console.Title = "Тестовое задание";
            Regex Fname = new Regex(@"[^\W\s]*");
            Console.Write("Введите название конечного файла: ");
            f:
            string k = Console.ReadLine();
            if (k.Contains(" "))
            {
                Console.Clear();
                Console.Write("В названии не должно быть пробелов! Повторите:");
                goto f;
            }
            MainFileName = Fname.Match(k).ToString();

            if (MainFileName != "") { }
            else {
                Console.Clear(); Console.WriteLine("В имени допускаются только цифры, буквы, а также знак нижнего подчеркивания!" +
                                                   "\nПовторите попытку:\n");
                goto f;
            }
            if (Directory.Exists(@"Результат работы\" + MainFileName))
            {
                Console.WriteLine("Директория с данным именем уже существует.\n" +
                                  "\nВы уверены, что хотите перезаписать файл?\n" +
                                  "Нажмите соответствующую клавишу на клавиатуре\n" +
                                  "Yes \\ No\n");
                f3:
                ConsoleKeyInfo key = Console.ReadKey(true);
                if ((key.Key != ConsoleKey.Y) && (key.Key != ConsoleKey.N))
                {
                    goto f3;
                }
                if (key.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    Console.Write("Введине название конечного файла: ");
                    goto f;
                }
                DirectoryInfo direc = new DirectoryInfo(@"Результат работы\" + MainFileName);
                Regex next = new Regex(MainFileName, RegexOptions.IgnoreCase);
                foreach (FileInfo currentFile in direc.GetFiles())
                {
                    if (next.IsMatch(currentFile.Name)) currentFile.Delete();
                }
            }
            f2:
            Console.Write("Введите ограничение на кол-во строк в одном html файле ОТ 10 ДО 100000 : ");
            f4:
            try
            {
                global = Int32.Parse(Console.ReadLine());
                if (global < 10 | global > 100000)
                {
                    Console.WriteLine("Введенное число находится вне диапозона (10;100000)!\n");
                    goto f2;
                }
            }
            catch
            {
                Console.Write("Вам требуется ввести натурльное число! Повторите: ");
                goto f4;
            }
            Directory.CreateDirectory(@"Результат работы\" + MainFileName + @"\");
            FileName = MainFileName.ToLowerInvariant() + ".часть";
            ProcessWork();
            while (flags[countprocess - 1] != true) { }
            Common();
            Console.WriteLine("Работа завершена. Нажмите любую клавишу для выхода..");
            Console.ReadKey();
        }

        // Метод Common() объединяет ссылки на отформатированные html-файлы
        // и создает новый html-файл,
        // при запуске которого загружается
        // отформатированный текст пользователя.
        private static void Common()
        {
            FileStream file = new FileStream(@"Результат работы\" + MainFileName + @"\" + MainFileName + ".html", FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine("<meta charset=\"utf-8\">\n<script src =\"http://code.jquery.com/jquery-1.9.1.js \"></script>");
            for (int i = 0; i <= NumName; i++)
            {
                writer.WriteLine("<div id=\"{1}{0}\"></div>", i, "result");
            }
            writer.WriteLine("<script>");
            for (int i = 0; i <= NumName; i++)
            {
                writer.WriteLine("$(\"#{1}{0}\").load(\"{2}{0}.html\");", i, "result", FileName);
            }
            writer.WriteLine("</script>");
            writer.Close();
        }

        // Метод ProcessWork() создает N-ое кол-во потоков,
        // зависящее от кол-ва логических ядер пользователя.
        // Каждый поток исполняет метод CreateProcess().
        // Текст из файла_с_текстом разбивается на N-ое кол-во составляющих.
        // Остаток, от деления Кол-ва строк на некратное число,
        // добавляется к последнему процессу.
        // В имя каждого потока передается
        // Начальный и Конечный индексы строк файла_с_текстом,
        // А также его порядковый номер.
        private static void ProcessWork()
        {
            int StartIndex = 0;
            int EndIndex = shag;
            if (text.countstrings < 800)
            {
                Thread potok = new Thread(CreateProcess);
                potok.Name = string.Format("{0},{1},{2}", 0, text.countstrings, 0);
                potok.Start();
            }
            else
            {
                Thread[] threads = new Thread[countprocess];
                for (int i = 0; i < countprocess; i++)
                {
                    threads[i] = new Thread(delegate ()
                    {
                        CreateProcess();
                    });
                    threads[i].Name = string.Format("{0},{1},{2}", StartIndex, EndIndex, i);
                    threads[i].Start();
                    StartIndex = EndIndex;
                    EndIndex += shag;
                    if (i == (countprocess - 2))
                    {
                        EndIndex =EndIndex+ostatok;
                    }
                    Console.WriteLine("Поток №{0} запущен", (i+1));
                }
            }
        }

        // Метод CreateProcess() служит для формирования и передачи массива,
        // содеражищй строки отформатированного текста, в метод WriteToFile.
        // Начальный и конечные индексы считывания строк из файла_с_текстом
        // передаются методу из имени потока, который его реализует.
        // Каждый поток обладает своим массивом с индексом, который передается
        // ему в имени "ProcessNumber". Сам цикл форматирования -
        // Каждый элемент строки, отделенный от другого пробелом
        // определяется в переменную "slovo", далее идет перебор по формам из словаря
        // и при совпадении, создается новая строка "zamena", в которую слева и справа
        // вставляется необходимый html код, посередине - первое вхождение формы
        // в строке "slovo". Далее, определяется новая строковая переменная "slovonew",
        // в которой все вхождения неизмененной формы "SecondShablon" в строке "slovo"
        // меняются на значение переменной "zamena". После, значение переменной "slovonew"
        // прибавляется к значению переменной "stringforWriting".
        // "stringforWriting" записывается в массив потока под порядковым индексом "textArrayIndex".
        // Данные о начальном, конечном индексах, порядковый номер потока
        // и кол-во элементов его массива передаются в метод WriteToFile().
        private static void CreateProcess()
        {
            string[] index = Thread.CurrentThread.Name.Split(',');
            int ProcessNumber = Convert.ToInt32(index[2]);
            int StartIndex = Convert.ToInt32(index[0]);
            int EndIndex = Convert.ToInt32(index[1]);
            int ArrayLength = (EndIndex - StartIndex);
            textArray[ProcessNumber] = new string[ArrayLength];
            flags[ProcessNumber] = false;
            List<Regex> reg = slovar.shablon;
            StreamReader read = new StreamReader(text.path, Encoding.GetEncoding(1251));
            int textArrayIndex = 0;
            for (int i = 0; i < StartIndex; i++)
            {
                read.ReadLine();
            }
            for (int i = StartIndex; i < EndIndex; i++)
            {
                try
                {
                    string[] stroka = read.ReadLine().Split(' ');

                    string stringforWriting = String.Empty;
                    int in2 = 0;
                    foreach (string slovo in stroka)
                    {
                        bool checkFlag = false;

                        int countReg = 0;
                        foreach (Regex regex in reg)
                        {
                            Interlocked.Increment(ref count);
                            if (regex.IsMatch(slovo))
                            {
                                string zamena =
                                    "<span style=\"font-weight: bold; font-style: italic; font-color:#434313;\">" +
                                    slovar.SecondShablon[countReg].Match(slovo) + "</span>";
                                string slovonew = slovar.SecondShablon[countReg].Replace(slovo, zamena);
                                if (in2 == 0)
                                {
                                    stringforWriting += slovonew;
                                }
                                else
                                {
                                    stringforWriting += " " + slovonew;
                                }
                                checkFlag = true;
                                in2++;
                                break;
                            }
                            countReg++;
                        }
                        if (checkFlag == false)
                        {
                            if (in2 == 0)
                            {
                                stringforWriting += slovo;
                            }
                            else
                            {
                                stringforWriting += " " + slovo;
                            }
                            in2++;
                        }
                    }
                    textArray[ProcessNumber][textArrayIndex] = stringforWriting;
                    textArrayIndex++;
                }
                catch
                {
                    Console.WriteLine("Сработал catch");
                    string slovonew = string.Empty;
                    string slovo = read.ReadLine();
                    int countReg = 0;
                    foreach (Regex regex in reg)
                    {
                        Interlocked.Increment(ref count);
                        if (regex.IsMatch(slovo))
                        {
                            string zamena =
                                "<span style=\"font-weight: bold; font-style: italic; font-color:#434313;\">" +
                                slovar.SecondShablon[countReg].Match(slovo) + "</span>";
                            slovonew = slovar.SecondShablon[countReg].Replace(slovo, zamena);
                            break;
                        }
                        countReg++;
                    }
                    textArray[ProcessNumber][textArrayIndex] = slovonew;
                    textArrayIndex++;
                }
            }
            read.Close();
            if (ProcessNumber == 0)
            {
                WriteToFile(StartIndex, EndIndex, ProcessNumber, ArrayLength);
            }
            else
            {
                while (flags[ProcessNumber - 1] == false) { }
                if (flags[ProcessNumber - 1] == true) { WriteToFile(StartIndex, EndIndex, ProcessNumber, ArrayLength); }
            }
            flags[ProcessNumber] = true;
            Console.WriteLine("Поток №{0} завершил работу \n", (ProcessNumber+1));
        }

        // Метод WriteToFile() производит запись строк поступившего в него массива.
        // Тут же происходит создание опредленного кол-ва html файлов,
        // зависящего от строк в одном html файле.
        // Переменная "global" - кол-во строк в одном файле, которое
        // задал пользователь. Переменная "local" - кол-во записанных строк из "global"
        // на данный момент. Когда "local" достигает значение "global" создается новый html файл,
        // и значение переменной "local" приравнивается к 0.
        //
        private static void WriteToFile(int start, int end, int processnumber, int length)
        {
            int i2;
            int i = 0;
            if (NumName == 0) { i2 = 0; }
             else { i2 = 1; }
            

            start:
            FileStream file = new FileStream(@"Результат работы\" + MainFileName + @"\" + FileName + NumName + ".html", FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file);
            
            while (i<length)
            {
                GotoCheck();
                if (gotoCheck == true)
                {
                 writer.Close();  i2= 0; goto start; 
                }
                if (i2 == 0) writer.Write(textArray[processnumber][i] + "</br>");
                else
                {
                     
                    writer.Write("\n" + textArray[processnumber][i] + "</br>");
                }
                Interlocked.Increment(ref local);
                i2++;
                i++;
            }
            writer.Close();
            Array.Clear(textArray[processnumber], 0, length);
            Console.WriteLine("Записаны строки с {0} по {1}\nОбщая занятая память {2:###,###,###} Байт", start + 1,end + 1 , GC.GetTotalMemory(true));
        }

        // Метод GotoCheck() проверяет необходимость создания нового html файла
        // и, в зависимости от результата, устанавливает значение gotoCheck.
        private static void GotoCheck()
        {
            if (local == global)
            {
                gotoCheck = true;
                NumName++;
                local = 0;
            }
            else
            {
                gotoCheck = false;
            }
        }
    }
}