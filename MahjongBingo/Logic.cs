using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongBingo {
    class Logic {
        private readonly int PAI_AMOUNT = MainForm.PAI_AMOUNT;       //總牌數

        public List<Pai> Board { get; private set; }        //盤面區
        public List<Pai> Selection { get; private set; }    //選擇區
        public int[] IsHit { get; private set; }
        //TODO:remaining count?

        public Logic() {
            Board = new List<Pai>();
            Selection = new List<Pai>();
            IsHit = new int[PAI_AMOUNT];

            for (int i = 0; i < PAI_AMOUNT; i++) {
                Board.Add((Pai)i);
                Selection.Add((Pai)i);
                IsHit[i] = 0;
            }

            Board = Shuffle(Board);
            Selection = Shuffle(Selection);
        }

        //洗牌
        public List<Pai> Shuffle(List<Pai> oldList) {
            List<Pai> newList = new List<Pai>();
            Random ran = new Random();
            while (oldList.Count > 0) {
                int idx = ran.Next(oldList.Count);
                newList.Add(oldList[idx]);
                oldList.RemoveAt(idx);
            }
            return newList;
        }

        //檢查某條線中了幾張牌
        public int CheckForLine(LineType lineType) {
            int paiCount = 0;
            switch (lineType) {
                //橫列
                case LineType.row1:
                    paiCount = IsHit[0] + IsHit[1] + IsHit[2] + IsHit[3] + IsHit[4] + IsHit[5]; break;
                case LineType.row2:
                    paiCount = IsHit[6] + IsHit[7] + IsHit[8] + IsHit[9] + IsHit[10] + IsHit[11]; break;
                case LineType.row3:
                    paiCount = IsHit[12] + IsHit[13] + IsHit[14] + IsHit[15] + IsHit[16] + IsHit[17]; break;
                case LineType.row4:
                    paiCount = IsHit[18] + IsHit[19] + IsHit[20] + IsHit[21] + IsHit[22] + IsHit[23]; break;
                case LineType.row5:
                    paiCount = IsHit[24] + IsHit[25] + IsHit[26] + IsHit[27] + IsHit[28] + IsHit[29]; break;
                case LineType.row6:
                    paiCount = IsHit[30] + IsHit[31] + IsHit[32] + IsHit[33] + IsHit[34] + IsHit[35]; break;
                //直行
                case LineType.column1:
                    paiCount = IsHit[0] + IsHit[6] + IsHit[12] + IsHit[18] + IsHit[24] + IsHit[30]; break;
                case LineType.column2:
                    paiCount = IsHit[1] + IsHit[7] + IsHit[13] + IsHit[19] + IsHit[25] + IsHit[31]; break;
                case LineType.column3:
                    paiCount = IsHit[2] + IsHit[8] + IsHit[14] + IsHit[20] + IsHit[26] + IsHit[32]; break;
                case LineType.column4:
                    paiCount = IsHit[3] + IsHit[9] + IsHit[15] + IsHit[21] + IsHit[27] + IsHit[33]; break;
                case LineType.column5:
                    paiCount = IsHit[4] + IsHit[10] + IsHit[16] + IsHit[22] + IsHit[28] + IsHit[34]; break;
                case LineType.column6:
                    paiCount = IsHit[5] + IsHit[11] + IsHit[17] + IsHit[23] + IsHit[29] + IsHit[35]; break;
                //右上左下
                case LineType.slash:
                    paiCount = IsHit[5] + IsHit[10] + IsHit[15] + IsHit[20] + IsHit[25] + IsHit[30]; break;
                //左上右下
                case LineType.backslash:
                    paiCount = IsHit[0] + IsHit[7] + IsHit[14] + IsHit[21] + IsHit[28] + IsHit[35]; break;
                default: break;
            }
            return paiCount;
        }


    }
}
