using System.Collections.Generic;

namespace S_DES.Implementation
{
    public class Encoder : Encryption
    {
        public Encoder(string originalKey, string text) : base(originalKey, text) { }
        private protected override List<byte> ProcessSymbol(char symbol)
        {
            return ProcessSymbolWithAlgorithm(symbol, this.FirstKey, this.SecondKey);
        }

        
    }
}