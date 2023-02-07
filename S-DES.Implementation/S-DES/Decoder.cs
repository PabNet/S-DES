using System.Collections.Generic;

namespace S_DES.Implementation
{
    public class Decoder : Encryption
    {
        public Decoder(string originalKey, string text) : base(originalKey, text) { }
        

        private protected override List<byte> ProcessSymbol(char symbol)
        {
            return ProcessSymbolWithAlgorithm(symbol, this.SecondKey, this.FirstKey);
        }
        
    }
}