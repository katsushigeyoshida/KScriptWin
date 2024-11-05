#include "scriptLib.sc";
//	ジオメトリック・グラフィック(幾何模様)

//	パターン A
patA_x[] = {  35,  19, 10,  3, 0, -3, -10, -19, -35 };
patA_y[] = { -20, -20, -5, -5, 0, -5,  -5, -20, -20 };
//	パターン B
patB_x[] = { 0, -10, -20,  35 };
patB_y[] = { 0,   0, -20, -20 };
//	パターン C
patC_x[] = { 0, 5,   5, -15, -25 };
patC_y[] = { 0, 8, -20, -20,  -3 };


menu[] = { "パターン1", "パターン2", "パターン3" };
title = "図形の種類";
menuNo = menuSelect(menu[], title);
startTime();
if (menuNo == 0) geometric(patA_x[], patA_y[]);
else if (menuNo == 1) geometric(patB_x[], patB_y[]);
else if (menuNo == 2) geometric(patC_x[], patC_y[]);
println(lapTime());

geometric(x[], y[]) {
    n = count(x[]);
    m = 70;                 // 正三角形の辺の長さ
    h = m * sqrt(3) / 2;    // 正三角形の辺の高さ
    println(n, " , ",m, " , ", h);
    plotWindow(-m / 2, -h, m / 2 + 500, h * 2 / 3 + 330);
    turtleInit();
    b = 1;
    for (vy = 0; vy <= 330; vy += h) {
        a = 1;
        for (vx = 0; vx <= 500; vx += m / 2) {
            for (j = 0; j < 3; j++) {
                for (k = 0; k < n; k++) {
                    px = x[k] * cos(RAD(120 * j)) - y[k] * sin(RAD(120 * j));
                    py = x[k] * sin(RAD(120 * j)) + y[k] * cos(RAD(120 * j));
                    if ((a * b) == -1)
                        py = -py + h / 3;
                    px += vx;
                    py += vy;
                    if (k == 0)
                        turtleSetpoint(px, py);
                    else 
                        turtleMoveto(px, py);
                }
            }
            a = -a;
        }
        b = -b;
    }
}
