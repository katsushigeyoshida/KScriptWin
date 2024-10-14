//	グラフィックプロットテスト
print("グラフィックテストプログラム\n");
plotWindow(0, 0, 100, 100);
plotAspect(0);
plotColor("Blue");
plotLineType("dash");
plotLine(10, 10, 90, 90);
plotLineType("solid");
plotColor("Red");
plotLine(10, 10, 90, 10);
plotColor("Olive");
plotLine(10, 10, 10, 90);
for (x =10; x < 100; x +=10)
    plotArc(x, 100 - x, 1);
plotColor("Green");
plotPointSize(3);
plotPointType("triangle");
for (x =10; x < 100; x +=10)
    plotPoint(x, x + 5);
text = "Xタイトル";
x = 30;
y = 8;
size = 4;
plotText(text, x, 8, size);
print(RAD(90), "\n");
plotText("Yタイトル", 5, 50, size, RAD(90));
