//	グラフィックプロットテスト
print("グラフィックテストプログラム\n");
plot.Aspect(0);
plot.Window(0, 0, 100, 100);
plot.Color("Blue");
plot.LineType("dash");
plot.Line(10, 10, 90, 90);
plot.LineType("solid");
plot.Color("Red");
plot.Line(10, 10, 90, 10);
plot.Color("Olive");
plot.Line(10, 10, 10, 90);
for (x =10; x < 100; x +=10)
    plot.Arc(x, 100 - x, 1);
plot.Color("Green");
plot.PointSize(3);
plot.PointType("triangle");
for (x =10; x < 100; x +=10)
    plot.Point(x, x + 5);
text = "Xタイトル";
x = 30;
y = 8;
size = 4;
plot.Text(text, x, 8, size);
print(RAD(90), "\n");
plot.Text("Yタイトル", 5, 50, size, RAD(90));

plot.Disp();
