//	統計処理

x[] = {  2,  4,  7,  8,  9 };
y[] = { 30, 50, 70, 40, 80 };
//	データのグラフ表示
graph.Reset();
graph.GraphType("scatter");
graph.PointSize(3);
graph.SetData(x[],y[]);

count = array.count(x[]);
sum = math.sum(x[]);
ave = math.average(x[]);
println("x[] 数: ",count," 合計: ",sum," 平均: ",ave);
vari = math.variance(x[]);
coor = math.corrCoeff(x[],y[]);
println("x[] 分散: ", vari," 相関係数: ",coor);
//	回帰直線の係数
r[] = math.regression(x[],y[]);
println("回帰直線の係数: y = ", r[0],"x + ", r[1]);
//	回帰直線の端点座標と表示
l[,] ={
	{ x[0], r[0]*x[0]+r[1] }
	{ x[4], r[0]*x[4]+r[1] }
};
plot.Line(l[,]);