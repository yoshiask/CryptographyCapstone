using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaMachine.Models
{
    public class Rotor
    {
        int[][] wiring;
        int position = 0;
        int rotorNo;
        int rotorPos;

        Rotor(int rotorNumber, int rotorPosition) {
            rotorNo = rotorNumber;
            rotorPos = rotorPosition;
            SetWiring(rotorNo);
        }

        public int RunThrough(int input, bool forward) {

            if (forward) {
                input = (input+position) % 26;

                return wiring[input][1];
            }
            else {
                for (int i = 0; i< 26; i++) {
                    if (input == wiring[i][1]) {
                        int output = (wiring[i][0]-position);
                        while (output<0) {
                            output = 26+output;
                        }
                        output = output % 26;

                        return output;
                    }
                }
            }

            return -1;
        }

        void NextRotor() {
            rotorNo = (rotorNo + 1) % 5;
            SetWiring(rotorNo);
        }

        void SetWiring(int rotorNo)
        {
            switch(rotorNo) {
                case 0:
                    wiring = new int[][] { new[] { 0, 15 }, new[] { 1, 4 }, new[] { 2, 25 }, new[] { 3, 20 }, new[] { 4, 14 }, new[] { 5, 7 }, new[] { 6, 23 }, new[] { 7, 18 }, new[] { 8, 2 }, new[] { 9, 21 }, new[] { 10, 5 }, new[] { 11, 12 }, new[] { 12, 19 }, new[] { 13, 1 }, new[] { 14, 6 }, new[] { 15, 11 }, new[] { 16, 17 }, new[] { 17, 8 }, new[] { 18, 13 }, new[] { 19, 16 }, new[] { 20, 9 }, new[] { 21, 22 }, new[] { 22, 0 }, new[] { 23, 24 }, new[] { 24, 3 }, new[] { 25, 10 } };
                    break;
                case 1:
                    wiring = new int[][] { new[] {0, 25 }, new[] {1, 14 }, new[] {2, 20 }, new[] {3, 4 }, new[] {4, 18 }, new[] {5, 24 }, new[] {6, 3 }, new[] {7, 10 }, new[] {8, 5 }, new[] {9, 22 }, new[] {10, 15 }, new[] {11, 2 }, new[] {12, 8 }, new[] {13, 16 }, new[] {14, 23 }, new[] {15, 7 }, new[] {16, 12 }, new[] {17, 21 }, new[] {18, 1 }, new[] {19, 11 }, new[] {20, 6 }, new[] {21, 13 }, new[] {22, 9 }, new[] {23, 17 }, new[] {24, 0 }, new[] {25, 19 } };
                    break;
                case 2:
                    wiring = new int[][] { new[] {0, 4 }, new[] {1, 7 }, new[] {2, 17 }, new[] {3, 21 }, new[] {4, 23 }, new[] {5, 6 }, new[] {6, 0 }, new[] {7, 14 }, new[] {8, 1 }, new[] {9, 16 }, new[] {10, 20 }, new[] {11, 18 }, new[] {12, 8 }, new[] {13, 12 }, new[] {14, 25 }, new[] {15, 5 }, new[] {16, 11 }, new[] {17, 24 }, new[] {18, 13 }, new[] {19, 22 }, new[] {20, 10 }, new[] {21, 19 }, new[] {22, 15 }, new[] {23, 3 }, new[] {24, 9 }, new[] {25, 2 } };
                    break;
                case 3:
                    wiring = new int[][] { new[] {0, 8 }, new[] {1, 12 }, new[] {2, 4 }, new[] {3, 19 }, new[] {4, 2 }, new[] {5, 6 }, new[] {6, 5 }, new[] {7, 17 }, new[] {8, 0 }, new[] {9, 24 }, new[] {10, 18 }, new[] {11, 16 }, new[] {12, 1 }, new[] {13, 25 }, new[] {14, 23 }, new[] {15, 22 }, new[] {16, 11 }, new[] {17, 7 }, new[] {18, 10 }, new[] {19, 3 }, new[] {20, 21 }, new[] {21, 20 }, new[] {22, 15 }, new[] {23, 14 }, new[] {24, 9 }, new[] {25, 13 } };
                    break;
                case 4:
                    wiring = new int[][] { new[] {0, 16 }, new[] {1, 22 }, new[] {2, 4 }, new[] {3, 17 }, new[] {4, 19 }, new[] {5, 25 }, new[] {6, 20 }, new[] {7, 8 }, new[] {8, 14 }, new[] {9, 0 }, new[] {10, 18 }, new[] {11, 3 }, new[] {12, 5 }, new[] {13, 6 }, new[] {14, 7 }, new[] {15, 9 }, new[] {16, 10 }, new[] {17, 15 }, new[] {18, 24 }, new[] {19, 23 }, new[] {20, 2 }, new[] {21, 21 }, new[] {22, 1 }, new[] {23, 13 }, new[] {24, 12 }, new[] {25, 11 } };
                    break; 
            }
        }
    }

    public class Reflector
    {
        int[][] wiring;

        Reflector() {
            wiring = new int[][] { new[] {0, 21}, new[] {1, 10}, new[] {2, 22}, new[] {3, 17}, new[] {4, 6}, new[] {5, 8}, new[] {6, 4}, new[] {7, 19}, new[] {8, 5}, new[] {9, 25}, new[] {10, 1}, new[] {11, 20}, new[] {12, 18}, new[] {13, 15}, new[] {14, 16}, new[] {15, 13}, new[] {16, 14}, new[] {17, 3}, new[] {18, 12}, new[] {19, 7}, new[] {20, 11}, new[] {21, 0}, new[] {22, 2}, new[] {23, 24}, new[] {24, 23}, new[] {25, 9}};
        }

        public int RunThrough(int input, bool forward) {
            input = (input) % 26;
            if (forward) {
                return wiring[input][1];
            } else {
                return wiring[input][0];
            }
        }
    }
}
