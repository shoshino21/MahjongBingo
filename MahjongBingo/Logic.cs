using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MahjongBingo {
    class Logic {
        private readonly int PAI_AMOUNT = MainForm.PAI_AMOUNT;                      //總牌數
        private readonly int SELECT_COUNT_INIT = MainForm.SELECT_COUNT_INIT;        //初始牌數
        private readonly int SELECT_COUNT_EXTEND = MainForm.SELECT_COUNT_EXTEND;    //延長後增加牌數

        private int _remainingCount;                        //剩餘牌數
        private Random _ran = new Random();

        public List<Pai> Board { get; private set; }        //盤面區
        public List<Pai> Selection { get; private set; }    //選擇區
        public int[] IsOpened { get; private set; }         //牌是否被點開
        public bool IsExtended { get; private set; }        //是否已延長
        public string Message { get; private set; }         //顯示用文字訊息
        public int GameOverCounter { get; private set; }    //紀錄GameOver次數 (過一定次數show緩和難度鈕)
        public int CurrentSelectCount { get; set; }         //目前牌數設定值

        public Logic() {
            Board = new List<Pai>();
            Selection = new List<Pai>();
            IsOpened = new int[PAI_AMOUNT];
            CurrentSelectCount = SELECT_COUNT_INIT;      //牌數先訂為原始設定
            GameOverCounter = 0;

            for (int i = 0; i < PAI_AMOUNT; i++) {
                Board.Add((Pai)i);
                Selection.Add((Pai)i);
            }
            Initialize(true);
        }

        //遊戲初始化，參數:是否重置盤面
        public void Initialize(bool initializeBoard) {
            if (initializeBoard) Board = Shuffle(Board);

            Selection = Shuffle(Selection);
            IsExtended = false;
            for (int i = 0; i < PAI_AMOUNT; i++) {
                IsOpened[i] = 0;
            }
            _remainingCount = CurrentSelectCount;
            Message = "還有 " + _remainingCount + " 張";
        }

        //洗牌
        public List<Pai> Shuffle(List<Pai> oldList) {
            List<Pai> newList = new List<Pai>();
            while (oldList.Count > 0) {
                int idx = _ran.Next(oldList.Count);
                newList.Add(oldList[idx]);
                oldList.RemoveAt(idx);
            }
            return newList;
        }

        //點開一張牌，回傳遊戲結束與否
        public bool OpenPai(int idx) {
            if (_remainingCount-- > 0) IsOpened[idx] = 1;

            bool isGameOver = false;
            if (_remainingCount == 0) {
                isGameOver = CheckForGameOver();
            } else {
                Message = "還有 " + _remainingCount + " 張";
            }
            return isGameOver;
        }

        //確定遊戲結束 or 聽牌延長，回傳遊戲結束與否
        public bool CheckForGameOver() {
            int bingoCount = 0;         //賓果數
            bool isTenpai = false;      //是否聽牌
            bool isGameOver = false;    //遊戲是否結束

            for (int i = 0; i < 14; i++) {
                if (CheckForLine((LineType)i) == 6) bingoCount++;
                if (CheckForLine((LineType)i) == 5) isTenpai = true;
            }

            if (bingoCount > 0) {
                Message = "恭喜你連成 " + bingoCount + " 條線！送妳大娃娃～";
                isGameOver = true;
            } else if (isTenpai && !IsExtended) {
                Message = "聽牌可多開 " + SELECT_COUNT_EXTEND + " 張牌！";
                _remainingCount += SELECT_COUNT_EXTEND;
                IsExtended = true;
                isGameOver = false;
            } else {
                Message = "你GG惹~";
                isGameOver = true;
                GameOverCounter++;
            }
            return isGameOver;
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
