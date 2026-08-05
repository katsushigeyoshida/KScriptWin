gtype[] = {"line", "scatter", "bar" };

graph.SetSplitArea(2,1);
graph.SetUseArea(0);
graph.FontSize(12);
graph.Title("Test");
graph.XTitle("X-軸");
graph.YTitle("Y-軸");

data[,] = { 
    {"A社","B社","C社"}, // X軸タイトル
    {2,4,3},            // Green
    {3,3,4},            // Red
    {4,6,5},            // Yerrow
};
graph.GraphType("Bar");
graph.SetColor("Black");
graph.SetFillColor("Yellow");
graph.BarCount(3);
graph.BarPosition(2);
graph.SetData(data[0,], data[3,]);
graph.BarPosition(1);
graph.SetFillColor("Red");
graph.AddData(data[0,], data[2,]);
graph.BarPosition(0);
graph.SetFillColor("Green");
graph.AddData(data[0,], data[1,]);

x[] = array.linspace(-100,100,20);
y[] = x[];
express = "([x]/5)^3/5+[x]^2/10";
array.calc(y[], express);

graph.SetUseArea(3);
graph.Title(express);
graph.XTitle("[X]");
graph.YTitle("[Y]");
graph.GraphType(gtype[0]);
graph.LineType("solid");
graph.SetColor("Black");
//graph.SetDataArea(-5,0,50,10);
graph.SetData(x[], y[]);

graph.GraphType(gtype[1]);
graph.SetColor("Red");
graph.PointType("circle");
graph.PointSize(2);
graph.AddData(x[],y[]);


xcount = array.count(x[]);
ycount = array.count(y[]);
plot.Color("Blue");
plot.Line(x[0],y[0],x[xcount-1],y[ycount-1]);

