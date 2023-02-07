using System;
using System.Collections.Generic;
using System.Linq;

namespace S_DES.Implementation
{
    public abstract class Encryption : EncryptionRule
    {
        private static bool _status;
        private protected readonly List<byte> FirstKey, SecondKey;
        private string _text;

        static Encryption()
        {
            _status = true;
        }

        public Encryption(string originalKey, string text)
        {
            this._text = text;
            var key = ConvertToBitList(originalKey);
            
            this.FirstKey = GenerateKey(key);
            this.SecondKey = GenerateKey(key);
        }

        private List<byte> ConvertToBitList(string String)
        {
            List<byte> bitList = new();
            foreach (var bit in String)
            {
                bitList.Add(Convert.ToByte(bit));
            }

            return bitList;
        }

        private List<byte> GenerateKey(List<byte> key)
        {
            return ProcessKeyWithP8(ShiftBitsLeft(ProcessKeyWithP10(key)));
        }

        private List<byte> ProcessKeyWithP10(List<byte> key)
        {
            return ProcessBitsAccordingToRule(P10, key, (byte)P10.Count);
        }

        private List<byte> ProcessKeyWithP8(List<byte> key)
        {
            return ProcessBitsAccordingToRule(P8, key, (byte)P8.Count);
        }

        private List<byte> ProcessBitsAccordingToRule(List<byte> rule,List<byte> key, byte border)
        {
            List<byte> newKey = new();
            for (byte i = 0; i < border; i++)
            {
                newKey.Add(key[rule[i] - 1]);
            }
            
            return newKey;
        }

        /*циклический сдвиг битов влево*/
        private List<byte> ShiftBitsLeft(List<byte> key)
        {
            byte step = Convert.ToByte((_status) ? 1 : 3);
            _status = _status!;
            
            List<byte> updatedKey = new();
            for (byte i = 0; i < key.Count/2; i++)
            {
                updatedKey.Add(key[i + step < (key.Count/2) ? i + step : i + step - (key.Count / 2)]);
            }

            for (byte i = Convert.ToByte(key.Count/2); i < key.Count; i++)
            {
                updatedKey.Add(key[i + step < key.Count ? i + step : i + step - (key.Count / 2)]);
            }

            return updatedKey;
        }

        public string GetText()
        {
            var encryptMessage = "";
            foreach (var letter in this._text.Select(ProcessSymbol).ToList())
            {
                encryptMessage += ConvertLetterBits(letter);
            }

            return encryptMessage;
        }


        private protected char ConvertLetterBits(List<byte> letter)
        {
            var encryptString = "";
            foreach (var letterBit in letter)
            {
                encryptString += letterBit.Equals(48) ? '1' : '0';
            }

            return (char)Convert.ToInt32(encryptString, 2);
        }

        
        private List<List<byte>> ConvertSymbolToBinary(char symbol)
        {
            List<List<byte>> bitSymbol = new();
            var bitSymbolString = Convert.ToString(symbol, 2).PadLeft(16,'0');
            bitSymbol.AddRange(new []
            {
                ConvertToBitList(bitSymbolString.Substring(0, 8)),
                ConvertToBitList(bitSymbolString.Substring(8))
            });
            return bitSymbol;
        }

        /*Начальная перестановка Ip*/
        private List<byte> MakeInitialChange(List<byte> bitPart)
        {
            return ProcessBitsAccordingToRule(Ip, bitPart, (byte) Ip.Count);
        }
        
        
        /*Конечная перестановка Ip-1*/
        private List<byte> MakeFinalChange(List<byte> bitPart)
        {
            return ProcessBitsAccordingToRule(Ip1, bitPart, (byte) Ip1.Count);
        }

        /*Рашсирение Ep*/
        private List<byte> ExpandBitParts(List<byte> bitPart)
        {
            return ProcessBitsAccordingToRule(Ep, bitPart, (byte) Ep.Count);
        }

        /*Исключающее ИЛИ*/
        private List<byte> PerformXorOperation(List<byte> firstValue, List<byte> secondValue)
        {
            var resultValue = new List<byte>();
            for (var i = 0; i < firstValue.Count; i++)
            {
                resultValue.Add(Convert.ToByte(firstValue[i] ^ secondValue[i]));
            }

            return resultValue;
        }

        /*S-блоки*/
        private List<byte> GenerateSBlockPart(List<List<byte>> blockTemplate, List<byte> block)
        {
            var Bit = blockTemplate[GenerateFloatNumber(block[0], block[3])][GenerateFloatNumber(block[1], block[2])];
            var stringBinaryBit = Convert.ToString(Bit, 2);
            return ConvertToBitList(stringBinaryBit.PadLeft(2, '0'));
        }
        
        private byte GenerateFloatNumber(byte firstBit, byte secondBit)
        {
            var stringFloatNumber = Convert.ToByte(firstBit).ToString() + Convert.ToByte(secondBit);
            return Convert.ToByte(stringFloatNumber, 2);
        }

        private List<byte> CreateSBlock(List<byte> bits)
        {
            var splitBits = BreakBitesApart(bits);
            var leftSBlock = GenerateSBlockPart(FirstSBlock, splitBits[0]);
            var rightSBlock = GenerateSBlockPart(SecondSBlock, splitBits[1]);
            return RearrangeBitParts(new List<List<byte>>()
            {
                leftSBlock, rightSBlock
            });
        }

        private List<byte> ProcessSBlockAccordingToP4Rule(List<byte> sBlock)
        {
            return ProcessBitsAccordingToRule(P4, sBlock, (byte) P4.Count);
        }

        private List<byte> CompleteRound(List<byte> key, List<byte> bits)
        {
            var splitBits = BreakBitesApart(bits);
            return RearrangeBitParts(new List<List<byte>>()
            {
                PerformXorOperation(
                    ProcessSBlockAccordingToP4Rule(
                        CreateSBlock(
                            PerformXorOperation(
                                ExpandBitParts(splitBits[1]), key))), splitBits[0]), 
                splitBits[1]
            });
        }

        /*Разбиение битов*/
        private List<List<byte>> BreakBitesApart(List<byte> bits)
        {
            var splitBits = new List<List<byte>>();
            for (var i = 0; i < 2; i++)
            {
                var splitBitsColumn = new List<byte>();
                for (var j = 0; j < bits.Count/2; j++)
                {
                    splitBitsColumn.Add(bits[i * (bits.Count / 2) + j]);
                }
                splitBits.Add(splitBitsColumn);
            }

            return splitBits;
        }

        /*Перестановка битов*/
        private List<byte> RearrangeBitParts(List<List<byte>> matrix)
        {
            var rearrangeBitPartsList = new List<byte>();
            foreach (var column in matrix)
            {
                foreach (var element in column)
                {
                    rearrangeBitPartsList.Add(element);
                }
            }

            return rearrangeBitPartsList;
        }

        private protected List<byte> ProcessSymbolWithAlgorithm(char symbol, List<byte> firstKey, List<byte> secondKey)
        {
            List<byte> cipherSymbol = new();
            foreach (var bitPart in ConvertSymbolToBinary(symbol))
            {
                cipherSymbol.AddRange(
                    MakeFinalChange(
                        CompleteRound(secondKey,
                            RearrangeBitParts(
                                BreakBitesApart(
                                    CompleteRound(firstKey,
                                        MakeInitialChange(bitPart)))))));
            }

            return cipherSymbol;
        }

        private protected abstract List<byte> ProcessSymbol(char symbol);
    }
}