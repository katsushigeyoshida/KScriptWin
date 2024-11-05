//	方程式の解

//	2次方程式の解
//g_a = 1; g_b = 0; g_c = -1;
xmin = -2; xmax = 2;
//y[] = solveQuadraticEquation(g_a, g_b, g_c);

//	3次方程式の解
g_a = 1; g_b = 0; g_c = -1; g_d = 0;
xmin = -1.7; xmax = 1.7;
y[] = solveCubicEquation(g_a, g_b, g_c, g_d);

//	4次方程式の解
//g_a = 2; g_b = 3; g_c = -4; g_d = -1.2; g_e = 2;
//xmin = -1; xmax = 0.5;
//y[] = solveQuarticEquation(g_a, g_b, g_c, g_d, g_e);

//print("方程式の解 : ");
//for (i = 0; i < count(y[]); i++)
//	print(y[i], ", ");
//print();
//
funcPlot(xmin, xmax);

func(x) {
	return x * x - x;
//	return g_a * x^2 + g_b * x + g_c;
//	return g_a * x^3 + g_b * x^2 + g_c * x + g_d;
//	return g_a * x^4 + g_b * x^3 + g_c * x^2 + g_d * x + g_e;
}

funcPlot(xmin, xmax) {
	xstep = (xmax - xmin) / 100;
	ymin = func(xmin);
	ymax = xmin;
	for (x = xmin; x <=xmax; x += xstep) {
		fx =func(x);
		ymin = min(ymin, fx);
		ymax = max(ymax, fx);
	}
	xmargin = (xmax - xmin) / 10;
	ymargin = (ymax - ymin) / 10;
	
	plotAspect(0);
	plotWindow(xmin - xmargin, ymin - ymargin, xmax + xmargin, ymax + ymargin);
	fx = xmin;
	fy = func(xmin);
	for (x = xmin + xstep; x <=xmax; x += xstep) {
		y = func(x);
		plotLine(fx,fy, x, y);
		fx = x;
		fy = y;
	}

	plotColor("Aqua");
	xstep = (xmax - xmin) / 5;
	xstep = roundTop(xstep);
	fontsize = ymargin / 2;
	for (x = roundTop(xmin); x <= xmax; x += xstep) {
		x = roundRound(x, 5);
		if (x < xmin) continue;
		plotLine(x, ymin, x, ymax);
		plotText(x, x, ymin, fontsize, 0, 1, 0);
	}
	ystep = (ymax - ymin) / 4;
	ystep = roundTop(ystep);
	for (y = roundTop(ymin); y <= ymax; y +=ystep) {
		y = roundRound(y, 5);
		plotLine(xmin, y, xmax, y);
		plotText(y, xmin, y,fontsize, 0, 2, 1);
	}
	

	plotColor("Brown");
	plotLine(xmin, 0, xmax, 0);
	plotLine(0, ymin, 0, ymax);
}

//  最上位の桁で丸める
roundTop(x) {
	w = floor(log(x));
	return round(x, 10^w);
}

//	有効桁数での丸め
roundRound(val, digit) {
	order = digit - floor(log(abs(val))) - 1;
	val = round(val * 10^order);
	val /= 10^order;
	if (abs(val) < 1e-28)
		return 0;
	else
		return val;
}
