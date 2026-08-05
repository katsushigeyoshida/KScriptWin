println("== 統計計算 ==");
array.clear(a[]);
a[] = { 1, 2, 3, 4, 5 };
b[] = { 2, -2, 13, -4, 5 };
println("a[] =",a[]);
println("b[] =",b[]);
println("配列 a[] の数 = ", array.count(a[]), " 合計 = ", math.sum(a[]));
println("配列 b[] の数 = ", array.count(b[]), " 合計 = ", math.sum(b[]));
println("配列 a[] の最小 = ", math.min(a[]), " 最大 = ", math.max(a[]));
println("配列 b[] の最小 = ", math.min(b[]), " 最大 = ", math.max(b[]));
println("配列 a[] の平均 = ", math.average(a[]));
println("配列 b[] の平均 = ", math.average(b[]));
println("配列 a[] の分散 = ", math.variance(a[]));
println("配列 b[] の分散 = ", math.variance(b[]), " ", round(math.variance(b[])));
println("配列 a[] の標準偏差 = ", math.stdDeviation(a[]));
println("配列 b[] の標準偏差 = ", math.stdDeviation(b[]));

println("\n== 回帰関数 ==");
x[] = {  2,  4,  7,  8,  9 };
y[] = { 30, 50, 70, 40, 80 };
println("x[] =",x[]);
println("y[] =",y[]);
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


println("\n== 2次元配列 ==");
array.clear(a[,]);
a[,] = {{ 1, 2, 3, 4}, {2, 3, 4, 5}, {3, 4, 5, 6 }};
println("配列 a[,] = ");
for (i = 0; i < array.count(a[,0]); i++)
	println(a[i,]);
println("配列 a[,] の数 = ", array.count(a[,]), " 合計 = ", math.sum(a[,]));
println("配列2行目 a[1,] の数 = ", array.count(a[1,]), " 合計 = ", math.sum(a[1,]));
println("配列 a[,] の最小 = ", math.min(a[,]), " 最大 = ", math.max(a[,]));
println("配列2行目 a[1,] の最小 = ", math.min(a[1,]), " 最大 = ", math.max(a[1,]));

println("\n方程式の解");
// 2次方程式の解
a = 1; b = 0; c = -1;
y[] = math.quadraticEquation(a, b, c);
println("y = ",a,"*x^2 + ",b,"*x + ",c);
for(i =0; i< array.count(y[]); i++)
	println("y[",i,"] = ",y[i]);

// 3次方程式の解
a = 1; b = 0; c = -1; d = 0;
y[] = math.qubicEquation(a, b, c, d);
println("y = ",a,"*x^3 + ",b,"*x^2 + ",c,"*x + ",d);
for(i =0; i< array.count(y[]); i++)
	println("y[",i,"] = ",y[i]);

// 4次方程式の解
a = 2; b = 3; c = -4; d = -1.2; e = 2;
y[] = math.quarticEquation(a, b, c, d, e);
println("y = ",a,"*x^4 + ",b,"*x^3 + ",c,"*x^2 + ",d,"*x + ",e);
for(i =0; i< array.count(y[]); i++)
	println("y[",i,"] = ",y[i]);

a = 2; b[] = {2,4,6,0}; c[,] = {{2,3,4},{10,9,8}};
max = math.max(a, b[],c[,]);
min = math.min(a, b[],c[,]);
sum = math.sum(a, b[],c[,]);
ave = math.average (a, b[],c[,]);
println("max = ",max," min = ",min," sum = ",sum," ave = ",ave);