using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace S_DES.Implementation
{
    public class Program
    {
        
        private static Dictionary<Files, string> FileNames;

        private enum Files : byte
        {
            Original,
            Encrypt,
            Decrypt
        }

        static Program()
        {
            FileNames = new()
            {
                [Files.Original] = "original.txt",
                [Files.Encrypt] = "encrypt.txt",
                [Files.Decrypt] = "decrypt.txt"
            };
        }


        private static void Main(string[] args)
        {
            Console.Write("Введите текст: ");
            FileHandler.WriteToFile(FileNames[Files.Original], Console.ReadLine());
            string key = "", choice = "";
            
            while (true)
            {
                Console.Write("Введите значение ключа: ");
                if (CheckKey((key = Console.ReadLine())))
                {
                    break;
                }
                Console.WriteLine("Некорректный ввод! Необходимо ввести 10 символов в двоичном формате.");
            }

            var encoder = new Encoder(key, FileHandler.ReadFromFile(FileNames[Files.Original]));
            FileHandler.WriteToFile(FileNames[Files.Encrypt], encoder.GetText());
            
            var decoder = new Decoder(key, FileHandler.ReadFromFile(FileNames[Files.Encrypt]));
            FileHandler.WriteToFile(FileNames[Files.Decrypt], decoder.GetText());
            
            Console.WriteLine("Операция прошла успешно! Какой файл желаете просмотреть?");

            while (true)
            {
                Console.Write("Выберите файл:" +
                              " \n1.Оригинальный текст" +
                              "\n2.Зашифрованный текст" +
                              "\n3.Расшифрованный текст" +
                              "\n4.Выйти" +
                              "\nВаш выбор: ");
                if (CheckChoice(choice = Console.ReadLine()))
                {
                    switch (choice)
                    {
                        case "1":
                        {
                            FileHandler.OpenFile(FileNames[Files.Original]);
                            break;
                        }
                        case "2":
                        {
                            FileHandler.OpenFile(FileNames[Files.Encrypt]);
                            break;
                        }
                        case "3":
                        {
                            FileHandler.OpenFile(FileNames[Files.Decrypt]);
                            break;
                        }
                        case "4":
                        {
                            Process.GetCurrentProcess().Kill();
                            break;
                        }
                        default:
                        {
                            Console.WriteLine("Некорректный ввод!");
                            break;
                        }
                    }
                }
                
            }
        }

        private static bool CheckKey(string text)
        {
            bool check = false;
            if (text.Length == 10)
            {
                check = true;
                foreach (var symbol in text)
                {
                    if (symbol is not '1' && symbol is not '0')
                    {
                        check = false;
                        break;
                    }
                }
            }

            return check;
        }

        private static bool CheckChoice(string choice)
        {
            bool check = true;
            try
            {
                var digit = Convert.ToInt32(choice);
                Console.WriteLine(digit);
                if (digit < 0 && digit > FileNames.Count+1)
                {
                    check = false;
                }
            }
            catch (Exception)
            {
                check = false;
            }
            
            return check;
        }
    }
}