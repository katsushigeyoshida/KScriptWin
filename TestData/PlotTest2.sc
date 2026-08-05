xmin  = -10;
xmax  = 110;
ymin  = -10;
ymax  = 110;

plot.Aspect(1);
plot.Window(xmin, ymin, xmax, ymax);
plot.Color("Green");
plot.Line(0,0,100,100);
pl[,]= {{10,10},{30,10}, {30,40}, {10,40}};
plot.Line(pl[,]);
plot.Color("Red");
plot.Fill(1);
plot.FillColor("Blue");
plot.Polygon(pl[,]);
plot.Fill(1);
plot.FillColor("Yellow");
plot.Arc(50,60,20);
plot.Color("Brown");
plot.Line(xmin+10, 0, xmax-10, 0);
plot.Line(0, ymin+10, 0, ymax-10);
//plot.Disp();
array.