//	タートルグラフィック
title = "タートルグラフィック";
menu[] = { "ポリゴン", "渦巻(89,1)", "渦巻(73,1)",
		"渦巻(90,4)", "渦巻(120,4)", "渦巻(122,4)",
		"渦巻(145,4)" };
menuNo = menuSelect(menu[], title);
startTime();
if (menuNo == 0) polygonPlot();
else if (menuNo == 1) spiralPlot(89, 1);
else if (menuNo == 2) spiralPlot(73, 1);
else if (menuNo == 3) spiralPlot(90, 4);
else if (menuNo == 4) spiralPlot(120, 4);
else if (menuNo == 5) spiralPlot(122, 4);
else if (menuNo == 6) spiralPlot(145, 4);
println(lapTime());

spiralPlot(ang, step) {
	plotWindow(-200, -200, 200, 200);
	plotAspect(1);
	plotColor("Green");
	turtleInit();
	leng = 200;		//	辺の初期値
//	step = 1;		//	辺の減少値
//	ang = 89;		//	回転角
	x = leng / 2;
	y = tan(RAD((180 - ang) / 2 )) * x;
	turtleSetpoint(-x, -y);
	turtleSetAngle(0);
	while (leng > 10) {
		turtleMove(leng);
		turtleTurn(ang);
		leng -= step;
	}
}


polygonPlot() {
	plotWindow(-20, -40, 100, 300);
	plotAspect(1);
	plotColor("Green");
	turtleInit();
	for (n = 3; n < 10; n++) {
		for (j = 1; j <= n; j++) {
			turtleMove(80);
			turtleTurn(360 / n);
		}
	}
}

//	タートルグラフィック
turtleSetpoint(x, y) {
	g_TurtleLPX = x;
	g_TurtleLPY = y;
}

turtleSetAngle(ang) {
	g_TurtleAngle = ang;
}


turtleMove(l) {
	ang = RAD(g_TurtleAngle);
	x = g_TurtleLPX + l * cos(ang);
	y = g_TurtleLPY + l * sin(ang);
	plotLine(g_TurtleLPX, g_TurtleLPY, x, y);
	g_TurtleLPX = x;
	g_TurtleLPY = y;
}

turtleTurn(angle) {
	g_TurtleAngle = (g_TurtleAngle + angle) % 360;
}

turtleInit(){
	g_TurtleAngle = 0;	//	degree
	g_TurtleLPX = 0;
	g_TurtleLPY = 0;
}