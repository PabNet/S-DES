using System.Collections.Generic;

namespace S_DES.Implementation
{
    public class EncryptionRule
    {
        private protected readonly List<byte> P10 = new() {3, 5, 2, 7, 4, 10, 1, 9, 8, 6},
                                              P8 = new() {6, 3, 7, 4, 8, 5, 10, 9},
                                              P4 = new() {2, 4, 3, 1},
                                              Ip = new() {2, 6, 3, 1, 4, 8, 5, 7},
                                              Ip1 = new() {4, 1, 3, 5, 7, 2, 8, 6},
                                              Ep = new() {4, 1, 2, 3, 2, 3, 4, 1};
        private protected readonly List<List<byte>>
            FirstSBlock = new ()
            {
                new(){ 1, 0, 3, 2 },
                new(){ 3, 2, 1, 0 },
                new(){ 0, 2, 1, 3 },
                new(){ 3, 1, 3, 2 }
            },
            SecondSBlock = new ()
            {
                new(){ 0, 1, 2, 3 },
                new(){ 2, 0, 1, 3 },
                new(){ 3, 0, 1, 0 },
                new(){ 2, 1, 0, 3 }
            };
    }
}