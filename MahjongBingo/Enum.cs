using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongBingo {
    //牌種類，wz:萬 pz:餅 sz:索 kz:四風 sg:三元 fa:花
    enum Pai {
        wz1, wz2, wz3, wz4, wz5, wz6, wz7, wz8, wz9,
        pz1, pz2, pz3, pz4, pz5, pz6, pz7, pz8, pz9,
        sz1, sz2, sz3, sz4, sz5, sz6, sz7, sz8, sz9,
        kz1, kz2, kz3, kz4,
        sg1, sg2, sg3,
        fa1, fa2
    }

    //盤面連線類型，row:橫列 column:直行 slash:/ backslash:\
    enum LineType {
        row1, row2, row3, row4, row5, row6,
        column1, column2, column3, column4, column5, column6,
        slash, backslash
    }
}
