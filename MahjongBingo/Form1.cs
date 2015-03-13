using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MahjongBingo {
    //牌種類列舉，wz:萬 pz:餅 sz:索 kz:四風 sg:三元 fa:花
    enum Pai {
        wz1, wz2, wz3, wz4, wz5, wz6, wz7, wz8, wz9,
        pz1, pz2, pz3, pz4, pz5, pz6, pz7, pz8, pz9,
        sz1, sz2, sz3, sz4, sz5, sz6, sz7, sz8, sz9,
        kz1, kz2, kz3, kz4,
        sg1, sg2, sg3,
        fa1, fa2
    }

    public partial class MainForm : Form {
        const int IMAGE_WIDTH = 36;
        const int IMAGE_HEIGHT = 54;
        const int BOARD_LENGTH_BY_PAI = 6;
        const int PAI_AMOUNT = 36;
        const int INTERVAL_X = 5;
        const int INTERVAL_Y = 5;

        List<int> board;
        List<int> selection;
        Random ran = new Random();
        PictureBox[] boardPics = new PictureBox[PAI_AMOUNT];
        PictureBox[] selectionPics = new PictureBox[PAI_AMOUNT];
        int[] isHit = new int[PAI_AMOUNT];  //不用boolean，計算連線聽牌比較方便


        public MainForm() {
            InitializeComponent();

            board = new List<int>();
            selection = new List<int>();
            for (int i = 0; i < PAI_AMOUNT; i++) {
                board.Add(i + 1);
                selection.Add(i + 1);
                isHit[i] = 0;
            }

            board = (List<int>)Shuffle(board);
            selection = (List<int>)Shuffle(selection);

            //for (int i = 0; i < 36; i++) {
            //    Console.WriteLine(board[i]);
            //}

            for (int i = 0; i < PAI_AMOUNT; i++) {
                //Point location = new Point(36 * i, 54 * (i / 6));
                //Pai imagePai = (Pai)i;

                boardPics[i] = new PictureBox();
                boardPics[i].Image = getPaiImage(board[i]);
                boardPics[i].Location = new Point((IMAGE_WIDTH + INTERVAL_X) * (i % BOARD_LENGTH_BY_PAI), (IMAGE_HEIGHT + INTERVAL_Y) * (i / BOARD_LENGTH_BY_PAI));
                boardPics[i].Size = new Size(IMAGE_WIDTH, IMAGE_HEIGHT);
                boardPics[i].Parent = this;
                boardPics[i].Tag = (int)board[i];
                //boardPics[i].SendToBack();

                selectionPics[i] = new PictureBox();
                selectionPics[i].Image = Properties.Resources.up1;
                selectionPics[i].Location = new Point(IMAGE_WIDTH * (i % BOARD_LENGTH_BY_PAI) + 300, IMAGE_HEIGHT * (i / BOARD_LENGTH_BY_PAI));
                selectionPics[i].Size = new Size(IMAGE_WIDTH, IMAGE_HEIGHT);
                selectionPics[i].Parent = this;
                selectionPics[i].Tag = (int)selection[i];

                selectionPics[i].MouseUp += (o, e) => {
                    PictureBox picSelected = o as PictureBox;
                    picSelected.Image = getPaiImage((int)picSelected.Tag);

                    //foreach (var boardPic in boardPics) {
                    //    if ((int)boardPic.Tag == (int)picSelected.Tag) {
                    //        boardPic.Image = Properties.Resources.up1;
                    //    }
                    //}

                    //找出盤面上的對應牌
                    for (int idx = 0; idx < PAI_AMOUNT; idx++) {
                        if ((int)boardPics[idx].Tag == (int)picSelected.Tag) {
                            boardPics[idx].Image = Properties.Resources.up1;
                            isHit[idx] = 1;
                            break;
                        }
                    }
                    //CheckForLine();

                };

                //boardPics[i].Paint += (o, e) => {
                //    Graphics g = e.Graphics;
                //    PictureBox picSelected = o as PictureBox;

                //    g.DrawLine(System.Drawing.Pens.Red, picSelected.Left, picSelected.Top, picSelected.Right, picSelected.Bottom);
                //};
            }


        }

        private IEnumerable<int> Shuffle(List<int> listToShuffle) {
            List<int> randomizedBoard = new List<int>();
            while (listToShuffle.Count > 0) {
                int idx = ran.Next(listToShuffle.Count);
                randomizedBoard.Add(listToShuffle[idx]);
                listToShuffle.RemoveAt(idx);
            }
            return randomizedBoard;
        }

        private Image getPaiImage(int i) {
            Image retImage;

            switch (i) {
                case 1: retImage = Properties.Resources.wz1; break;
                case 2: retImage = Properties.Resources.wz2; break;
                case 3: retImage = Properties.Resources.wz3; break;
                case 4: retImage = Properties.Resources.wz4; break;
                case 5: retImage = Properties.Resources.wz5; break;
                case 6: retImage = Properties.Resources.wz6; break;
                case 7: retImage = Properties.Resources.wz7; break;
                case 8: retImage = Properties.Resources.wz8; break;
                case 9: retImage = Properties.Resources.wz9; break;
                case 10: retImage = Properties.Resources.pz1; break;
                case 11: retImage = Properties.Resources.pz2; break;
                case 12: retImage = Properties.Resources.pz3; break;
                case 13: retImage = Properties.Resources.pz4; break;
                case 14: retImage = Properties.Resources.pz5; break;
                case 15: retImage = Properties.Resources.pz6; break;
                case 16: retImage = Properties.Resources.pz7; break;
                case 17: retImage = Properties.Resources.pz8; break;
                case 18: retImage = Properties.Resources.pz9; break;
                case 19: retImage = Properties.Resources.sz1; break;
                case 20: retImage = Properties.Resources.sz2; break;
                case 21: retImage = Properties.Resources.sz3; break;
                case 22: retImage = Properties.Resources.sz4; break;
                case 23: retImage = Properties.Resources.sz5; break;
                case 24: retImage = Properties.Resources.sz6; break;
                case 25: retImage = Properties.Resources.sz7; break;
                case 26: retImage = Properties.Resources.sz8; break;
                case 27: retImage = Properties.Resources.sz9; break;
                case 28: retImage = Properties.Resources.kz1; break;
                case 29: retImage = Properties.Resources.kz2; break;
                case 30: retImage = Properties.Resources.kz3; break;
                case 31: retImage = Properties.Resources.kz4; break;
                case 32: retImage = Properties.Resources.sg1; break;
                case 33: retImage = Properties.Resources.sg2; break;
                case 34: retImage = Properties.Resources.sg3; break;
                case 35: retImage = Properties.Resources.fa1; break;
                case 36: retImage = Properties.Resources.fa2; break;
                default: retImage = null; break;
            }
            return retImage;
        }

        //檢查連線
        private void CheckForLine() {
            int row1 = isHit[0] + isHit[1] + isHit[2] + isHit[3] + isHit[4] + isHit[5];
            int row2 = isHit[6] + isHit[7] + isHit[8] + isHit[9] + isHit[10] + isHit[11];
            int row3 = isHit[12] + isHit[13] + isHit[14] + isHit[15] + isHit[16] + isHit[17];
            int row4 = isHit[18] + isHit[19] + isHit[20] + isHit[21] + isHit[22] + isHit[23];
            int row5 = isHit[24] + isHit[25] + isHit[26] + isHit[27] + isHit[28] + isHit[29];
            int row6 = isHit[30] + isHit[31] + isHit[32] + isHit[33] + isHit[34] + isHit[35];

            int column1 = isHit[0] + isHit[6] + isHit[12] + isHit[18] + isHit[24] + isHit[30];
            int column2 = isHit[1] + isHit[7] + isHit[13] + isHit[19] + isHit[25] + isHit[31];
            int column3 = isHit[2] + isHit[8] + isHit[14] + isHit[20] + isHit[26] + isHit[32];
            int column4 = isHit[3] + isHit[9] + isHit[15] + isHit[21] + isHit[27] + isHit[33];
            int column5 = isHit[4] + isHit[10] + isHit[16] + isHit[22] + isHit[28] + isHit[34];
            int column6 = isHit[5] + isHit[11] + isHit[17] + isHit[23] + isHit[29] + isHit[35];

            int slash1 = isHit[0] + isHit[7] + isHit[14] + isHit[21] + isHit[28] + isHit[35];       //左上右下
            int slash2 = isHit[5] + isHit[10] + isHit[15] + isHit[20] + isHit[25] + isHit[30];      //右上左下
        }

        private void DrawBoardLine() {
            Graphics g = this.CreateGraphics();
            Pen pen = new Pen(Color.Black, 5);
            Point point1 = new Point();
            Point point2 = new Point();

            for (int i = 0; i < BOARD_LENGTH_BY_PAI; i++) {
                point1.X = IMAGE_WIDTH / 2;
                point1.Y = IMAGE_HEIGHT / 2 + (IMAGE_HEIGHT + INTERVAL_Y) * i;
                point2.X = IMAGE_WIDTH / 2 + (IMAGE_WIDTH + INTERVAL_X)*5;
                point2.Y = IMAGE_HEIGHT / 2 + (IMAGE_HEIGHT + INTERVAL_Y) * i;
                g.DrawLine(pen, point1, point2);
            }

            for (int i = 0; i < BOARD_LENGTH_BY_PAI; i++) {
                point1.X = IMAGE_WIDTH / 2 + (IMAGE_WIDTH + INTERVAL_X) * i;
                point1.Y = IMAGE_HEIGHT / 2;
                point2.X = IMAGE_WIDTH / 2 + (IMAGE_WIDTH + INTERVAL_X) * i;
                point2.Y = IMAGE_HEIGHT / 2 + (IMAGE_HEIGHT + INTERVAL_Y)*5;
                g.DrawLine(pen, point1, point2);
            }
            g.DrawLine(pen, point1, point2);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            DrawBoardLine();

        }
    }
}
