using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*/ 應某人要求寫的麻將賓果 written by shoshino21 2015.3.16 /*/
/*/ https://github.com/shoshino21/MahjongBingo /*/

namespace MahjongBingo {
    public partial class MainForm : Form {
        private Logic _logic;
        private PictureBox[] _selectionPics;
        private AboutForm _aboutForm;

        public static readonly int PAI_AMOUNT = 36;                 //總牌數
        public static readonly int SELECT_COUNT_INIT = 15;          //初始牌數
        public static readonly int SELECT_COUNT_EXTEND = 5;         //延長後增加牌數
        private readonly int GAMEOVER_COUNT_FOR_CHANGEDIFF = 3;     //GameOver幾次後開放緩和難度
        private readonly int CHANGEDIFF_TO = 20;                    //改成幾張牌

        private readonly int BOARD_LENGTH_BY_PAI = 6;               //盤面邊長 = 幾張牌
        private readonly int PAI_WIDTH = 36;                        //牌畫大小
        private readonly int PAI_HEIGHT = 54;
        private readonly int INTERVAL_X = 20;                       //牌間隔
        private readonly int INTERVAL_Y = 20;
        private readonly int MARGIN_LEFT = 20;                      //盤面和視窗邊緣距離
        private readonly int MARGIN_TOP = 50;

        public MainForm() {
            InitializeComponent();
            //設定雙重緩衝，防止重繪畫面時閃爍
            this.DoubleBuffered = true;

            _logic = new Logic();
            _aboutForm = new AboutForm();
            lblMessage.Text = _logic.Message;

            //建立選擇區
            _selectionPics = new PictureBox[PAI_AMOUNT];
            for (int i = 0; i < PAI_AMOUNT; i++) {
                _selectionPics[i] = new PictureBox();
                _selectionPics[i].Image = Properties.Resources.up1;
                _selectionPics[i].Location = new Point(PAI_WIDTH * (i % 12) + 360, PAI_HEIGHT * (i / 12) + 300);
                _selectionPics[i].Size = new Size(PAI_WIDTH, PAI_HEIGHT);
                _selectionPics[i].Parent = this;
                _selectionPics[i].Tag = _logic.Selection[i];

                _selectionPics[i].MouseUp += (o, e) => {
                    PictureBox picSelected = o as PictureBox;
                    picSelected.Image = GetPaiImage((Pai)picSelected.Tag, 1);

                    //找出盤面上的對應牌
                    for (int idx = 0; idx < PAI_AMOUNT; idx++) {
                        if (_logic.Board[idx] == (Pai)picSelected.Tag && _logic.IsOpened[idx] == 0) {
                            bool isGameOver = _logic.OpenPai(idx);
                            //更新文字訊息
                            lblMessage.Text = _logic.Message;
                            //若遊戲結束則停用選擇區
                            if (isGameOver) SwitchSelection(false);
                            //若Gameover太多次則顯示緩和難度按鈕
                            if (_logic.GameOverCounter >= GAMEOVER_COUNT_FOR_CHANGEDIFF && _logic.CurrentSelectCount == SELECT_COUNT_INIT) {
                                btnChangeDiff.Visible = true;
                            }
                            break;
                        }
                    }
                    Invalidate();   //重繪盤面
                };
            }
        }

        //畫面繪製
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            //設定畫面重繪範圍
            int repaintTop = MARGIN_TOP + (PAI_HEIGHT + INTERVAL_Y) * BOARD_LENGTH_BY_PAI;
            int repaintLeft = MARGIN_LEFT + (PAI_WIDTH + INTERVAL_X) * BOARD_LENGTH_BY_PAI;

            if (e.ClipRectangle.Top < repaintTop && e.ClipRectangle.Left < repaintLeft) {
                Graphics g = e.Graphics;
                DrawBaseLine(g);
                DrawTenpaiLine(g);
                DrawBoard(g);
                DrawBingoLine(g);
            }
        }

        //畫線
        private void DrawLine(Graphics g, LineType lineType, Pen pen) {
            Point point1 = new Point();
            Point point2 = new Point();
            //畫橫線
            if ((int)lineType >= 0 && (int)lineType <= 5) {
                int offset = (int)lineType;
                point1.X = PAI_WIDTH / 2 + MARGIN_LEFT;
                point1.Y = PAI_HEIGHT / 2 + (PAI_HEIGHT + INTERVAL_Y) * offset + MARGIN_TOP;
                point2.X = PAI_WIDTH / 2 + (PAI_WIDTH + INTERVAL_X) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_LEFT;
                point2.Y = PAI_HEIGHT / 2 + (PAI_HEIGHT + INTERVAL_Y) * offset + MARGIN_TOP;
            }

            //畫縱線
            else if ((int)lineType >= 6 && (int)lineType <= 11) {
                int offset = (int)lineType - 6;
                point1.X = PAI_WIDTH / 2 + (PAI_WIDTH + INTERVAL_X) * offset + MARGIN_LEFT;
                point1.Y = PAI_HEIGHT / 2 + MARGIN_TOP;
                point2.X = PAI_WIDTH / 2 + (PAI_WIDTH + INTERVAL_X) * offset + MARGIN_LEFT;
                point2.Y = PAI_HEIGHT / 2 + (PAI_HEIGHT + INTERVAL_Y) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_TOP;
            }

            //畫斜線
            else if (lineType == LineType.slash) {
                point1.X = PAI_WIDTH / 2 + (PAI_WIDTH + INTERVAL_X) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_LEFT;
                point1.Y = PAI_HEIGHT / 2 + MARGIN_TOP;
                point2.X = PAI_WIDTH / 2 + MARGIN_LEFT;
                point2.Y = PAI_HEIGHT / 2 + (PAI_HEIGHT + INTERVAL_Y) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_TOP;
            } else if (lineType == LineType.backslash) {
                point1.X = PAI_WIDTH / 2 + MARGIN_LEFT;
                point1.Y = PAI_HEIGHT / 2 + MARGIN_TOP;
                point2.X = PAI_WIDTH / 2 + (PAI_WIDTH + INTERVAL_X) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_LEFT;
                point2.Y = PAI_HEIGHT / 2 + (PAI_HEIGHT + INTERVAL_Y) * (BOARD_LENGTH_BY_PAI - 1) + MARGIN_TOP;
            }
            g.DrawLine(pen, point1, point2);
        }

        //畫盤面底線
        private void DrawBaseLine(Graphics g) {
            Pen pen = new Pen(Color.Black, 3);
            int lineTypeLength = Enum.GetNames(typeof(LineType)).Length;
            for (int i = 0; i < lineTypeLength; i++) {
                DrawLine(g, (LineType)i, pen);
            }
        }

        //畫賓果線
        private void DrawBingoLine(Graphics g) {
            Pen pen = new Pen(Color.Red, 4);
            int lineTypeLength = Enum.GetNames(typeof(LineType)).Length;
            for (int i = 0; i < lineTypeLength; i++) {
                if (_logic.CheckForLine((LineType)i) == 6) {
                    DrawLine(g, (LineType)i, pen);
                }
            }
        }

        //畫聽牌線
        private void DrawTenpaiLine(Graphics g) {
            Pen pen = new Pen(Color.Yellow, 4);
            int lineTypeLength = Enum.GetNames(typeof(LineType)).Length;
            for (int i = 0; i < lineTypeLength; i++) {
                if (_logic.CheckForLine((LineType)i) == 5) {
                    DrawLine(g, (LineType)i, pen);
                }
            }
        }

        //畫盤面區
        private void DrawBoard(Graphics g) {
            Point point = new Point();
            for (int i = 0; i < PAI_AMOUNT; i++) {
                point.X = (PAI_WIDTH + INTERVAL_X) * (i % BOARD_LENGTH_BY_PAI) + MARGIN_LEFT;
                point.Y = (PAI_HEIGHT + INTERVAL_Y) * (i / BOARD_LENGTH_BY_PAI) + MARGIN_TOP;
                Bitmap bmp = new Bitmap(GetPaiImage(_logic.Board[i], _logic.IsOpened[i]));
                g.DrawImage(bmp, point);
                bmp.Dispose();
            }
        }

        //取得指定牌的圖片，wz:萬 pz:餅 sz:索 kz:四風 sg:三元 fa:花
        private Image GetPaiImage(Pai pai, int isOpened) {
            Image img;
            //牌有被點開
            if (isOpened == 1) {
                switch (pai) {
                    case Pai.wz1: img = Properties.Resources.wz1; break;
                    case Pai.wz2: img = Properties.Resources.wz2; break;
                    case Pai.wz3: img = Properties.Resources.wz3; break;
                    case Pai.wz4: img = Properties.Resources.wz4; break;
                    case Pai.wz5: img = Properties.Resources.wz5; break;
                    case Pai.wz6: img = Properties.Resources.wz6; break;
                    case Pai.wz7: img = Properties.Resources.wz7; break;
                    case Pai.wz8: img = Properties.Resources.wz8; break;
                    case Pai.wz9: img = Properties.Resources.wz9; break;
                    case Pai.pz1: img = Properties.Resources.pz1; break;
                    case Pai.pz2: img = Properties.Resources.pz2; break;
                    case Pai.pz3: img = Properties.Resources.pz3; break;
                    case Pai.pz4: img = Properties.Resources.pz4; break;
                    case Pai.pz5: img = Properties.Resources.pz5; break;
                    case Pai.pz6: img = Properties.Resources.pz6; break;
                    case Pai.pz7: img = Properties.Resources.pz7; break;
                    case Pai.pz8: img = Properties.Resources.pz8; break;
                    case Pai.pz9: img = Properties.Resources.pz9; break;
                    case Pai.sz1: img = Properties.Resources.sz1; break;
                    case Pai.sz2: img = Properties.Resources.sz2; break;
                    case Pai.sz3: img = Properties.Resources.sz3; break;
                    case Pai.sz4: img = Properties.Resources.sz4; break;
                    case Pai.sz5: img = Properties.Resources.sz5; break;
                    case Pai.sz6: img = Properties.Resources.sz6; break;
                    case Pai.sz7: img = Properties.Resources.sz7; break;
                    case Pai.sz8: img = Properties.Resources.sz8; break;
                    case Pai.sz9: img = Properties.Resources.sz9; break;
                    case Pai.kz1: img = Properties.Resources.kz1; break;
                    case Pai.kz2: img = Properties.Resources.kz2; break;
                    case Pai.kz3: img = Properties.Resources.kz3; break;
                    case Pai.kz4: img = Properties.Resources.kz4; break;
                    case Pai.sg1: img = Properties.Resources.sg1; break;
                    case Pai.sg2: img = Properties.Resources.sg2; break;
                    case Pai.sg3: img = Properties.Resources.sg3; break;
                    case Pai.fa1: img = Properties.Resources.fa1; break;
                    case Pai.fa2: img = Properties.Resources.fa2; break;
                    default: img = null; break;
                }
            } else {
                switch (pai) {
                    case Pai.wz1: img = Properties.Resources.wz1_d; break;
                    case Pai.wz2: img = Properties.Resources.wz2_d; break;
                    case Pai.wz3: img = Properties.Resources.wz3_d; break;
                    case Pai.wz4: img = Properties.Resources.wz4_d; break;
                    case Pai.wz5: img = Properties.Resources.wz5_d; break;
                    case Pai.wz6: img = Properties.Resources.wz6_d; break;
                    case Pai.wz7: img = Properties.Resources.wz7_d; break;
                    case Pai.wz8: img = Properties.Resources.wz8_d; break;
                    case Pai.wz9: img = Properties.Resources.wz9_d; break;
                    case Pai.pz1: img = Properties.Resources.pz1_d; break;
                    case Pai.pz2: img = Properties.Resources.pz2_d; break;
                    case Pai.pz3: img = Properties.Resources.pz3_d; break;
                    case Pai.pz4: img = Properties.Resources.pz4_d; break;
                    case Pai.pz5: img = Properties.Resources.pz5_d; break;
                    case Pai.pz6: img = Properties.Resources.pz6_d; break;
                    case Pai.pz7: img = Properties.Resources.pz7_d; break;
                    case Pai.pz8: img = Properties.Resources.pz8_d; break;
                    case Pai.pz9: img = Properties.Resources.pz9_d; break;
                    case Pai.sz1: img = Properties.Resources.sz1_d; break;
                    case Pai.sz2: img = Properties.Resources.sz2_d; break;
                    case Pai.sz3: img = Properties.Resources.sz3_d; break;
                    case Pai.sz4: img = Properties.Resources.sz4_d; break;
                    case Pai.sz5: img = Properties.Resources.sz5_d; break;
                    case Pai.sz6: img = Properties.Resources.sz6_d; break;
                    case Pai.sz7: img = Properties.Resources.sz7_d; break;
                    case Pai.sz8: img = Properties.Resources.sz8_d; break;
                    case Pai.sz9: img = Properties.Resources.sz9_d; break;
                    case Pai.kz1: img = Properties.Resources.kz1_d; break;
                    case Pai.kz2: img = Properties.Resources.kz2_d; break;
                    case Pai.kz3: img = Properties.Resources.kz3_d; break;
                    case Pai.kz4: img = Properties.Resources.kz4_d; break;
                    case Pai.sg1: img = Properties.Resources.sg1_d; break;
                    case Pai.sg2: img = Properties.Resources.sg2_d; break;
                    case Pai.sg3: img = Properties.Resources.sg3_d; break;
                    case Pai.fa1: img = Properties.Resources.fa1_d; break;
                    case Pai.fa2: img = Properties.Resources.fa2_d; break;
                    default: img = null; break;
                }
            }
            return img;
        }

        //切換選擇區的可用狀態
        private void SwitchSelection(bool toStatus) {
            foreach (var pic in _selectionPics) {
                pic.Enabled = toStatus;
            }
        }

        //重置遊戲，參數:是否重置盤面
        private void ResetGame(bool isResetBoard) {
            _logic.Initialize(isResetBoard);

            for (int i = 0; i < PAI_AMOUNT; i++) {
                _selectionPics[i].Tag = _logic.Selection[i];
                _selectionPics[i].Image = Properties.Resources.up1;
            }
            SwitchSelection(true);              //啟用選擇區
            lblMessage.Text = _logic.Message;   //更新文字訊息
            Invalidate();                       //重繪盤面
        }

        //重置遊戲鈕
        private void btnResetGame_Click(object sender, EventArgs e) {
            ResetGame(false);
        }

        //重置盤面鈕
        private void btnResetBoard_Click(object sender, EventArgs e) {
            ResetGame(true);
        }

        //緩和難度用
        private void btnChangeDiff_Click(object sender, EventArgs e) {
            _logic.CurrentSelectCount = CHANGEDIFF_TO;
            MessageBox.Show("給你 " + CHANGEDIFF_TO + " 張總能過了吧！");

            ResetGame(false);
            btnChangeDiff.Visible = false;
        }

        //AboutForm
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            _aboutForm.ShowDialog();
        }
    }
}
