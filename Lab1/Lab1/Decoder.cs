namespace Lab1;

public class Decoder
{
    private const String inputPath = "D:\\BSUIR\\7_SEM\\MZI\\Lab1\\Lab1\\input.txt";
    private const String outputPath = "D:\\BSUIR\\7_SEM\\MZI\\Lab1\\Lab1\\output.txt";
    
    byte[,] perm = new byte[8, 16] // матрица перестановок
    {
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF },
        { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF }
    };
    
    uint[] key = [ 0x0123, 0x4567, 0x89AB, 0xCDEF, 0x0123, 0x4567, 0x89AB, 0xCDEF]; // ключ
    
    
    public Decoder() {}

    public void Start()
    {
        string[] data = ReadInput();
        
        string textInput = "";
        
        for (int i = 0; i < data.Length; i++)
        {
            int code = int.Parse(data[i]);
            textInput += (char)code;
        }
        
        Console.WriteLine("Text input: " + textInput);

        // Проверка на кратность 8
        if (data.Length % 8 != 0)
        {
            Console.WriteLine("Data can't be divided by 8 bits");
            Environment.Exit(0);
        }

        int blockSize = 8;
        int blockCount = data.Length / blockSize; // Количество блоков
        byte[][] blocks = new byte[blockCount][]; // Массив для блоков

        for (int i = 0; i < blockCount; i++)
        {
            blocks[i] = new byte[blockSize]; // Инициализация каждого блока
            for (int j = 0; j < blockSize; j++)
            {
                blocks[i][j] = byte.Parse(data[i * blockSize + j]); // Заполнение блока
            }
        }
        
        string outputData = "";
        
        // Основной цикл 
        for (int i = 0; i < blockCount; i++)
        {
            // Заполнение сумматоров N1 и N2
            uint n1 = 0, n2 = 0;
            for (int j = 0; j < 64; j++)
            {
                if (j < 32) // Первые 32 бита в n1
                {
                    n1 |= (uint)((blocks[i][j / 8] >> (7 - (j % 8)) & 1) << (31 - j));
                }
                else // Остальные 32 бита в n2
                {
                    n2 |= (uint)((blocks[i][j / 8] >> (7 - (j % 8)) & 1) << (63 - j));
                }
            }

            uint cm1 = 0, cm2 = 0;
            // 32 цикла замены
            for (int j = 0; j < 32; j++)
            {
                // Ищем индекс ключа на данном цикле
                int keyIndex = 0;
                if (j < 8)
                {
                    keyIndex = j % 8;
                }
                else
                {
                    keyIndex = 7 - (j % 8);
                }

                // Складываем n1 с key[keyIndex] в сумматоре cm1 по модулю 2^32
                cm1 = (n1 + key[keyIndex]) & 0xFFFFFFFF;

                // Прогоняем cm1 через таблицу замен

                cm1 = Substitution(cm1);

                // Сдвигаем данные в сумматоре на 11 влево
                cm1 = (cm1 << 1)|(cm1 >> 21);
                
                // Складываем сm1 и n2 в сумматоре cm2 по модулю 2
                cm2 = cm1 ^ n2;
                
                n2 = n1;
                n1 = cm2;

            }

            outputData += ConvertNToString(n1, n2);

        }
        
        WriteOutput(outputData);
        
        string textOutput = "";

        string currentCode = "";
        for (int i = 0; i < outputData.Length; i++)
        {
            if (outputData[i] == '\n')
            {
                int code = int.Parse(currentCode);
                textOutput += (char)code;
                currentCode = "";
            }
            else
            {
                currentCode += outputData[i];
            }
        }
        Console.WriteLine("Text output: " + textOutput);
        
    }

    public string[] ReadInput()
    {
        try
        {
            string[] content = File.ReadAllLines(inputPath);
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return [];
        }
    }
    public void WriteOutput(string data)
    {
        try
        {
            File.AppendAllText(outputPath, data + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи в файл: {ex.Message}");
        }
    }
    public string ConvertNToString(uint n1, uint n2)
    {
        ulong combined = ((ulong)n2 << 32) | n1;
        string answer = "";
        for (int i = 0; i < 8; i++) // 
        {
            byte byteValue = (byte)((combined >> (8 * (7 - i))) & 0xFF); // Извлечение байта
            answer += byteValue;
            answer += '\n';
        }

        return answer;
    }
    public uint Substitution(uint cm)
    {
        uint output = 0;
        for (int i = 0; i < 8; i++)
        {
            // Извлечение ниббла
            byte nibble = (byte)((cm >> (4 * (7 - i))) & 0x0F); // 4 бита
            // Замена по таблице
            byte substitutedNibble = perm[i, nibble];
            // Формирование результата
            output |= (uint)(substitutedNibble << (4 * (7 - i)));
        }

        return output;
    }
}