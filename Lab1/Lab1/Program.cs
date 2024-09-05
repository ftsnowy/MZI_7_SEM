namespace Lab1;

using System;

public class Program
{
    
    private const String inputPath = "D:\\BSUIR\\7_SEM\\MZI\\Lab1\\Lab1\\input.txt";
    private const String outputPath = "D:\\BSUIR\\7_SEM\\MZI\\Lab1\\Lab1\\output.txt";

    static void Main()
    {
        Encoder encoder = new Encoder();
        Decoder decoder = new Decoder();
        decoder.Start();
    }
}

