﻿//	グラフィックテスト
//	KIRA    func 0.78,  2.62
//		         4.86  10.30
//	10/30版 func 0.25   1.35
//		 no func 0.13   0.57
//	SH67	func 3.9    8.2
//		 no func 2.9    6.4
//	10/30版 func 0.13   0.53
//		 no func 0.09   0.47
//
xmin  = -2 * PI;
xmax  =  2 * PI;
xstep = (xmax - xmin) / 200;
ymin  = 0;
ymax  = ymin;

startTime();
for (x = xmin; x <= xmax; x += xstep) {
	fx = func(x);
	if (ymin > fx) ymin = fx;
	if (ymax < fx) ymax = fx;
}
println(lapTime()," s");

plot.Aspect(1);
plot.Window(xmin, ymin, xmax, ymax);
plot.Color("Green");
y = func(xmin);
for (x = xmin; x < xmax; x += xstep) {
	xn = x + xstep;
	yn = func(xn);
	plot.Line(x, y, xn, yn);
	y= yn;
}
println(lapTime()," s");

plot.Color("Brown");
plot.Line(xmin, 0, xmax, 0);
plot.Line(0, ymin, 0, ymax);


func(x) {
	return sin(x) + cos(x * 2) + cos(x * 4);
}
