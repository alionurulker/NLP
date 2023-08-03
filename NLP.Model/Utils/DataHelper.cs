
using Newtonsoft.Json;
using System.Data;
using System.Text.RegularExpressions;

namespace NLP.Model.Utils
{
    public static class DataHelper
    {
        #region NLP

        private static readonly Dictionary<string, int> numberWords = new Dictionary<string, int>
        {
            {"sıfır", 0}, {"bir", 1}, {"iki", 2}, {"üç", 3}, {"dört", 4}, {"beş", 5},
            {"altı", 6}, {"yedi", 7}, {"sekiz", 8}, {"dokuz", 9}, {"on", 10},
            {"yirmi", 20}, {"otuz", 30}, {"kırk", 40}, {"elli", 50}, {"altmış", 60},
            {"yetmiş", 70}, {"seksen", 80}, {"doksan", 90}, {"yüz", 100},
            {"bin", 1000}, {"milyon", 1000000}, {"milyar", 1000000000}
        };

        public static readonly string[] turkishNumberWords = new string[] {
            "on", "sıfır", "bir", "iki", "üç", "dört", "beş", "altı", "yedi", "sekiz", "dokuz",
            "yirmi","otuz", "kırk", "elli","altmış", "yetmiş","seksen","doksan",
            "milyar", "milyon", "bin", "yüz"
        };

        public static string ConvertText(string text)
        {
            string[] words = text.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> convertedWords = new List<string>();
            List<string> separatedWords = new List<string>();
            List<string> separatedNumbers = new List<string>();
            bool isNumeric = false;
            int numericValue = 0;
            int tempIndex = 0;
            bool tempContainsTurkishNumberWords = false;
            bool tempContainsTurkishNumberWordsWithoutSpace = false;
            string tempContainsTurkishNumberWord = "";

            foreach (string word in words)
            {
                string numberWord = word;
                bool exists = Array.Exists(turkishNumberWords, element => numberWord.ToLower().Contains(element));

                if (tempContainsTurkishNumberWordsWithoutSpace && exists)
                {
                    string numericTextWithoutSpace = string.Join("", separatedNumbers);

                    string numericText = string.Join(" ", separatedNumbers);

                    int number = ConvertTextToNumber(numericText);
                    number += ConvertTextToNumber(numberWord);

                    text = text.Replace(numericTextWithoutSpace, number.ToString());
                    text = text.Replace(numberWord + " ", "");
                    return text;
                }
                else if (exists)
                {
                    tempContainsTurkishNumberWords = true;
                    for (int i = 0; i < turkishNumberWords.Length; i++)
                    {
                        bool containsTurkishNumberWords = numberWord.ToLower().Contains(turkishNumberWords[i]);
                        if (containsTurkishNumberWords)
                        {
                            if (tempIndex == 0)
                            {
                                string separatedWordNumber = numberWord.Substring(tempIndex, turkishNumberWords[i].Length);
                                if (!string.IsNullOrWhiteSpace(separatedWordNumber))
                                {
                                    separatedNumbers.Add(separatedWordNumber);
                                    tempIndex = turkishNumberWords[i].Length;

                                    numberWord = numberWord.Substring(tempIndex, numberWord.Length - tempIndex);
                                    i = 0;
                                }
                            }

                            int indexTurkishNumberWord = numberWord.IndexOf(turkishNumberWords[i]);
                            if (indexTurkishNumberWord > -1)
                            {
                                string separatedWordNumber = numberWord.Substring(0, indexTurkishNumberWord);
                                if (!string.IsNullOrWhiteSpace(separatedWordNumber))
                                {
                                    tempContainsTurkishNumberWordsWithoutSpace = true;
                                    separatedNumbers.Add(separatedWordNumber);
                                    tempIndex = indexTurkishNumberWord;

                                    numberWord = numberWord.Substring(tempIndex, numberWord.Length - tempIndex);
                                    i = 0;
                                }
                                if (indexTurkishNumberWord == 0)
                                {
                                    string separatedExtentionWordNumber = numberWord.Substring(0, turkishNumberWords[i].Length);
                                    separatedNumbers.Add(separatedExtentionWordNumber);
                                }
                            }
                        }
                    }
                }
                else if (tempContainsTurkishNumberWords)
                {
                    tempContainsTurkishNumberWords = false;
                    tempContainsTurkishNumberWordsWithoutSpace = false;

                    string numericTextWithSpace = string.Join(" ", separatedNumbers);
                    int numberWithSpace = ConvertTextToNumber(numericTextWithSpace);

                    string numericText = string.Join("", separatedNumbers);

                    if (text.Contains(numericText))
                    {
                        text = text.Replace(numericText, numberWithSpace.ToString());
                    }
                    else
                    {
                        text = text.Replace(numericTextWithSpace, numberWithSpace.ToString());
                    }

                    separatedNumbers.Clear();
                    tempContainsTurkishNumberWord = "";
                }


            }

            //string numericText = string.Join(" ", separatedNumbers);

            //int number = ConvertTextToNumber(numericText);

            //string replacedText = text.Replace(numericText, number.ToString());

            return text;
        }

        private static int ConvertTextToNumber(string text)
        {
            string[] words = text.ToLower().Split(' ');

            int number = 0;
            int tempNumber = 0;
            int multiplier = 1;

            foreach (string word in words)
            {
                if (numberWords.TryGetValue(word, out int value))
                {
                    if (value == 100)
                    {
                        if (tempNumber != 0)
                            tempNumber *= value;
                        else
                            tempNumber += value;
                    }
                    else if (value == 1000)
                    {
                        if (tempNumber != 0)
                            number += (tempNumber + number) * value;
                        else
                            number += value;
                        tempNumber = 0;
                    }
                    else
                    {
                        tempNumber += value;
                    }
                }
            }

            number += tempNumber;

            return number;
        }

        #endregion NLP
    }
}
