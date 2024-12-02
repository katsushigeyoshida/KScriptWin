#include "scriptLib.sc";

//  リカーシブ・グラフィック(再帰曲線)
menu[] = {
    "コッホ曲線1", "コッホ曲線2", "クロスステッチ",
    "樹木曲線"
};

plot.Aspect(1);
plot.Window(0, 0, 400, 400);
plot.Color("Green");
turtleInit();

title = "図形の種類";
menuNo = menuSelect(menu[], title);
startTime();
if (menuNo == 0) koch1();
else if (menuNo == 1) koch2();
else if (menuNo == 2) stech1();
else if (menuNo == 3) tree1();
println(lapTime());

tree1() {
    n = 8;          //  枝の次数
    x0 = 200;       //  起点
    y0 = 50;        //  起点
    leng = 100;     //  長さ
    angle = 90;     //  向き
    g_scale = 1.4;  //  小枝の比率(逆数)
    g_branch = 20;  //  分岐角
    tree(n, x0, y0, leng, angle);
}


tree(n, x0, y0, leng, angle) {
    if (n != 0) {
        turtleSetpoint(x0, y0);
        turtleSetAngle(angle);
        turtleMove(leng);
        x0 = g_TurtleLPX;
        y0 = g_TurtleLPY;
        tree(n - 1, x0, y0, leng / g_scale, angle - g_branch);
        tree(n - 1, x0, y0, leng / g_scale, angle + g_branch);
    }
}

stech1() {
    n = 4;
    leng = 2;
    turtleSetpoint(120, 120);
    turtleSetAngle(0);
    for (k = 1; k <= 4; k++) {
        stech(n, leng);
        turtleTurn(90);
    }
}


stech(n, leng) {
    if (n == 0) {
        turtleMove(leng);
    } else {
        stech(n - 1, leng); turtleTurn(-90);
        stech(n - 1, leng); turtleTurn(90);
        stech(n - 1, leng); turtleTurn(90);
        stech(n - 1, leng); turtleTurn(-90);
        stech(n - 1, leng);
    }
}


koch2() {
    n = 4;
    leng = 4;
    turtleSetpoint(30,300);
    turtleSetAngle(0);
    for (i = 0; i < 3; i++) {
        koch(n, leng);
        turtleTurn(-120);
    }
}

koch1() {
    n = 4;
    leng = 4;
    turtleSetpoint(50,200);
    turtleSetAngle(0);
    koch(n, leng);
}

koch(n, leng) {
    if (n == 0) {
        turtleMove(leng);
    } else {
        koch(n - 1, leng);
        turtleTurn(60);
        koch(n - 1, leng);
        turtleTurn(-120);
        koch(n - 1, leng);
        turtleTurn(60);
        koch(n - 1, leng);
    }
}

