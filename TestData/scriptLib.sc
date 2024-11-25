//  Script Library
//  1次元配列表示
printArray(array[]) {
    start = 0;
    size = array.count(array[]);
    for (i = start; i < size; i = i + 1) {
        print(array[i], " ");
    }
    print();
}

//  2次元配列表示
printArray2(array[,]) {
    size = array.count(array[,]);
    count = 0;
    i = 0;
    while (count < size) {
        rowsize = array.count(array[i,]);
        for (j = 0; j < rowsize; j++) {
            print(array[i,j], " ");
            count++;
        }
        print();
        i++;
    }
}

//  最上位の桁で丸める
roundTop(x) {
    w = floor(log(x));
    return round(x, 10^w);
}

//  指定有効桁数での丸め(値,有効桁数)
roundRound(val, digit) {
    order = digit - floor(log(abs(val))) - 1;
    val = round(val * 10^order);
    val /= 10^order;
    if (abs(val) < 1e-28)
        return 0;
    else
        return val;}

//  タートルグラフィック
//  起点の設定
turtleSetpoint(x, y) {
    g_TurtleLPX = x;
    g_TurtleLPY = y;
}

//  方向の設定(dig)
turtleSetAngle(ang) {
    g_TurtleAngle = ang;
}

//  起点から指定点まで線分表示
turtleMoveto(x, y) {
    plot.Line(g_TurtleLPX, g_TurtleLPY, x, y);
    g_TurtleLPX = x;
    g_TurtleLPY = y;
}

//  起点から指定長さの線分表示
turtleMove(length) {
    ang = RAD(g_TurtleAngle);
    x = g_TurtleLPX + length * cos(ang);
    y = g_TurtleLPY + length * sin(ang);
    plot.Line(g_TurtleLPX, g_TurtleLPY, x, y);
    g_TurtleLPX = x;
    g_TurtleLPY = y;
}

//  方向を指定角度回転(dig)
turtleTurn(angle) {
    g_TurtleAngle = (g_TurtleAngle + angle) % 360;
}

//  初期化
turtleInit(){
    g_TurtleAngle = 0;  //  degree
    g_TurtleLPX = 0;
    g_TurtleLPY = 0;
}