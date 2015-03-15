using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MahjongBingo {
    class Logic {
        private readonly int PAI_AMOUNT = MainForm.PAI_AMOUNT;       //總牌數
        private readonly int SELECT_COUNT_INIT = MainForm.SELECT_COUNT_INIT;
        private readonly int SELECT_COUNT_EXTEND = MainForm.SELECT_COUNT_EXTEND;

        private bool isExtended;
        private int remainingCount;
        private Random ran = new Random();

        public List<Pai> Board { get; private set; }        //盤面區
        public List<Pai> Selection { get; private set; }    //選擇區
        public int[] IsOpened { get; private set; }

        public Logic() {
            Board = new List<Pai>();
            Selection = new List<Pai>();
            IsOpened = new int[PAI_AMOUNT];
            remainingCount = SELECT_COUNT_INIT;

            for (int i = 0; i < PAI_AMOUNT; i++) {
                Board.Add((Pai)i);
                Selection.Add((Pai)i);
                IsOpened[i] = 0;
            }
            Board = Shuffle(Board);
            Selection = Shuffle(Selection);
        }

        //洗牌
        public List<Pai> Shuffle(List<Pai> oldList) {
            List<Pai> newList = new List<Pai>();
            while (oldList.Count > 0) {
                int idx = ran.Next(oldList.Count);
                newList.Add(oldList[idx]);
                oldList.RemoveAt(idx);
            }
            return newList;
        }

        //點開一張牌
        public void OpenPai(int idx) {
            if (remainingCount-- > 0) IsOpened[idx] = 1;
            if (remainingCount == 0) CheckForGameOver();
        }

        //確定遊戲結束 or 聽牌延長
        public void CheckForGameOver() {
            int bingoCount = 0;     //賓果數
            bool isTenpai = false;  //是否聽牌
            for (int i = 0; i < 14; i++) {
                if (CheckForLine((LineType)i) == 6) bingoCount++;
                if (CheckForLine((LineType)i) == 5) isTenpai = true;
            }

            //TODO:messagebox只是暫用，實做時應該用label呈現, 
            //而且呈現的code應該放在mainform，必要時應該設個enum
            if (bingoCount > 0) {
                MessageBox.Show("恭喜你連成 " + bingoCount + " 條線!");
                //TODO:form gameover處理
            } else if (isTenpai && !isExtended) {
                MessageBox.Show("有聽牌可多開 " + SELECT_COUNT_EXTEND + " 張牌!");
                remainingCount += SELECT_COUNT_EXTEND;
                isExtended = true;
            } else {
                MessageBox.Show("遊戲結束!");
                //TODO:form gameover處理
            }
        }

        //檢查某條線中了幾張牌
        public int CheckForLine(LineType lineType) {
            int paiCount = 0;
            switch (lineType) {
                //橫列
                case LineType.row1:
                    paiCount = IsOpened[0] + IsOpened[1] + IsOpened[2] + IsOpened[3] + IsOpened[4] + IsOpened[5]; break;
                case LineType.row2:
                    paiCount = IsOpened[6] + IsOpened[7] + IsOpened[8] + IsOpened[9] + IsOpened[10] + IsOpened[11]; break;
                case LineType.row3:
                    paiCount = IsOpened[12] + IsOpened[13] + IsOpened[14] + IsOpened[15] + IsOpened[16] + IsOpened[17]; break;
                case LineType.row4:
                    paiCount = IsOpened[18] + IsOpened[19] + IsOpened[20] + IsOpened[21] + IsOpened[22] + IsOpened[23]; break;
                case LineType.row5:
                    paiCount = IsOpened[24] + IsOpened[25] + IsOpened[26] + IsOpened[27] + IsOpened[28] + IsOpened[29]; break;
                case LineType.row6:
                    paiCount = IsOpened[30] + IsOpened[31] + IsOpened[32] + IsOpened[33] + IsOpened[34] + IsOpened[35]; break;
                //直行
                case LineType.column1:
                    paiCount = IsOpened[0] + IsOpened[6] + IsOpened[12] + IsOpened[18] + IsOpened[24] + IsOpened[30]; break;
                case LineType.column2:
                    paiCount = IsOpened[1] + IsOpened[7] + IsOpened[13] + IsOpened[19] + IsOpened[25] + IsOpened[31]; break;
                case LineType.column3:
                    paiCount = IsOpened[2] + IsOpened[8] + IsOpened[14] + IsOpened[20] + IsOpened[26] + IsOpened[32]; break;
                case LineType.column4:
                    paiCount = IsOpened[3] + IsOpened[9] + IsOpened[15] + IsOpened[21] + IsOpened[27] + IsOpened[33]; break;
                case LineType.column5:
                    paiCount = IsOpened[4] + IsOpened[10] + IsOpened[16] + IsOpened[22] + IsOpened[28] + IsOpened[34]; break;
                case LineType.column6:
                    paiCount = IsOpened[5] + IsOpened[11] + IsOpened[17] + IsOpened[23] + IsOpened[29] + IsOpened[35]; break;
                //右上左下
                case LineType.slash:
                    paiCount = IsOpened[5] + IsOpened[10] + IsOpened[15] + IsOpened[20] + IsOpened[25] + IsOpened[30]; break;
                //左上右下
                case LineType.backslash:
                    paiCount = IsOpened[0] + IsOpened[7] + IsOpened[14] + IsOpened[21] + IsOpened[28] + IsOpened[35]; break;
                default: break;
            }
            return paiCount;
        }
    }
}
